using Refit;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public interface IAsrApiClient
{
    [Multipart]
    [Post("/asr?encode=true&task=transcribe&language=ru&output=json")]
    Task<ApiResponse<TranscribeResponse>> TranscribeAudio([AliasAs("audio_file")] StreamPart audioFile);
}