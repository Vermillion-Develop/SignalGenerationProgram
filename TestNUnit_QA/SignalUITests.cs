using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using System.Diagnostics;
using System.IO;
namespace TestNUnit_QA
{
    [TestFixture]
    public class SignalUITests
    {
        private Application? _application;
        private UIA3Automation? _automation;
        private Window? _window;
        private MainPage? _mainPage;

        [SetUp]
        public void BeforeToTest()//Предустановки
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\"));
            string projectRoot = Path.Combine(solutionRoot, "SignalGenerationProgram");
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
        public void AfterToTest()//После теста
        {
            _automation?.Dispose();
            if (_application != null && !_application.HasExited)
            {
                _application.Close();
            }
        }
        //4 готовых тест кейса, для полного теста комбобокса с типами сигналов
        [TestCase("Синусоида", "5.0", "2.0", "500")]
        [TestCase("Меандр", "3.55", "17", "200")]
        [TestCase("Треугольный", "72.1", "10.5", "100")]  
        [TestCase("Пилообразный", "15.0", "20.0", "10000")]
        public void Test_01_GenerateSignal(string typeSignal, string amplitude, string freq, string points)//Тест с корректными значениями тест кейсов
        {
            string? combo = _mainPage.ComboBoxElement("type_signal_combo_box", typeSignal);
            Assert.That(combo, Is.Not.Null, "Выпадающий список с сигналами не найден");

            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", amplitude);
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", freq);
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", points);

            _mainPage.ClickButton("generate_button");

            TextBox? maxTextBox = _mainPage.TextBoxElement("max_text_box");
            TextBox? minTextBox = _mainPage.TextBoxElement("min_text_box");
            TextBox? avgTextBox = _mainPage.TextBoxElement("average_text_box");
            TextBox? peresechenieTextBox = _mainPage.TextBoxElement("zero_line_text_box");

            //Проверка пустоты в итоговых полях после расчета
            Assert.That(maxTextBox?.Text, Is.Not.Null.And.Not.Empty, "Поле максимума пустое");
            Assert.That(minTextBox?.Text, Is.Not.Null.And.Not.Empty, "Поле минимума пустое");
            Assert.That(avgTextBox?.Text, Is.Not.Null.And.Not.Empty, "Поле среднего пустое");
            Assert.That(peresechenieTextBox?.Text, Is.Not.Null.And.Not.Empty, "Поле пересечение пустое");
        }

        [TestCase("Синусоида", "", "", "")]
        [TestCase("Синусоида", "adw", "adw", "awd")]
        [TestCase("Меандр", "-15", "-1", "-150")]
        [TestCase("Синусоида", "0", "0", "0")]
        [TestCase("Меандр", "15,0", "20,0", "10,0")]
        public void Test_02_NegativeGenerateSignal(string typeSignal, string amplitude, string freq, string points)//Тест с некорректными значениями
        {
            string? combo = _mainPage.ComboBoxElement("type_signal_combo_box", typeSignal);
            Assert.That(combo, Is.Not.Null, "Выпадающий список с сигналами не найден");

            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", amplitude);
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", freq);
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", points);

            _mainPage.ClickButton("generate_button");

            TextBox? maxTextBox = _mainPage.TextBoxElement("max_text_box");
            TextBox? minTextBox = _mainPage.TextBoxElement("min_text_box");
            TextBox? avgTextBox = _mainPage.TextBoxElement("average_text_box");
            TextBox? peresechenieTextBox = _mainPage.TextBoxElement("zero_line_text_box");

            //Проверка пустоты в итоговых полях после расчета
            Assert.That(maxTextBox?.Text, Is.Null.Or.Empty, "Поле максимума пустое");
            Assert.That(minTextBox?.Text, Is.Null.Or.Empty, "Поле минимума пустое");
            Assert.That(avgTextBox?.Text, Is.Null.Or.Empty, "Поле среднего пустое");
            Assert.That(peresechenieTextBox?.Text, Is.Null.Or.Empty, "Поле пересечение пустое");
        }

        [TestCase("Синусоида", "5.0", "2.0", "500")]
        [TestCase("Меандр", "3.55", "17", "200")]
        [TestCase("Треугольный", "72.1", "10.5", "100")]
        [TestCase("Пилообразный", "15.0", "20.0", "3000")]
        public void Test_03_OpenChartAfterGenerateSignal(string typeSignal, string amplitude, string freq, string points)//Тест на открытие графика после генерации сигнала
        {
            string? combo = _mainPage.ComboBoxElement("type_signal_combo_box", typeSignal);

            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", amplitude);
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", freq);
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", points);

            _mainPage.ClickButton("generate_button");
            _mainPage.ClickButton("graph_button");

            Window chartWindow = _mainPage.GetAllWindows();

            Assert.That(chartWindow, Is.Not.Null, "Окно с графиком сигнала не открылось");
        }

        [Test]
        public void Test_04_ClearSignalsGeneration()//Тест на очистку данных с формы
        {
            string? combo = _mainPage.ComboBoxElement("type_signal_combo_box", "Синусоида");
            TextBox? amplitudeField = _mainPage.TextBoxElement("amplitude_text_box", "5.5");
            TextBox? freqField = _mainPage.TextBoxElement("frequency_text_box", "5.5");
            TextBox? pointsField = _mainPage.TextBoxElement("points_text_box", "150");

            _mainPage.ClickButton("generate_button");
            _mainPage.ClickButton("clear_button");

            Assert.That(amplitudeField.Text, Is.Empty.Or.WhiteSpace, "Поле не очистилось");
            Assert.That(freqField.Text, Is.Empty.Or.WhiteSpace, "Поле не очистилось");
            Assert.That(pointsField.Text, Is.Empty.Or.WhiteSpace, "Поле не очистилось");
        }

        [TestCase("graph_button")]
        [TestCase("save_db_button")]
        [TestCase("save_file_button")]
        public void Test_05_ClickOnButtonsBeforeGenerateSignal(string automatId) //Тесты нажатия кнопок перед генерацией сигнала
        {
            _mainPage.ClickButton(automatId);
            var msgBox = _mainPage?.CloseMessageBox();
            Assert.That(msgBox, Is.Not.Null, "Окно с графиком сигнала не открылось");
        }

    }
}
