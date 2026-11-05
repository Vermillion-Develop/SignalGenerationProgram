using System;
using System.Collections.Generic;

namespace SignalGenerationProgram
{
    internal class SignalGenerator
    {
        InputProcessing inputProcessing;

        public SignalGenerator()
        {
            inputProcessing = new InputProcessing();
        }

        public List<SignalPoint> GenerateSine(double amplitude, double frequency, int pointsCount)
        {
            inputProcessing.ValidateArguments(amplitude, frequency, pointsCount);

            var result = new List<SignalPoint>();

            double duration = 1.0;
            double step = duration / pointsCount;

            for (int i = 0; i <= pointsCount; i++)
            {
                double t = i * step;
                double y = amplitude * Math.Sin(2 * Math.PI * frequency * t);

                result.Add(new SignalPoint
                {
                    Time = t,
                    Value = y
                });
            }
            return result;
        }

        public List<SignalPoint> GenerateMeander(double amplitude, double frequency, int pointsCount)
        {
            inputProcessing.ValidateArguments(amplitude, frequency, pointsCount);
            var result = new List<SignalPoint>();

            double duration = 1.0;
            double step = duration / pointsCount;

            for (int i = 1; i <= pointsCount; i++)
            {
                double t = i * step;
                double sineValue = Math.Sin(2 * Math.PI * frequency * t);

                double y = amplitude * Math.Sign(sineValue);

                result.Add(new SignalPoint
                {
                    Time = t,
                    Value = y
                });
            }

            return result;
        }

        public List<SignalPoint> GenerateTriangular(double amplitude, double frequency, int pointsCount)
        {
            inputProcessing.ValidateArguments(amplitude, frequency, pointsCount);
            var result = new List<SignalPoint>();

            double duration = 1.0;
            double step = duration / pointsCount;
            double period = 1.0 / frequency;

            for (int i = 0; i <= pointsCount; i++)
            {
                double t = i * step;

                double y = 4 * amplitude * Math.Abs(t / period - Math.Floor(t / period + 0.5)) - amplitude;

                result.Add(new SignalPoint
                {
                    Time = t,
                    Value = y
                });
            }

            return result;
        }

        public List<SignalPoint> GenerateSawtooth(double amplitude, double frequency, int pointsCount)
        {
            inputProcessing.ValidateArguments(amplitude, frequency, pointsCount);
            var result = new List<SignalPoint>();

            double duration = 1.0;
            double step = duration / pointsCount;
            double period = 1.0 / frequency;

            for (int i = 0; i <= pointsCount; i++)
            {
                double t = i * step;

                double normalized = t / period;
                double y = 2 * amplitude * (normalized - Math.Floor(normalized)) - amplitude;

                result.Add(new SignalPoint
                {
                    Time = t,
                    Value = y
                });
            }

            return result;
        }
    }
}
