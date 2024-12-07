using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;

namespace mathAi_backend.Helpers;

[Experimental("OPENAI001")]
public class AssistantClientHelper
{
    private readonly OpenAIFileClient _fileClient;
    private readonly AssistantClient _assistantClient;

    public AssistantClientHelper(IConfiguration config)
    {
        var openAiClient = new OpenAIClient(apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
        
        _fileClient = openAiClient.GetOpenAIFileClient();
        _assistantClient = openAiClient.GetAssistantClient();
    }

    public async Task<Assistant> CreateExerciseAssistant()
    {
        var assistantOptions = new AssistantCreationOptions
        {
            Name = "Asystent Oceny Zadań",
            Instructions = "Jesteś asystentem przeznaczonym do oceniania i udzielania informacji zwrotnej na temat rozwiązań uczniów. Analizuj treść zadania oraz załączony obraz z rozwiązaniem. Przyznaj ocenę w skali od 1 do 100 i zapewnij szczegółową informację zwrotną.",
            Tools =
            {
                new FileSearchToolDefinition(),
            },
        };

        return await _assistantClient.CreateAssistantAsync("gpt-4o", assistantOptions);
    }

    public async Task<OpenAIFile> UploadSolutionImageAsync(byte[] solutionImage, string fileName)
    {
        using var imageStream = new MemoryStream(solutionImage);
        var uploadedFile = await _fileClient.UploadFileAsync(
            imageStream,
            fileName,
            FileUploadPurpose.Assistants
        );
        return uploadedFile;
    }

    public async Task<(int grade, string feedback)> GradeExerciseSolutionAsync(Assistant assistant, string exerciseContent, OpenAIFile solutionImageFile)
    {
        var threadOptions = new ThreadCreationOptions
        {
            InitialMessages = {
                $"Przeanalizuj poniższe zadanie oraz załączone rozwiązanie. Przyznaj ocenę (1-100) i udziel informacji zwrotnej:\n\n" +
                $"Zadanie:\n{exerciseContent}\n\nRozwiązanie zostało dostarczone jako plik graficzny: ID pliku - {solutionImageFile.Id}"
            }
        };
        
        var threadRun = await _assistantClient.CreateThreadAndRunAsync(assistant.Id, threadOptions);
        
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            threadRun = await _assistantClient.GetRunAsync(threadRun.Value.ThreadId, threadRun.Value.Id);
        } while (!threadRun.Value.Status.IsTerminal);

        if (threadRun.Value.Status != RunStatus.Completed)
        {
            throw new Exception("The assistant could not complete the grading.");
        }
        
        CollectionResult<ThreadMessage> messages = _assistantClient.GetMessages(threadRun.Value.ThreadId, new MessageCollectionOptions { Order = MessageCollectionOrder.Ascending });
        
        foreach (var message in messages)
        {
            if (message.Role != MessageRole.Assistant) continue;
            
            foreach (var contentItem in message.Content)
            {
                if (string.IsNullOrEmpty(contentItem.Text)) continue;
                // Extract grade and feedback (assumes JSON response structure)
                var response = System.Text.Json.JsonSerializer.Deserialize<GradeFeedbackResponse>(contentItem.Text);
                if (response is { Grade: not null } && !string.IsNullOrEmpty(response.Feedback))
                {
                    return (response.Grade.Value, response.Feedback);
                }
            }
        }

        throw new Exception("Failed to parse grading response.");
    }


    private record GradeFeedbackResponse
    {
        public int? Grade { get; init; }
        public string? Feedback { get; init; }
    }
}
