using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace SignalGenerationProgram
{
    internal class DataBase
    {
        private string typeSignal;
        private string connectionString;

        public DataBase(string typeSignal)
        {
            this.typeSignal = typeSignal;
            
            string filename = "signals.db";
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string filePath = Path.Combine(projectRoot, "DataBase", filename);
            connectionString = "Data Source=" + filePath;

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createSignalsTable = @"
                    CREATE TABLE IF NOT EXISTS Signals (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SignalType TEXT,
                        Amplitude REAL,
                        Frequency REAL,
                        PointsCount INTEGER,
                        CurrentDate TEXT,
                        Max REAL,
                        Min REAL,
                        Average REAL,
                        ZeroCrossings INTEGER
                    );";

                string createPointsTable = @"
                    CREATE TABLE IF NOT EXISTS SignalPoints (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SignalId INTEGER,
                        X REAL,
                        Y REAL,
                        FOREIGN KEY (SignalId) REFERENCES Signals(Id)
                    );";

                new SQLiteCommand(createSignalsTable, connection).ExecuteNonQuery();
                new SQLiteCommand(createPointsTable, connection).ExecuteNonQuery();
            }
        }

        public void SaveData(double amplitude, double frequency, int pointsCount, List<SignalPoint> points, double max, double min, double average,int zeroCrossings)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string date = DateTime.Now.ToString("yyyyMMdd");

                string insertSignal = @"
                    INSERT INTO Signals 
                    (SignalType, Amplitude, Frequency, PointsCount, CurrentDate, Max, Min, Average, ZeroCrossings)
                    VALUES 
                    (@type, @amp, @freq, @count, @date, @max, @min, @avg, @zero);
            
                    SELECT last_insert_rowid();";

                var command = new SQLiteCommand(insertSignal, connection);

                command.Parameters.AddWithValue("@type", typeSignal);
                command.Parameters.AddWithValue("@amp", amplitude);
                command.Parameters.AddWithValue("@freq", frequency);
                command.Parameters.AddWithValue("@count", pointsCount);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@max", max);
                command.Parameters.AddWithValue("@min", min);
                command.Parameters.AddWithValue("@avg", average);
                command.Parameters.AddWithValue("@zero", zeroCrossings);

                long signalId = (long)command.ExecuteScalar();

                foreach (var p in points)
                {
                    string insertPoint = @"
                        INSERT INTO SignalPoints (SignalId, X, Y)
                        VALUES (@id, @x, @y)";

                    var pointCmd = new SQLiteCommand(insertPoint, connection);
                    pointCmd.Parameters.AddWithValue("@id", signalId);
                    pointCmd.Parameters.AddWithValue("@x", p.Time);
                    pointCmd.Parameters.AddWithValue("@y", p.Value);

                    pointCmd.ExecuteNonQuery();
                }
            }
        }

        public List<(long Id, string Type)> GetAllSignals()
        {
            var result = new List<(long, string)>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Id, SignalType FROM Signals";

                var cmd = new SQLiteCommand(query, connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add((reader.GetInt64(0), reader.GetString(1)));
                }
            }

            return result;
        }

        public void UpdateAmplitude(long id, double newAmplitude)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Signals SET Amplitude = @amp WHERE Id = @id";

                var cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@amp", newAmplitude);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSignal(long id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // сначала точки
                var cmd1 = new SQLiteCommand("DELETE FROM SignalPoints WHERE SignalId = @id", connection);
                cmd1.Parameters.AddWithValue("@id", id);
                cmd1.ExecuteNonQuery();

                // потом сигнал
                var cmd2 = new SQLiteCommand("DELETE FROM Signals WHERE Id = @id", connection);
                cmd2.Parameters.AddWithValue("@id", id);
                cmd2.ExecuteNonQuery();
            }
        }
    }
}