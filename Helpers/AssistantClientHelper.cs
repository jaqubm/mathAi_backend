using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
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
        var openAiClient = new OpenAIClient(apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ?? "");
        
        _fileClient = openAiClient.GetOpenAIFileClient();
        _assistantClient = openAiClient.GetAssistantClient();
    }

    /// <summary>
    /// Uploads an image for vision purposes.
    /// </summary>
    public async Task<OpenAIFile> UploadSolutionImageAsync(byte[] solutionImage, string fileName)
    {
        using var imageStream = new MemoryStream(solutionImage);
        // Use Vision upload purpose for images
        var uploadedFile = await _fileClient.UploadFileAsync(
            imageStream,
            fileName,
            FileUploadPurpose.Vision
        );
        return uploadedFile;
    }

    public async Task<Assistant> CreateExerciseAssistant(OpenAIFile openAiFile)
    {
        var assistantOptions = new AssistantCreationOptions
        {
            Name = "Asystent Oceny Zadań",
            Instructions = "Jesteś asystentem przeznaczonym do oceniania i udzielania informacji zwrotnej na temat rozwiązań uczniów. " +
                           "Analizuj treść zadania oraz załączony obraz z rozwiązaniem. " +
                           "Przyznaj ocenę w skali od 1 do 100 i zapewnij szczegółową informację zwrotną. " +
                           "Zwróć odpowiedź w następującym formacie JSON:\n" +
                           "{ \"Grade\": <int>, \"Feedback\": \"<string>\" }",
        };

        // Use a vision-capable model, e.g. "gpt-4o"
        return await _assistantClient.CreateAssistantAsync("gpt-4o", assistantOptions);
    }

    /// <summary>
    /// Creates a thread that includes the image as a message to the assistant.
    /// </summary>
    public async Task<(int grade, string feedback)> GradeExerciseSolutionAsync(Assistant assistant, string exerciseContent, OpenAIFile solutionImageFile)
    {
        // Provide both the textual prompt and the image in the initial messages
        var threadOptions = new ThreadCreationOptions
        {
            InitialMessages = 
            {
                new ThreadInitializationMessage(
                    MessageRole.User,
                    [
                        MessageContent.FromText(
                            "Przeanalizuj poniższe zadanie oraz załączone rozwiązanie. " +
                            "Przyznaj ocenę (1-100) i udziel informacji zwrotnej:\n\n" +
                            $"Zadanie:\n{exerciseContent}\n\n" +
                            "Rozwiązanie graficzne:"),
                        MessageContent.FromImageFileId(solutionImageFile.Id)
                    ]
                )
            }
        };

        // Create the thread and run the assistant
        var threadRun = await _assistantClient.CreateThreadAndRunAsync(assistant.Id, threadOptions);

        // Poll until the run completes
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            threadRun = await _assistantClient.GetRunAsync(threadRun.Value.ThreadId, threadRun.Value.Id);
        } while (!threadRun.Value.Status.IsTerminal);

        if (threadRun.Value.Status != RunStatus.Completed)
        {
            throw new Exception("The assistant could not complete the grading.");
        }

        // Retrieve the messages and look for the assistant's response
        CollectionResult<ThreadMessage> messages = _assistantClient.GetMessages(threadRun.Value.ThreadId, new MessageCollectionOptions { Order = MessageCollectionOrder.Ascending });
        
        foreach (var message in messages)
        {
            if (message.Role != MessageRole.Assistant) continue;
            
            foreach (var contentItem in message.Content)
            {
                if (string.IsNullOrEmpty(contentItem.Text)) continue;
                
                var jsonString = contentItem.Text;
                
                if (jsonString.StartsWith("```json") && jsonString.EndsWith("```"))
                {
                    var lines = jsonString.Split('\n');
        
                    if (lines.Length > 2)
                        jsonString = string.Join("\n", lines.Skip(1).Take(lines.Length - 2));
                }

                try
                {
                    var response = JsonSerializer.Deserialize<GradeFeedbackResponse>(jsonString);
                    if (response?.Grade is not null && !string.IsNullOrEmpty(response.Feedback))
                    {
                        return (response.Grade.Value, response.Feedback);
                    }
                }
                catch (JsonException)
                {
                    Console.WriteLine($"Invalid JSON received: {contentItem.Text}");
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
