using mathAi_backend.Dtos;
using OpenAI.Chat;

namespace mathAi_backend.Helpers;

public class OpenAiHelper(IConfiguration config)
{
    public ChatClient CreateChatClient()
    {
        return new ChatClient(model: "gpt-4o", apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
    }
    
    public static string GenerateExerciseSetPrompt(ExerciseSetSettingsDto exerciseSetSettings)
    {
        return $"Wygeneruj zadanie z Matematyki, wraz z trzema podpowiedziami oraz odpowiedzią dla ucznia " +
               $"ze szkoły: {exerciseSetSettings.SchoolType}, " +
               $"klasa: {exerciseSetSettings.Grade}, " +
               $"o tematyce: {exerciseSetSettings.Subject}. " + 
               $"Zadanie powinno być ciekawe i rozbudowane.\n" + 
               ExerciseAnswerFormat();
    }
    
    private static string ExerciseAnswerFormat()
    {
        return "Odpowiedź odeślij w formacie JSON, tak jak w przykładzie, w formacie gotowym do wyświetlenia na stronie internetowej:" +
               "{Content: string," +
               "FirstHint: string," +
               "SecondHint: string," +
               "ThirdHint: string," +
               "Solution: string}";
    }
}