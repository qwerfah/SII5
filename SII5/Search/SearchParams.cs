using SII5.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SII5.Search
{
    public class SearchParams
    {
        public Tree Tree { get; set; }

        public Interval SpeedInterval { get; set; }
        public Interval CapacityInterval { get; set; }
        public Interval YearInterval { get; set; }
        public Interval CostInterval { get; set; }

        public bool? ApplicationType { get; set; }
        public MemoryType? MemoryType { get; set; }

        public int Count { get; set; }
    }
}
