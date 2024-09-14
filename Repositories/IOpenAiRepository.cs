using OpenAI.Chat;

namespace mathAi_backend.Repositories;

public interface IOpenAiRepository
{
    public ChatClient CreateChatClient();
}