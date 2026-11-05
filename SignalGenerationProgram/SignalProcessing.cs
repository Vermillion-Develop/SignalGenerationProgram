using System.Collections.Generic;
using System.Linq;

namespace SignalGenerationProgram
{
    internal class SignalProcessing
    {
        public int NumberZeroLine(List<SignalPoint> points)
        {
            int zeroCrossings = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i - 1].Value * points[i].Value < 0 || points[i].Value == 0)
                    zeroCrossings++;
            }
            return zeroCrossings;            
        }

        public double MaxSignal(List<SignalPoint> points)
        {
            return points.Max(p => p.Value);
        }

        public double MinSignal(List<SignalPoint> points)
        {
            return points.Min(p => p.Value);
        }

        public double AverageSignal(List<SignalPoint> points)
        {
            return points.Average(p => p.Value);
        }

    }
}
