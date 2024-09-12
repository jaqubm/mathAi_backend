using OpenAI.Chat;

namespace mathAi_backend.Repositories;

public class OpenAiRepository(IConfiguration config) : IOpenAiRepository
{
    public ChatClient CreateChatClient()
    {
        return new ChatClient(model: "gpt-4o-mini", config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
    }
}