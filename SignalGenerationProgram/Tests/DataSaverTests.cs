using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class DataSaverTests
    {
        private string saveDirectory;

        [SetUp]
        public void Setup()
        {
            string project = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            saveDirectory = Path.Combine(project, "SaveData");

            foreach (var file in Directory.GetFiles(saveDirectory))
                File.Delete(file);
        }
        //проверка создания файла и его содержимого
        [Test]
        public void Save_CreateFileWithContent()
        {
            string type = "sinus";
            double amplitude = 1;
            double frequency = 2;
            int pointsCount = 2;

            var points = new List<SignalPoint>
            {
                new SignalPoint { Time = 0.0, Value = 0.0 },
                new SignalPoint { Time = 0.5, Value = 1.0 }
            };

            var saver = new DataSaver(type);
            saver.Save(amplitude, frequency, pointsCount, points);

            
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string expectedFileName = type + "_A" + amplitude + "_F" + frequency + "_P" + pointsCount + "_" + currentDate + ".csv";

            string expectedPath = Path.Combine(saveDirectory, expectedFileName);

            Assert.That(File.Exists(expectedPath), Is.True, "Файл не был создан");

            var lines = File.ReadAllLines(expectedPath);

            Assert.That(lines[0], Is.EqualTo($"Signal Type, {type}"));
            Assert.That(lines[1], Is.EqualTo($"Amplitude, {amplitude.ToString(CultureInfo.InvariantCulture)}"));
            Assert.That(lines[2], Is.EqualTo($"Frequency, {frequency.ToString(CultureInfo.InvariantCulture)}"));
            Assert.That(lines[3], Is.EqualTo($"Points Count, {pointsCount}"));
            Assert.That(lines[4], Is.EqualTo($"Date, {currentDate}"));

            Assert.That(lines, Does.Contain("Time,Value"));
            Assert.That(lines, Does.Contain("0,0"));
            Assert.That(lines, Does.Contain("0.5,1"));
        }
        // проверка корректности названия файла
        [Test]
        public void Save_FileNameIsCorrect()
        {
            string type = "sinus";
            double amplitude = 1;
            double frequency = 3;
            int pointsCount = 3;

            var saver = new DataSaver(type);
            var points = new List<SignalPoint>
            {
                new SignalPoint { Time = 0, Value = 0 },
                new SignalPoint { Time = 1, Value = 1 }
            };
            saver.Save(amplitude, frequency, pointsCount, points);

            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            string expectedFileName = type + "_A" + amplitude + "_F" + frequency + "_P" + pointsCount + "_" + currentDate + ".csv";
            string expectedPath = Path.Combine(saveDirectory, expectedFileName);

            Assert.That(File.Exists(expectedPath), Is.True);
        }
        // проверка строк в файле
        [Test]
        public void Save_File_LineCountIsCorrect()
        {
            var saver = new DataSaver("sinus");

            var points = new List<SignalPoint>
            {
                new SignalPoint { Time = 0, Value = 0 },
                new SignalPoint { Time = 1, Value = 1 }
            };

            saver.Save(1, 1, 2, points);

            var file = Directory.GetFiles(saveDirectory).First();
            var lines = File.ReadAllLines(file);

            int expectedLines = 5 + 1 + 1 + points.Count;

            Assert.That(lines.Length, Is.EqualTo(expectedLines));
        }
        //проверка формата файла
        [Test]
        public void Save_File_HasCsvFormat()
        {
            var saver = new DataSaver("sinus");

            var points = new List<SignalPoint>
            {
                new SignalPoint { Time = 0, Value = 1 }
            };

            saver.Save(1, 1, 1, points);

            var file = Directory.GetFiles(saveDirectory).First();
            var lines = File.ReadAllLines(file);

            Assert.That(lines.Any(l => l.Contains(",")), Is.True, "Файл не в CSV формате");
        }
        // отрицательные значения для записи
        [Test]
        public void Save_NegativeArguments()
        {
            var saver = new DataSaver("sinus");
            var points = new List<SignalPoint>
            {
                new SignalPoint { Time = 0, Value = 0 },
                new SignalPoint { Time = 1, Value = 1 }
            };

            Assert.Throws<ArgumentException>(() =>
                saver.Save(-1, 1, 100, points));

            Assert.Throws<ArgumentException>(() =>
                saver.Save(1, -1, 100, points));

            Assert.Throws<ArgumentException>(() =>
                saver.Save(1, 1, -100, points));
        }
        // пустые значения точек
        [Test]
        public void Save_EmptyPoints()
        {
            var saver = new DataSaver("sinus");

            Assert.Throws<ArgumentException>(() =>
                saver.Save(1, 1, 100, new List<SignalPoint>()));
        }

    }
}
