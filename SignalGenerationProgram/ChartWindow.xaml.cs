using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows;

namespace SignalGenerationProgram
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartWindow(string name, List<SignalPoint> points)
        {
            InitializeComponent();
            BuildChart(name, points);
        }

        private void BuildChart(string name, List<SignalPoint> points)
        {
            var values = new ChartValues<ObservablePoint>();

            foreach (var point in points)
            {
                values.Add(new ObservablePoint(point.Time, point.Value));
            }

            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = name,
                    Values = values,
                    PointGeometry = null
                }
            };

            chart.AxisX.Add(new Axis { Title = "Время (с)" });
            chart.AxisY.Add(new Axis { Title = "Амплитуда" });
        }
    }
}
