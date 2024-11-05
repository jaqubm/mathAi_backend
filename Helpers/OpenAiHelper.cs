using mathAi_backend.Dtos;
using OpenAI.Chat;

namespace mathAi_backend.Helpers;

public class OpenAiHelper(IConfiguration config)
{
    public ChatClient CreateChatClient()
    {
        return new ChatClient(model: "gpt-4o", apiKey: config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
    }
    
    public static string GenerateExerciseSetPrompt(ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        return $"Wygeneruj zadanie z Matematyki, wraz z trzema podpowiedziami oraz odpowiedzią dla ucznia " +
               $"ze szkoły: {exerciseSetGenerator.SchoolType}, " +
               $"klasa: {exerciseSetGenerator.Grade}, " +
               $"o tematyce: {exerciseSetGenerator.Subject}. " + 
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