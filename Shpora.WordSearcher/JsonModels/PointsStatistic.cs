using Newtonsoft.Json;

namespace Shpora.WordSearcher
{
    public class PointsStatistic
    {
        [JsonProperty("points")] public int Points { get; set; }
    }
}