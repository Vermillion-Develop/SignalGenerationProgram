using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace SignalGenerationProgram
{
    internal class InputProcessing
    {
        public double ReadDouble(string input, string name_box)
        {
            string amplitude = "amplitude";
            string frequency = "frequency";
            if (input == "")
            {
                MessageBox.Show("Данные не введены", "Внимание");
                return -1;
            }
            else 
            {
                if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) || value <= 0)
                {
                    if (name_box.Contains(amplitude))
                    {
                        MessageBox.Show("Амплитуда записывается только числами.\nВвод дробных чисел осуществляется через точку.\nАмплитуда должна быть больше 0.", "Внимание");
                        return -1;
                    }
                    if (name_box.Contains(frequency))
                    {
                        MessageBox.Show("Частота записывается только числами.\nВвод дробных чисел осуществляется через точку.\nЧастота должна быть больше 0.", "Внимание");
                        return -1;
                    }
                }
                return value;
            }  
        }

        public int ReadInt(string input, int min, int max)
        {
            if (!int.TryParse(input, out int value))
                MessageBox.Show("Ввод количества точек задается целыми числами", "Внимание");
            else
                if (value < min || value > max)
                    MessageBox.Show("Количество точек должно быть в диапазоне от " + min + " до " + max, "Внимание");
                else
                    return value;
            return -1;
        }

        public void ValidateArguments(double amplitude, double frequency, int pointsCount)
        {
            if (pointsCount <= 0)
                throw new ArgumentException("pointsCount должен быть больше 0", nameof(pointsCount));

            if (frequency <= 0)
                throw new ArgumentException("frequency должен быть больше 0", nameof(frequency));

            if (amplitude <= 0)
                throw new ArgumentException("amplitude должен быть больше 0", nameof(amplitude));
        }

        public void ValidatePoints(List<SignalPoint> points)
        {
            if (points.Count == 0)
                throw new ArgumentException("points пуст", nameof(points));
        }
    }
}
