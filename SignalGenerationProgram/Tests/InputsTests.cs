using NUnit.Framework;
using System.Linq;

namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class InputsTests
    {
        private InputProcessing input;
        int min, max;
        string amplitude_box, frequency_box;


        [SetUp]
        public void Setup()
        {
            input = new InputProcessing();
            amplitude_box = "amplitude";
            frequency_box = "frequency";
            max = 10000;
            min = 100;
        }

        private void Test_ReadDouble_Amplitude_Box(string input_string, double valid_result)
        {
            double result_ampitude_box = input.ReadDouble(input_string, amplitude_box);
            Assert.That(result_ampitude_box, Is.EqualTo(valid_result));
        }

        private void Test_ReadDouble_Frequency_Box(string input_string, double valid_result)
        {
            double result_frequency_box = input.ReadDouble(input_string, frequency_box);
            Assert.That(result_frequency_box, Is.EqualTo(valid_result));
        }

        private void Test_ReadInt(string input_string, double valid_result)
        {
            double result = input.ReadInt(input_string, min, max);
            Assert.That(result, Is.EqualTo(valid_result));
        }

        // ReadDouble
        // Верное число
        [Test]
        public void ReadDouble_ValidNumber()
        {
            string input_string = "2.5";
            double valid_result = 2.5;

            Test_ReadDouble_Amplitude_Box(input_string, valid_result);
            Test_ReadDouble_Frequency_Box(input_string, valid_result);
        }
        // нулевое значение
        [Test]
        public void ReadDouble_ZeroNumber()
        {
            string input_string = "0";
            double valid_result = -1;

            Test_ReadDouble_Amplitude_Box(input_string, valid_result);
            Test_ReadDouble_Frequency_Box(input_string, valid_result);
        }
        // отрицательное число
        [Test]
        public void ReadDouble_NegativeNumber()
        {
            string input_string = "-100";
            double valid_result = -1;

            Test_ReadDouble_Amplitude_Box(input_string, valid_result);
            Test_ReadDouble_Frequency_Box(input_string, valid_result);
        }
        // строка
        [Test]
        public void ReadDouble_InputString()
        {
            string input_string = "abcd";
            double valid_result = -1;
            
            Test_ReadDouble_Amplitude_Box(input_string, valid_result);
            Test_ReadDouble_Frequency_Box(input_string, valid_result);
        }
        // неправильный формат записи числа
        [Test]
        public void ReadDouble_InvavidNumberType()
        {
            double valid_result = -1;

            string[] inputs = { "12 . 5", "12. 5", "12 .5", "12,5", "12, 5", "12 ,5", "12 , 5", "12 5"};

            foreach (string input_string in inputs)
            {
                Test_ReadDouble_Amplitude_Box(input_string, valid_result);
                Test_ReadDouble_Frequency_Box(input_string, valid_result);
            }
        }
        // пустое значение
        [Test]
        public void ReadDouble_Empty()
        {
            string input_string = string.Empty;
            double valid_result = -1;

            Test_ReadDouble_Amplitude_Box(input_string, valid_result);
            Test_ReadDouble_Frequency_Box(input_string, valid_result);
        }
        // спецсимволы
        [Test]
        public void ReadDouble_Simbols()
        {
            double valid_result = -1;

            string[] punctuation = { ".", ",", ";", ":", "?", "!" };
            string[] brackets = { "(", ")", "[", "]", "{", "}" };
            string[] mathSymbols = { "+", "-", "*", "/", "=", "<", ">" };
            string[] specialSymbols = { "@", "#", "$", "%", "^", "&" };


            string[] inputs = punctuation.Concat(brackets).Concat(mathSymbols).Concat(specialSymbols).ToArray();

            foreach (string input_string in inputs)
            {
                Test_ReadDouble_Amplitude_Box(input_string, valid_result);
                Test_ReadDouble_Frequency_Box(input_string, valid_result);
            }
        }

        // ReadInt
        // Верное число
        [Test]
        public void ReadInt_ValidNumber()
        {
            string input_string = "100";
            double valid_result = 100;

            double result = input.ReadInt(input_string, min, max);
            Assert.That(result, Is.EqualTo(valid_result));
        }
        // Числа за пределами диапазона
        [Test]
        public void ReadInt_OutsideNumber()
        {
            string min_string = "10";
            string max_string = "100000";
            double valid_result = -1;

            Test_ReadInt(min_string, valid_result);
            Test_ReadInt(max_string, valid_result);
        }
        //Отрицательное число
        [Test]
        public void ReadInt_NegativeNumber()
        {
            string input_string = "-100";
            double valid_result = -1;

            Test_ReadInt(input_string, valid_result);
        }
        //Нулевое число
        [Test]
        public void ReadInt_ZeroNumber()
        {
            string input_string = "0";
            double valid_result = -1;

            Test_ReadInt(input_string, valid_result);
        }
        // Строка
        [Test]
        public void ReadInt_InvalidString()
        {
            string input_string = "abcd";
            double valid_result = -1;

            Test_ReadInt(input_string, valid_result);
        }
        // неправильный формат записи числа
        [Test]
        public void ReadInt_InvavidNumber()
        {
            double valid_result = -1;

            string[] inputs = { "12 . 5", "12. 5", "12 .5", "12,5", "12, 5", "12 ,5", "12 , 5", "12 5" };

            foreach (string input_string in inputs)
            {
                Test_ReadInt(input_string, valid_result);
            }
        }
        // спецсимволы
        [Test]
        public void ReadInt_Simbols()
        {
            double valid_result = -1;

            string[] punctuation = { ".", ",", ";", ":", "?", "!" };
            string[] brackets = { "(", ")", "[", "]", "{", "}" };
            string[] mathSymbols = { "+", "-", "*", "/", "=", "<", ">" };
            string[] specialSymbols = { "@", "#", "$", "%", "^", "&" };


            string[] inputs = punctuation.Concat(brackets).Concat(mathSymbols).Concat(specialSymbols).ToArray();

            foreach (string input_string in inputs)
            {
                Test_ReadInt(input_string, valid_result);
            }
        }
        // пустое значение
        [Test]
        public void ReadInt_Empty()
        {
            string input_string = string.Empty;
            double valid_result = -1;

            Test_ReadInt(input_string, valid_result);
        }
    }
}
