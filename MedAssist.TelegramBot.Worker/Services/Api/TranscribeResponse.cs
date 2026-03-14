namespace MedAssist.TelegramBot.Worker.Services.Api
{
    public class TranscribeResponse
    {
        public string Text { get; set; }
        public Segment[] Segments { get; set; }
        public string Language { get; set; }
    }

    public class Segment
    {
        public int Id { get; set; }
        public int Seek { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public string Text { get; set; }
        public int[] Tokens { get; set; }
        public float Temperature { get; set; }
        public float Avg_logprob { get; set; }
        public float Compression_ratio { get; set; }
        public float No_speech_prob { get; set; }
    }

}