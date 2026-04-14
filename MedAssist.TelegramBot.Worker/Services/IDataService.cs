using MedAssist.TelegramBot.Worker.Services.Api;

namespace MedAssist.TelegramBot.Worker.Services;

/// <summary>
/// Интерфейс для сервиса данных, предоставляющего доступ к специальностям, профилям пользователей, клиентам и другим данным через API.
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Асинхронно получает список специальностей.
    /// </summary>
    /// <returns>Коллекция специальностей.</returns>
    ValueTask<IEnumerable<Speciality>> GetSpecialitiesAsync();

    /// <summary>
    /// Асинхронно получает текст подтверждения условий использования.
    /// </summary>
    /// <returns>Текст подтверждения.</returns>
    ValueTask<string> GetConfirmationTextAsync();

    /// <summary>
    /// Асинхронно получает текст помощи.
    /// </summary>
    /// <returns>Текст помощи.</returns>
    ValueTask<string> GetHelpTextAsync();

    /// <summary>
    /// Асинхронно обновляет специальность пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="specialityCode">Код специальности.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task UpdateSpecialityAsync(long userId, string specialityCode);

    /// <summary>
    /// Асинхронно регистрирует пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="username">Имя пользователя.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task RegisterAsync(long userId, string username);

    /// <summary>
    /// Асинхронно отменяет регистрацию пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task UnregisterAsync(long userId);

    /// <summary>
    /// Асинхронно получает информацию о профиле пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <returns>Профиль пользователя или null, если не найден.</returns>
    Task<UserProfile?> GetUserInfoAsync(long userId);

    /// <summary>
    /// Асинхронно получает информацию о последнем незавершенном диалоге пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <returns>Профиль пользователя или null, если не найден.</returns>
    Task<PatientDialogDto?> GetClientDialogInfoAsync(long userId);

    /// <summary>
    /// Асинхронно получает список клиентов пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <returns>Коллекция клиентов.</returns>
    Task<IEnumerable<Client>> GetClientsAsync(long userId);

    /// <summary>
    /// Асинхронно получает информацию о клиенте.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="clientId">Уникальный идентификатор клиента.</param>
    /// <returns>Информация о клиенте.</returns>
    Task<Client> GetClientInfoAsync(long userId, Guid clientId);

    /// <summary>
    /// Асинхронно удаляет информацию о клиенте.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="clientId">Уникальный идентификатор клиента.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task DeleteClientInfoAsync(long userId, Guid clientId);

    /// <summary>
    /// Асинхронно создает нового клиента.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="clientName">Имя клиента.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task CreateClientInfoAsync(long userId, string clientName);

    /// <summary>
    /// Асинхронно отправляет сообщение чата.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="text">Сообщение пользователя.</param>
    /// <param name="overridedSpecialityCode">Переопределенный код специальности</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task<ChatMessageDto> SendChatMessage(long userId, string text, string? overridedSpecialityCode);

    /// <summary>
    /// Асинхронно отправляет сообщение чата.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="text">Сообщение пользователя.</param>
    /// <param name="clientId">Идентификатор запроса</param>
    /// <param name="overridedSpecialityCode">Переопределенный код специальности</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task<ChatMessageDto> SendClientChatMessage(long userId, string text, Guid clientId, string? overridedSpecialityCode);

    /// <summary>
    /// Начинает новый диалог с пользователем
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="clientId">Идентификатор пациента</param>
    /// <returns></returns>
    Task<StartNewDialogDto> StartClientDialog(long userId, Guid clientId);

    /// <summary>
    /// Завершить диалог
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в Telegram.</param>
    /// <param name="clientId">Идентификатор пациента</param>
    /// <returns></returns>
    Task CompleteClientDialog(long userId, Guid clientId);
}