using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SignalGenerationProgram
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SignalGenerator generator;
        InputProcessing input;
        SignalProcessing sp;
        DataSaver saver;
        DataBase db;

        double amplitude, frequency, max, min, average;
        int pointsCount, zeroCrossings;

        List<SignalPoint> points;

        public MainWindow()
        {
            InitializeComponent();
            points_text_box.MaxLength = 5;
            string[] items = { "Синусоида", "Меандр", "Треугольный", "Пилообразный" };
            type_signal_combo_box.ItemsSource = items;
            type_signal_combo_box.SelectedIndex = 0;

            generator = new SignalGenerator();
            input = new InputProcessing();
            sp = new SignalProcessing();
        }

        private void generate_button_Click(object sender, RoutedEventArgs e)
        {
            if (amplitude_text_box.Text.Length > 0 && frequency_text_box.Text.Length > 0 && points_text_box.Text.Length > 0)
            {
                amplitude = input.ReadDouble(amplitude_text_box.Text, amplitude_text_box.Name);
                frequency = input.ReadDouble(frequency_text_box.Text, frequency_text_box.Name);
                pointsCount = input.ReadInt(points_text_box.Text, 100, 10000);
                string[] signal_type = { "sinus", "meander", "triangular", "sawtooth" };

                if (amplitude > 0 && frequency > 0 && pointsCount > 0)
                {
                        switch(type_signal_combo_box.SelectedIndex)
                        {
                            case 0:
                                points = generator.GenerateSine(amplitude, frequency, pointsCount);
                                points_data_grid.ItemsSource = points;
                                saver = new DataSaver(signal_type[0]);
                                db = new DataBase(signal_type[0]);
                            break;
                            case 1:
                                points = generator.GenerateMeander(amplitude, frequency, pointsCount);
                                points_data_grid.ItemsSource = points;
                                saver = new DataSaver(signal_type[1]);
                                db = new DataBase(signal_type[1]);
                            break;
                            case 2:
                                points = generator.GenerateTriangular(amplitude, frequency, pointsCount);
                                points_data_grid.ItemsSource = points;
                                saver = new DataSaver(signal_type[2]);
                                db = new DataBase(signal_type[2]);
                            break;
                            case 3:
                                points = generator.GenerateSawtooth(amplitude, frequency, pointsCount);
                                points_data_grid.ItemsSource = points;
                                saver = new DataSaver(signal_type[3]);
                                db = new DataBase(signal_type[3]);
                            break;
                        }

                        max = sp.MaxSignal(points);
                        min = sp.MinSignal(points);
                        average = sp.AverageSignal(points);
                        zeroCrossings = sp.NumberZeroLine(points);

                        max_text_box.Text = max.ToString();
                        min_text_box.Text = min.ToString();
                        average_text_box.Text = average.ToString();
                        zero_line_text_box.Text = zeroCrossings.ToString();
                }
            }
            else { MessageBox.Show("Заполните все параметры", "Внимание"); }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            if (points != null)
            {
                saver.Save(amplitude, frequency, pointsCount, points);
                MessageBox.Show("Данные успешно сохранены в папку проекта 'SaveData'", "Внимание");
            } 
            else
                MessageBox.Show("Сначала сгенерируйте сигнал", "Внимание");
        }

        private void save_db_button_Click(object sender, RoutedEventArgs e)
        {
            if (points != null)
            {
                db.SaveData(amplitude, frequency, pointsCount, points, max, min, average, zeroCrossings);
                MessageBox.Show("Данные сохранены в БД 'signals.db' папке 'DataBase' в папке проекта", "Внимание");
            }
            else
                MessageBox.Show("Сначала сгенерируйте сигнал", "Внимание");
        }

        private void graph_button_Click(object sender, RoutedEventArgs e)
        {
            if (points != null)
                new ChartWindow(type_signal_combo_box.Text, points).Show();
            else
                MessageBox.Show("Сначала сгенерируйте сигнал", "Внимание");
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            amplitude_text_box.Clear();
            amplitude = double.NaN;

            frequency_text_box.Clear();
            frequency = double.NaN;

            points_text_box.Clear();
            pointsCount = 0;

            max_text_box.Clear();
            min_text_box.Clear();
            average_text_box.Clear();
            zero_line_text_box.Clear();

            points_data_grid.ItemsSource = null;
            points = null;
        }
    }

    public class SignalPoint
    {
        public double Time { get; set; }
        public double Value { get; set; }
    }
}
