using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homepage2
{
    public partial class Event
    {
        public int EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public string userID { get; set; }
        public int Freq { get; set; }
        public string RecId { get; set; }
        public int? Reminder { get; set; }
        public string JobID { get; set; }
    }
}

