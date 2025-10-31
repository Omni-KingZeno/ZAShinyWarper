using System.Collections.Generic;

namespace ZAWarper
{
    public class ProgramConfig
    {
        public string IPAddress { get; set; } = "192.168.0.1";
        public string Webhook { get; set; } = "";
        public bool SendWebhook { get; set; } = false;
        public List<Vector3> Positions { get; set; } = [];
        public List<int> SpeciesIndices { get; set; } = [];
        public decimal SpawnCheckTime { get; set; } = 2000;
        public decimal CamMove { get; set; } = 16000;
        public decimal SaveFreq { get; set; } = 3;
        public decimal ScaleMin { get; set; } = 0;
        public decimal ScaleMax { get; set; } = 255;
        public int WhenShinyFound { get; set; } = 0;
        public int IVHP { get; set; } = 0;
        public int IVAtk { get; set; } = 0;
        public int IVDef { get; set; } = 0;
        public int IVSpA { get; set; } = 0;
        public int IVSpD { get; set; } = 0;
        public int IVSpe { get; set; } = 0;
    }
}