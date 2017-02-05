using System;

namespace Keebee.AAT.Display.Models
{
    public class LogInteractiveActivityEventArgs : EventArgs
    {
        public int InteractiveActivityTypeId { get; set; }
        public string Description { get; set; }
        public int? DifficultyLevel { get; set; }
        public bool? Success { get; set; }
    }
}
