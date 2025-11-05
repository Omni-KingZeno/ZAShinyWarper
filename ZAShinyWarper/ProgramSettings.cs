namespace ZAShinyWarper
{
    public class ProgramConfig
    {
        public string IPAddress { get; set; } = "192.168.0.1";
        public List<WebhookData> Webhooks { get; set; } = [];
        public List<Vector3> Positions { get; set; } = [];
        public List<int> SpeciesIndices { get; set; } = [];
        public decimal SpawnCheckTime { get; set; } = 2000;
        public decimal CamMove { get; set; } = 16000;
        public decimal SaveFreq { get; set; } = 3;
        public decimal ScaleMin { get; set; } = 0;
        public decimal ScaleMax { get; set; } = 255;
        public int WhenShinyFound { get; set; } = 0;
        public int ForcedWeather { get; set; } = -1;
        public int ForcedTimeOfDay { get; set; } = 0;
        public bool IsAlpha { get; set; } = false;
        public int IVHP { get; set; } = 0;
        public int IVAtk { get; set; } = 0;
        public int IVDef { get; set; } = 0;
        public int IVSpA { get; set; } = 0;
        public int IVSpD { get; set; } = 0;
        public int IVSpe { get; set; } = 0;
    }

    public class WebhookData
    {
        public string WebhookAddress { get; set; } = string.Empty;
        public string MessageContents { get; set; } = string.Empty;
        public bool Enabled { get; set; } = false;
    }
}