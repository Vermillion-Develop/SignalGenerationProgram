using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;
using SignalGenerationProgram;
using System.Data.Common;

namespace TestNUnit_QA
{
    [TestFixture]
    public class DataSaveTests
    {
        private Application? _application;
        private UIA3Automation? _automation;
        private Window? _window;
        private MainPage? _mainPage;

        private string projectRoot;

        [SetUp]
        public void BeforeToTest()
        {

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\"));
            projectRoot = Path.Combine(solutionRoot, "SignalGenerationProgram");
            string debugFolder = Path.Combine(projectRoot, "bin", "Debug");
            string appPath = Path.Combine(debugFolder, "SignalGenerationProgram.exe");

            //Создание папок с бд и для сохранения .csv файла
            if (!Directory.Exists(Path.Combine(debugFolder, "DataBase")))
            {
                Directory.CreateDirectory(Path.Combine(debugFolder, "DataBase"));
            }

            if (!Directory.Exists(Path.Combine(debugFolder, "SaveData")))
            {
                Directory.CreateDirectory(Path.Combine(debugFolder, "SaveData"));
            }

            ProcessStartInfo startInfo = new()
            {
                FileName = appPath,
                WorkingDirectory = debugFolder
            };

            _application = Application.Launch(startInfo);
            _automation = new UIA3Automation();
            _window = _application.GetMainWindow(_automation);
            _window.WaitUntilClickable(TimeSpan.FromSeconds(5));
            _mainPage = new MainPage(_window, _application, _automation);
        }

        [TearDown]
        public void AfterToTest()
        {
            _automation?.Dispose();
            if (_application != null && !_application.HasExited)
            {
                _application.Close();
            }
        }

        [Test]
        public void Test_01_SaveFileToSaveData()//Тест сохранения в файл .csv
        {
            //Поля для файла .csv
            string? amplitude = "";
            string? freq = "";
            string? point = "";

            //Поиск по automationid
            string? comboType = _mainPage.ComboBoxElement("type_signal_combo_box", "Меандр");
            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", "5.5");
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", "5.5");
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", "150");
            DataGridView? datagrd = _mainPage.DataGridElement("points_data_grid");

            _mainPage.ClickButton("generate_button");

            var rows = datagrd.Rows;
            Assert.That(rows.Length, Is.GreaterThan(0));

            //Взятие первых 2 столбцов с датагрид
            var firstRow = rows[0];
            var firstColumnTime = firstRow.Cells[0].Value;
            var firstColumnZnach = firstRow.Cells[1].Value;

            var secondRow = rows[1];
            var secondColumnTime = secondRow.Cells[0].Value;
            var secondColumnZnach = secondRow.Cells[1].Value;

            Thread.Sleep(300);

            _mainPage.ClickButton("save_file_button");

            amplitude = amplitudeField?.Text;
            freq = freqField?.Text;
            point = pointsField?.Text;

            amplitude = amplitude?.Replace('.', ',');
            freq = freq?.Replace('.', ',');
            point = point?.Replace('.', ',');

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string expectedFileName = comboType + "_A" + amplitude + "_F" + freq + "_P" + point + "_" + currentDate + ".csv";

            string expectedPath = Path.Combine(projectRoot, "SaveData", expectedFileName);
            Assert.That(File.Exists(expectedPath), Is.True);

            string[] fileLines = File.ReadAllLines(expectedPath);

            amplitude = amplitude?.Replace(',', '.');
            freq = freq?.Replace(',', '.');
            point = point?.Replace(',', '.');

            Assert.That(fileLines[0], Is.EqualTo($"Signal Type, {comboType}"));
            Assert.That(fileLines[1], Is.EqualTo($"Amplitude, {amplitude.ToString(CultureInfo.InvariantCulture)}"));
            Assert.That(fileLines[2], Is.EqualTo($"Frequency, {freq.ToString(CultureInfo.InvariantCulture)}"));
            Assert.That(fileLines[3], Is.EqualTo($"Points Count, {point}"));
            Assert.That(fileLines[4], Is.EqualTo($"Date, {currentDate}"));

            Assert.That(fileLines, Does.Contain("Time,Value"));

            Assert.That(fileLines, Does.Contain($"{firstColumnTime},{firstColumnZnach}"));
            Assert.That(fileLines, Does.Contain($"{secondColumnTime},{secondColumnZnach}"));
        }
    }
}

