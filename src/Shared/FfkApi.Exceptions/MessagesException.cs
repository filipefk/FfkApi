namespace FfkApi.Exceptions;

public static class MessagesException
{
    public static string GetString(string key)
    {
        return ResourceMessagesException.ResourceManager.GetString(key)!;
    }
}
