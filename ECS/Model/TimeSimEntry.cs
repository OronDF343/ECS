using System;

namespace ECS.Model
{
    public class TimeSimEntry
    {
        public TimeSpan Time { get; set; }
        public Switch Switch { get; set; }
        public bool IsClosed { get; set; }
        public bool WasProcessed { get; set; }
    }
}
