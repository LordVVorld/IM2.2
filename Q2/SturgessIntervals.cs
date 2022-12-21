using System;
using System.Collections.Generic;
using System.Linq;

namespace Q2
{
    public class SturgessIntervals : List<Interval>
    {
        public double Step { get; private set; }

        public SturgessIntervals(List<double> values)
        {
            int intervalsCount = (int)Math.Ceiling(1 + 3.3221 * Math.Log10(values.Count));

            Step = Math.Round((values.Max() - values.Min()) / intervalsCount, 3);

            DefineIntervals(values, intervalsCount);
        }

        private void DefineIntervals(List<double> values, int inrevalsCount)
        {
            double lowerBorder = values.Min() - 0.001;
            double upperBorder = values.Min() + Step;
            for (int intervalIndex = 0; intervalIndex < inrevalsCount; ++intervalIndex)
            {
                Add(
                    new Interval(
                        values.Where(value => lowerBorder < value && value <= upperBorder).Count(),
                        lowerBorder,
                        upperBorder,
                        intervalIndex)
                    );
                lowerBorder = upperBorder;
                upperBorder += Step;
            }
        }
    }

    public class Interval
    {
        public int Count { get; private set; }
        public double LowerBorder { get; private set; }
        public double UpperBorder { get; private set; }
        public double Middle { get; private set; }
        public int Index { get; private set; }
        
        public Interval(int count, double lowerBorder, double upperBorder, int index)
        {
            Count = count;
            LowerBorder = lowerBorder;
            UpperBorder = upperBorder;
            Middle = (lowerBorder + upperBorder) / 2;
            Index = index;
        }
    }
}
