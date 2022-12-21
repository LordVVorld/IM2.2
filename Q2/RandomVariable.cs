using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Q2
{
    public class RandomVariable : List<double>
    {
        private readonly Random rand = new Random();
        public double MathExpectation { get; private set; }
        public double Dispersion { get; private set; }
        public double Sigma { get; private set; }

        public RandomVariable(int valuesCount, double lambda)
        {
            for (int index = 0; index < valuesCount; ++index)
            {
                Thread.Sleep(1);
                Add(Math.Round(-1 * Math.Log(new Random().NextDouble()) / lambda, 2));
            }

            Sort();

            MathExpectation = Math.Round(this.Sum() / valuesCount, 2);

            Dispersion = Math.Round(this
                .Select(value => Math.Pow(value - MathExpectation, 2))
                .Sum() / (valuesCount - 1), 2);

            Sigma = Math.Sqrt(Dispersion);
        }
    }
}
