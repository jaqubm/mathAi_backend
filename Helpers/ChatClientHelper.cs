using System.ClientModel;
using mathAi_backend.Dtos;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;

namespace mathAi_backend.Helpers;

public class ChatClientHelper(IConfiguration config)
{
    public ChatClient CreateChatClient()
    {
        return new ChatClient(model: "gpt-4o", apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
    }

    public static async Task<ClientResult<ChatCompletion>> GenerateExercise(ChatClient client, ExerciseSetSettingsDto exerciseSetSettings)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Jesteś asystentem, który nie korzysta z formatowania Markdown. " +
                                  "Używaj wyłącznie czystego tekstu oraz notacji LaTeX do wzorów matematycznych. " +
                                  "Nie stosuj pogrubień, nagłówków, list ani innego formatowania Markdown."),
            GetGenerateExercisePrompt(exerciseSetSettings)
        };

        return await client.CompleteChatAsync(messages, GetGenerateExerciseOptions());
    }


    private static UserChatMessage GetGenerateExercisePrompt(ExerciseSetSettingsDto exerciseSetSettings)
    {
        return new UserChatMessage(
            "Wygeneruj zadanie z Matematyki dla ucznia " +
            $"ze szkoły: {exerciseSetSettings.SchoolType}, " +
            $"klasa: {exerciseSetSettings.Grade}, " +
            $"o tematyce: {exerciseSetSettings.Subject}. " +
            "Zadanie powinno być ciekawe i rozbudowane. " + 
            (exerciseSetSettings.Personalized.IsNullOrEmpty() ? string.Empty : $"\nSpersonalizuj zadania, aby dotyczyły: {exerciseSetSettings.Personalized}. ") +
            "Zadanie powinno składać się z treści zadania - Content, trzech podpowiedzi - FirstHint, SecondHint, ThirdHint, oraz odpowiedzi - Solution. " +
            "Wzory formatuj w taki sposób, aby były możliwe do wyświetlenia z MathJax. " +
            @"Przykład formatowania wzorów: $$ f(x) = \tan\left(x + \frac{\pi}{4}\right) - \sin(2x) $$. " +
            "Formatuj tekst jak zadania matematyczne. Nie korzystaj z Markdown. " +
            "Oto przykład w jakim formacie powinieneś zwracać dane: " +
            
            """
            Rozważ funkcję trygonometryczną oraz jej złożoność:
            \[
             f(x) = \tan\left(x + \frac{\pi}{4}\right) - \sin(2x)
            \]
            """
        );
    }

    private static ChatCompletionOptions GetGenerateExerciseOptions()
    {
        return new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "exercise",
                jsonSchema: BinaryData.FromString(
                    """
                    {
                        "type": "object",
                        "properties": {
                            "Content": {
                                "type": "string"
                            },
                            "FirstHint": {
                                "type": "string"
                            },
                            "SecondHint": {
                                "type": "string"
                            },
                            "ThirdHint": {
                                "type": "string"
                            },
                            "Solution": {
                                "type": "string"
                            }
                        },
                        "required": ["Content", "FirstHint", "SecondHint", "ThirdHint", "Solution"],
                        "additionalProperties": false
                    }
                    """
                    ),
                jsonSchemaIsStrict: true
                )
        };
    }
}
