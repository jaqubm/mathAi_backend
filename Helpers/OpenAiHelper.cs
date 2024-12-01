using System.ClientModel;
using mathAi_backend.Dtos;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;

namespace mathAi_backend.Helpers;

public class OpenAiHelper(IConfiguration config)
{
    public ChatClient CreateChatClient()
    {
        return new ChatClient(model: "gpt-4o", apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
    }

    public static async Task<ClientResult<ChatCompletion>> GenerateExercise(ChatClient client, ExerciseSetSettingsDto exerciseSetSettings)
    {
        return await client.CompleteChatAsync([GetGenerateExercisePrompt(exerciseSetSettings)], GetGenerateExerciseOptions());
    }

    private static UserChatMessage GetGenerateExercisePrompt(ExerciseSetSettingsDto exerciseSetSettings)
    {
        return new UserChatMessage(
            "Wygeneruj zadanie z Matematyki dla ucznia " +
            $"ze szkoły: {exerciseSetSettings.SchoolType}, " +
            $"klasa: {exerciseSetSettings.Grade}, " +
            $"o tematyce: {exerciseSetSettings.Subject}. " +
            "Zadanie powinno być ciekawe i rozbudowane. " + 
            (exerciseSetSettings.Personalized.IsNullOrEmpty() ? string.Empty : $"\n\nSpersonalizuj zadania, aby dotyczyły: {exerciseSetSettings.Personalized}. ") +
            "Zadanie powinno składać się z treści zadania - Content, trzech podpowiedzi - FirstHint, SecondHint, ThirdHint, oraz odpowiedzi - Solution. " +
            "Wzory formatuj w taki sposób, aby były możliwe do wyświetlenia z MathJax. " +
            @"Przykład formatowania wzorów: $$ T(t) = 5 \cdot \sin\left(\frac{\pi}{12}t \right) + 20 $$. " +
            "Oto przykład w jakim formacie powinieneś zwracać dane - nie formatuj ich jako markdown tylko niesformatowany tekst za wyjątkiem wzorów: " +
            
            """
            Rozważ funkcję trygonometryczną oraz jej złożoność:
            \[
             f(x) = \tan\left(x + \frac{\pi}{4}\right) - \sin(2x)
            \]

            1. Zbadaj dziedzinę funkcji \(f(x)\)

            2. Znajdź miejsca zerowe funkcji \(f(x)\).

            3. Oblicz wartości funkcji \(f(x)\) w punktach \(x = 0, \frac{\pi}{2}\), i \(\pi\).

            4. Wyznacz przeciwną funkcję odwrotną \(f^{-1}(x)\). Kiedy jest to możliwe?

            5. Skonstruuj wykres funkcji \(f(x)\) dla przedziału \([-\frac{\pi}{2}, \frac{3\pi}{2}]\). 
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