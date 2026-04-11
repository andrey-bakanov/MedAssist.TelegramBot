using MedAssist.TelegramBot.Worker.Models;

namespace MedAssist.TelegramBot.Worker.Services.State;

public class UserState
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public required UserChatIdentity Identity { get; init; }

    /// <summary>
    /// регистрация пользователя
    /// </summary>
    public required bool IsRegistered { get; init; } = false;

    /// <summary>
    /// Последнее сохраненное состояние
    /// </summary>
    public int? AwaitingReplyMessageId { get; set; }

    /// <summary>
    /// Последня команда
    /// </summary>
    public string? LastCommandName { get; set; }


    /// <summary>
    /// Последня команда
    /// </summary>
    public string LastLLMResponse { get; set; }

    /// <summary>
    /// Контекст пациента
    /// </summary>
    public NamedItem? ClientName { get; set; }
}