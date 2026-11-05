using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace SignalGenerationProgram
{
    internal class DataSaver
    {
        string typeSignal;
        InputProcessing inputProcessing;
        public DataSaver(string typeSignal) 
        {
            this.typeSignal = typeSignal;
            inputProcessing = new InputProcessing();
        }

        public void Save(double amplitude, double frequency, int pointsCount, List<SignalPoint> points)
        {
            inputProcessing.ValidateArguments(amplitude, frequency, pointsCount);
            inputProcessing.ValidatePoints(points);

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string filename = typeSignal + "_A" + amplitude + "_F" + frequency + "_P" + pointsCount + "_" + currentDate + ".csv";

            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string filePath = Path.Combine(projectRoot, "SaveData", filename);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Signal Type, {typeSignal}");
                writer.WriteLine($"Amplitude, {amplitude.ToString(CultureInfo.InvariantCulture)}");
                writer.WriteLine($"Frequency, {frequency.ToString(CultureInfo.InvariantCulture)}");
                writer.WriteLine($"Points Count, {pointsCount}");
                writer.WriteLine($"Date, {currentDate}");
                writer.WriteLine("----,----");
                writer.WriteLine("Time,Value");

                foreach (var point in points)
                {
                    writer.WriteLine($"{point.Time.ToString(CultureInfo.InvariantCulture)}," +
                                     $"{point.Value.ToString(CultureInfo.InvariantCulture)}");
                }
            }
        }
    }
}
