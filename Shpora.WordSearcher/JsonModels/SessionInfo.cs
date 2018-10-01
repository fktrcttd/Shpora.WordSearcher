using System;

namespace Shpora.WordSearcher
{
    public class SessionInfo
    {
        public TimeSpan Expires { get; set; }
        public DateTime Created { get; set; }
    }
}