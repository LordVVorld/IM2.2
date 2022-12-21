using System;
using System.Collections.Generic;
using System.Linq;

namespace Q2
{
    internal class Program
    {
        private const int selectionSet = 100;
        private const double lambda = 0.1;
        private const double TheorMathExpectation = 1 / lambda;
        private const double TheorDispersion = 1 / (lambda * lambda);
        private static readonly Dictionary<int, double> PearsonTable = new Dictionary<int, double>{
            { 1, 3.8 }, { 2, 6.0 }, { 3, 7.8 }, { 4, 9.5 },
            { 5, 11.1 }, { 6, 12.6}, {7, 14.1}, {8 , 15.5}
        };

        static void Main(string[] args)
        {
            //Генерация случайной величины и разбиение её значений по интервалам Стрёрджерса
            RandomVariable variable = new RandomVariable(selectionSet, lambda);
            SturgessIntervals intervals = new SturgessIntervals(variable);

            //Нахождение выборочного среднего 
            double xMiddle = (double)intervals
                .Select(interval => interval.Middle * interval.Count).Sum()
                / intervals.Select(interval => interval.Count).Sum();

            //Выделение списка частот и нахождение списка теоретических частот
            List<double> frequencies = intervals.Select(interval => (double)interval.Count).ToList();
            List<double> theorFrequencies = GetTheorFrequancies(intervals, xMiddle);

            //Блок выводов дисперсии и мат.ожидания
            Console.WriteLine($"Теор. знач. мат.ожидания: {TheorMathExpectation}");
            Console.WriteLine($"Теор. знач. дисперсии: {TheorDispersion}");
            Console.WriteLine($"Оценка мат.ожидания: {variable.MathExpectation}");
            Console.WriteLine($"Оценка дисперсии: {variable.Dispersion}");
            Console.WriteLine($"Ошибка мат.ожидания: {Math.Round(TheorMathExpectation - variable.MathExpectation, 2)}");
            Console.WriteLine($"Ошибка дисперсии: {Math.Round(TheorDispersion - variable.Dispersion, 2)}\n");

            //Вывод интервалов и частот
            Console.WriteLine($"С шагом {intervals.Step} образовано {intervals.Count} интервалов:");
            intervals.ForEach(interval =>
            {
                Console.WriteLine($"{interval.Index + 1}) [{interval.LowerBorder}-{interval.UpperBorder}], " +
                    $"n = {interval.Count}, n' = {theorFrequencies[interval.Index]}");
            });
            Console.WriteLine();

            //Объединение малочисленных частот (в т.ч. теоретических)
            for (int index = intervals.Count - 1; index > 0; --index)
            {
                if (frequencies[index] < 5)
                {
                    frequencies[index - 1] += frequencies[index];
                    frequencies.Remove(frequencies[index]);
                    theorFrequencies[index - 1] += theorFrequencies[index];
                    theorFrequencies.Remove(theorFrequencies[index]);
                }
            }

            //Вывод частот объединённых интервалов
            Console.WriteLine($"В результате объединения малочисленных частот образовано {frequencies.Count} интервалов:");
            for (int index = 0; index < frequencies.Count; index++)
            {
                Console.WriteLine($"{index + 1}) n = {frequencies[index]}, n' = {theorFrequencies[index]}");
            }
            Console.WriteLine();

            //Нахождение наблюдаемого значения критерия Пирсона
            double Hi2Observed = 0;
            for (int index = 0; index < frequencies.Count; index++)
            {
                Hi2Observed += Math.Pow(frequencies[index] - theorFrequencies[index], 2) / theorFrequencies[index];
            }
            Hi2Observed = Math.Round(Hi2Observed, 3);
            int degreesOfFreedom = frequencies.Count - 2;
            double Hi2Crit = PearsonTable[frequencies.Count - 2];

            //Вывод итогов
            Console.WriteLine($"При числе степеней свободы {degreesOfFreedom} и уровне значимости 0,05:\n" +
                $"X2набл = {Hi2Observed}, X2крит = {Hi2Crit}\n" +
                $"Следовательно гипотеза {((Hi2Observed < Hi2Crit) ? "принимается" : "отвергается")}" );
        }

        private static List<double> GetTheorFrequancies(SturgessIntervals intervals, double xMiddle)
        {
            List<double> result = new List<double>();
            for (int index = 0; index < intervals.Count; index++)
            {
                result.Add(new double());
                if (intervals[index].Count != 0)
                {
                    result[index] = CalculateFrequancy(
                            xMiddle,
                            intervals[index].LowerBorder,
                            intervals[index].UpperBorder
                        );
                }
            }
            return result;
        }

        private static double CalculateFrequancy(double xmiddle, double lowerBorder, double upperBorder)
        {
            double Pi = Math.Exp(-1 * lowerBorder / xmiddle) - Math.Exp(-1 * upperBorder / xmiddle);
            return Math.Round(selectionSet * Pi, 2);
        }
    }
}