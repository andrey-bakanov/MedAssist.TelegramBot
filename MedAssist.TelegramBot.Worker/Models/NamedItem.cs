namespace MedAssist.TelegramBot.Worker.Models;

/// <summary>
/// Представляет элемент с идентификатором и названием.
/// </summary>
public record NamedItem
{
    /// <summary>
    /// Уникальный идентификатор элемента.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Название элемента.
    /// </summary>
    public required string Name { get; init; }
}