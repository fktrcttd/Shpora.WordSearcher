using Newtonsoft.Json;

namespace Shpora.WordSearcher
{
    public class FullStatistic : PointsStatistic
    {
        [JsonProperty("words")] public int Words { get; set; }
        [JsonProperty("moves")] public int Moves { get; set; }
    }
}