using Steamworks;

namespace LahStats.Models
{
    public class Stat
    {
        public CSteamID Id { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public float KDRatio => (Deaths > 0) ? ((float)Kills / (float)Deaths) : ((float)Kills);
    }
}
