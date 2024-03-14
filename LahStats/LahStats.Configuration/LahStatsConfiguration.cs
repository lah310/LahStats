using Rocket.API;

namespace LahStats.Configuration
{
    public class LahStatsConfiguration : IRocketPluginConfiguration, IDefaultable
    {
        public string Image { get; set; }
        public ushort UIEffectID { get; set; }
        public int UpdateInterval { get; set; }

        public void LoadDefaults()
        {
            Image = "url here";
            UIEffectID = 11011;
            UpdateInterval = 300;
        }
    }
}
