using System;
using System.Collections.Generic;

namespace SmartAdder.Models
{
    public class HistoryRecord
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public List<double> Entries { get; set; } = new List<double>();
        public double TotalSum { get; set; }
    }
}
