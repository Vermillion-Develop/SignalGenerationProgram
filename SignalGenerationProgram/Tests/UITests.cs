using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class UITests
    {
        private Application app;
        private UIA3Automation automation;
        private Window window;

        private ComboBox type_signal;
        private TextBox amplitude, frequency, points, max, min, avg, zeroCrossing;
        private Button generate_button, show_graph, save_in_file, save_in_db, clear;
        private DataGridView gridView; 

        [SetUp]
        public void Setup()
        {
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).FullName;
            string appPath = projectRoot + @"\Debug\SignalGenerationProgram.exe";

            app = Application.Launch(appPath);

            Thread.Sleep(1000);

            automation = new UIA3Automation();

            window = app.GetMainWindow(automation);

            InitializeUI();
        }

        [TearDown]
        public void TearDown()
        {
            app?.Close();
            automation?.Dispose();
            Thread.Sleep(1000);
        }

        private void InitializeUI()
        {
            type_signal = window.FindFirstDescendant(cf => cf.ByAutomationId("type_signal_combo_box")).AsComboBox();
            amplitude = window.FindFirstDescendant(cf => cf.ByAutomationId("amplitude_text_box")).AsTextBox();
            frequency = window.FindFirstDescendant(cf => cf.ByAutomationId("frequency_text_box")).AsTextBox();
            points = window.FindFirstDescendant(cf => cf.ByAutomationId("points_text_box")).AsTextBox();

            max = window.FindFirstDescendant(cf => cf.ByAutomationId("max_text_box")).AsTextBox();
            min = window.FindFirstDescendant(cf => cf.ByAutomationId("min_text_box")).AsTextBox();
            avg = window.FindFirstDescendant(cf => cf.ByAutomationId("average_text_box")).AsTextBox();
            zeroCrossing = window.FindFirstDescendant(cf => cf.ByAutomationId("zero_line_text_box")).AsTextBox();

            generate_button = window.FindFirstDescendant(cf => cf.ByAutomationId("generate_button")).AsButton();
            show_graph = window.FindFirstDescendant(cf => cf.ByAutomationId("graph_button")).AsButton();
            save_in_file = window.FindFirstDescendant(cf => cf.ByAutomationId("save_file_button")).AsButton();
            save_in_db = window.FindFirstDescendant(cf => cf.ByAutomationId("save_db_button")).AsButton();
            clear = window.FindFirstDescendant(cf => cf.ByAutomationId("clear_button")).AsButton();

            gridView = window.FindFirstDescendant(cf => cf.ByAutomationId("points_data_grid")).AsDataGridView();
        }

        private void Input (int _typeSignal, string _amplitude, string _frequency, string _points)
        {
            type_signal.Select(_typeSignal);
            amplitude.Enter(_amplitude);
            frequency.Enter(_frequency);
            points.Enter(_points);
        }

        private void Generate()
        {
            generate_button.Invoke();
            Thread.Sleep(1000);

            if (amplitude.Text == string.Empty || frequency.Text == string.Empty || points.Text == string.Empty)
            {
                var dialog = window.ModalWindows.FirstOrDefault();

                Assert.That(dialog, Is.Not.Null);

                var text = dialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));

                Assert.That(text, Is.Not.Null);
                Assert.That(text.Name, Does.Contain("Заполните все параметры"));

                var okButton = dialog.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));

                okButton.AsButton().Invoke();
            }
            else 
            {
                Assert.Multiple(() =>
                {
                    Assert.That(gridView.Rows.Length, Is.GreaterThan(0));
                    Assert.That(max.Text, !Is.Empty);
                    Assert.That(min.Text, !Is.Empty);
                    Assert.That(avg.Text, !Is.Empty);
                    Assert.That(zeroCrossing.Text, !Is.Empty);
                });
            }
        }

        private void ShowGraph()
        {
            show_graph.Invoke();
            Thread.Sleep(1000);

            if (amplitude.Text == string.Empty || frequency.Text == string.Empty || points.Text == string.Empty)
            {
                var dialog = window.ModalWindows.FirstOrDefault();

                Assert.That(dialog, Is.Not.Null);

                var text = dialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));

                Assert.That(text, Is.Not.Null);
                Assert.That(text.Name, Does.Contain("Сначала сгенерируйте сигнал"));

                var okButton = dialog.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));

                okButton.AsButton().Invoke();
            }
            else 
            {
                var graphWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault();

                Assert.That(graphWindow, Is.Not.Null);
                Assert.That(graphWindow.Title, Does.Contain("График сигналов"));

                var chart = graphWindow.FindFirstDescendant(cf => cf.ByAutomationId("chart"));
                Assert.That(chart, Is.Not.Null);

                graphWindow.Close();
                Thread.Sleep(1000);
            }               
        }

        private void SaveInFile()
        {
            save_in_file.Invoke();
            Thread.Sleep(1000);

            var dialog = window.ModalWindows.FirstOrDefault();

            Assert.That(dialog, Is.Not.Null);

            var text = dialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));

            Assert.That(text, Is.Not.Null);

            if (amplitude.Text == string.Empty || frequency.Text == string.Empty || points.Text == string.Empty)
                Assert.That(text.Name, Does.Contain("Сначала сгенерируйте сигнал"));
            else
                Assert.That(text.Name, Does.Contain("Данные успешно сохранены в папку проекта 'SaveData'"));

            var okButton = dialog.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));

            okButton.AsButton().Invoke();
        }

        private void SaveInDB()
        {
            save_in_db.Invoke();
            Thread.Sleep(1000);

            var dialog = window.ModalWindows.FirstOrDefault();

            Assert.That(dialog, Is.Not.Null);

            var text = dialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));

            Assert.That(text, Is.Not.Null);

            if (amplitude.Text == string.Empty || frequency.Text == string.Empty || points.Text == string.Empty)
                Assert.That(text.Name, Does.Contain("Сначала сгенерируйте сигнал"));
            else
                Assert.That(text.Name, Does.Contain("Данные сохранены в БД 'signals.db' папке 'DataBase' в папке проекта"));

            var okButton = dialog.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));

            okButton.AsButton().Invoke();
        }

        private void Clear()
        {
            clear.Invoke();
            Thread.Sleep(1000);

            Assert.Multiple(() =>
            {
                Assert.That(gridView.Rows.Length, Is.EqualTo(0));
                Assert.That(max.Text, Is.Empty);
                Assert.That(min.Text, Is.Empty);
                Assert.That(avg.Text, Is.Empty);
                Assert.That(zeroCrossing.Text, Is.Empty);
            });
        }

        // генерация сигналов
        [Test]
        public void GenerateSignals_UITests()
        {
            string amplitude = "1";
            string frequency = "1";
            string points = "100";

            Input(0, amplitude, frequency, points);
            Generate();
            ShowGraph();
            SaveInFile();
            SaveInDB();
            Clear();

            Input(1, amplitude, frequency, points);
            Generate();
            ShowGraph();
            SaveInFile();
            SaveInDB();
            Clear();

            Input(2, amplitude, frequency, points);
            Generate();
            ShowGraph();
            SaveInFile();
            SaveInDB();
            Clear();

            Input(3, amplitude, frequency, points);
            Generate();
            ShowGraph();
            SaveInFile();
            SaveInDB();
            Clear();
        }
        // Пустой ввод
        [Test]
        public void EmptyInput_UITests()
        {
            Input(0, "1", "1", "");
            Generate();

            Input(0, "1", "", "100");
            Generate();

            Input(0, "", "1", "100");
            Generate();

            Clear();

            Generate();
            ShowGraph();
            SaveInFile();
            SaveInDB();
        }
        // Неверный ввод
        [Test]
        public void IncorrectInput_UITests()
        {
            //amlitude
            Input(0, "abs", "1", "100");            
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "-1", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "0", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "2,1", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "2. 1", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "2 .1", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "2 . 1", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");
            Input(0, "*", "1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Амплитуда");

            //frequency
            Input(0, "1", "abs", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "-1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "0", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "2,1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "2. 1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "2 .1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "2 . 1", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");
            Input(0, "1", "*", "100");
            ModalWindow_FindAndClose_IncorrectInput("Частота");

            //points
            Input(0, "1", "1", "abs");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "-100");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "0");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "2,1");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "2. 1");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "2 .1");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "2 . 1");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "*");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "10");
            ModalWindow_FindAndClose_IncorrectInput("точек");
            Input(0, "1", "1", "99999");
            ModalWindow_FindAndClose_IncorrectInput("точек");
        }
        public void ModalWindow_FindAndClose_IncorrectInput(string param)
        {
            generate_button.Invoke();
            Thread.Sleep(1000);

            var dialog = window.ModalWindows.FirstOrDefault();
            Assert.That(dialog, Is.Not.Null);
            var text = dialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Name, Does.Contain(param));

            var okButton = dialog.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));

            okButton.AsButton().Invoke();
        }
    }
}
