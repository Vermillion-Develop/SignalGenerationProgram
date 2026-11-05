using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class DataBaseTests
    {
        private DataBase db;
        private string connectionString;

        [SetUp]
        public void Setup()
        {
            db = new DataBase("test");

            string filename = "signals.db";
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string filePath = Path.Combine(projectRoot, "DataBase", filename);
            connectionString = "Data Source=" + filePath;

            var signals = db.GetAllSignals();
            foreach (var s in signals)
            {
                db.DeleteSignal(s.Id);
            }
        }

        private List<SignalPoint> GetTestPoints()
        {
            return new List<SignalPoint>
            {
                new SignalPoint { Time = 0, Value = 0 },
                new SignalPoint { Time = 1, Value = 1 }
            };
        }

        [Test]
        public void SaveData_CreatesRecord()
        {
            db.SaveData(1, 2, 2, GetTestPoints(), 1, 1, 0.5, 1);

            var signals = db.GetAllSignals();

            Assert.That(signals.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAllSignals_ReturnsData()
        {
            db.SaveData(1, 2, 2, GetTestPoints(), 1, 1, 0.5, 1);

            var signals = db.GetAllSignals();

            Assert.That(signals.Any(), Is.True);
        }

        [Test]
        public void UpdateAmplitude_ChangesValue()
        {
            db.SaveData(1, 2, 2, GetTestPoints(), 1, 1, 0.5, 1);

            var signal = db.GetAllSignals().First();

            db.UpdateAmplitude(signal.Id, 99);

            using (var connection = new System.Data.SQLite.SQLiteConnection(connectionString))
            {
                connection.Open();
                var cmd = new System.Data.SQLite.SQLiteCommand(
                    "SELECT Amplitude FROM Signals WHERE Id = @id", connection);

                cmd.Parameters.AddWithValue("@id", signal.Id);

                double value = (double)cmd.ExecuteScalar();

                Assert.That(value, Is.EqualTo(99));
            }
        }

        [Test]
        public void DeleteSignal_RemovesRecord()
        {
            db.SaveData(1, 2, 2, GetTestPoints(), 1, 1, 0.5, 1);

            var signal = db.GetAllSignals().First();

            db.DeleteSignal(signal.Id);

            var signals = db.GetAllSignals();

            Assert.That(signals.Count, Is.EqualTo(0));
        }
    }
}
