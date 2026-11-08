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
    public class DatabaseTests
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
        public void Test_01_SaveDataToDataBase()//Тест сохранения файла в signals.db
        {
            string? comboType = _mainPage.ComboBoxElement("type_signal_combo_box", "Меандр");

            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", "5.5");
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", "5.5");
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", "150");

            _mainPage.ClickButton("generate_button");

            _mainPage.ClickButton("save_db_button");

            Assert.That(File.Exists(Path.Combine(projectRoot, "DataBase", "signals.db")), Is.True);
        }

        public class TestSignalPoint
        {
            public double Time { get; set; }
            public double Value { get; set; }
        }

        [Test]
        public void Test_02_GetAllSignalsFromDataBase()
        {
            string connect = $"Data Source={Path.Combine(projectRoot, "DataBase", "signals.db")};";
            List<TestSignalPoint> list = new List<TestSignalPoint>();
            using (var connection = new SqliteConnection(connect))
            {
                connection.Open();

                using (var command = new SqliteCommand("SELECT * FROM SignalPoints;", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pont = new TestSignalPoint()
                            {
                                Time = reader.GetDouble(0),
                                Value = reader.GetDouble(1)
                            };
                            list.Add(pont);
                        }
                    }
                }

                Assert.That(list.Count, Is.GreaterThan(0), "Таблица в базе данных осталась пустой после генерации!");
                TestContext.WriteLine($"Получено объектов: {list.Count}");
            }
        }
    }
}

