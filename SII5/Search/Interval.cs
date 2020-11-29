using System;
using System.Collections.Generic;
using System.Text;

namespace SII5.Search
{
    public class Interval
    {
        public double From { get; }
        
        public double To { get; }

        public Interval(double from, double to)
        {
            if (from != 0 && to != 0 && from > to)
            {
                throw new ArgumentException(
                    "Верхняя граница интервала может быть только больше нижней либо равна 0.");
            }

            From = from;
            To = to;
        }

        public bool IsInInterval(double value)
        {
            return (value >= From && ((To > 0) ? (value <= To) : true));
        }
    }
}
