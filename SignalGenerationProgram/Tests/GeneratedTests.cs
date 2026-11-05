using NUnit.Framework;
using System;
using System.Linq;

namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class GeneratedTests
    {
        SignalGenerator generator;

        [SetUp]
        public void Setup()
        {
            generator = new SignalGenerator();
        }

        // -------- GenerateSine --------
        // проверка количества точек
        [Test]
        public void GenerateSine_PointsCount()
        {
            double amplitude = 1;
            double frequency = 1;
            int points = 100;

            var result = generator.GenerateSine(amplitude, frequency, points);
            Assert.That(result.Count, Is.EqualTo(points + 1));
        }
        //Проверка нулевых значений в ожидаемых точках sin(0) = 0; sin(π) = 0; sin(2π) = 0
        [Test]
        public void GenerateSine_ZeroPoints()
        {
            double amplitude = 1;
            double frequency = 1;
            int points = 100;

            var result = generator.GenerateSine(amplitude, frequency, points);

            Assert.That(result[0].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[50].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[100].Value, Is.EqualTo(0).Within(1e-12));
        }
        // Ключевые точки синусоиды t = 0 => 0; t = 0.25 => sin(π/2) = 1; t = 0.5 => 0; t = 0.75 => -1
        [Test]
        public void GenerateSine_PointsAreCorrect()
        {
            double amplitude = 1;
            double frequency = 1;
            int points = 4;

            var result = generator.GenerateSine(amplitude, frequency, points);

            Assert.That(result[0].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[1].Value, Is.EqualTo(1).Within(1e-12));
            Assert.That(result[2].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[3].Value, Is.EqualTo(-1).Within(1e-12));
            Assert.That(result[4].Value, Is.EqualTo(0).Within(1e-12));
        }
        // Проверка пересечения с 0
        [Test]
        public void GenerateSine_ZeroCrossing()
        {
            double amplitude = 1;
            double frequency = 2;
            int points = 1000;

            var result = generator.GenerateSine(amplitude, frequency, points);

            int zeroCrossings = 0;

            for (int i = 1; i < result.Count; i++)
            {
                if (result[i - 1].Value * result[i].Value < 0)
                    zeroCrossings++;
            }

            Assert.That(Math.Abs(zeroCrossings - 4) <= 1, Is.True);
        }
        //проверка на входные нулевые параметры
        [Test]
        public void GenerateSine_WhenInputsParamIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(1, 0, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(0, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(1, 1, 0));
        }
        //проверка на отрицательные параметры
        [Test]
        public void GenerateSine_WhenInputsParamIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(-1, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(1, -1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSine(1, 1, -100));
        }

        // -------- GenerateMeander --------
        // проверка количества точек
        [Test]
        public void GenerateMeander_PointsCount()
        {
            int points = 100;
            var result = generator.GenerateMeander(1, 1, points);

            Assert.That(result.Count, Is.EqualTo(points));
        }
        // проверка амплитуды
        [Test]
        public void GenerateMeander_PlusAndMinusAmplitude()
        {
            double amplitude = 2.5;
            int pointsCount = 200;

            var result = generator.GenerateMeander(amplitude, 1, pointsCount);

            for (int i = 0; i < result.Count; i++)
            {
                double value = result[i].Value;
                Assert.That(value, Is.EqualTo(amplitude).Or.EqualTo(-amplitude));
            }
        }
        // Проверка пересечения с 0
        [Test]
        public void GenerateMeander_ZeroCrossing()
        {
            double amplitude = 1;
            double frequency = 2;
            int points = 1000;

            var result = generator.GenerateMeander(amplitude, frequency, points);

            int zeroCrossings = 0;

            for (int i = 1; i < result.Count; i++)
            {
                if (result[i - 1].Value * result[i].Value < 0)
                    zeroCrossings++;
            }

            Assert.That(Math.Abs(zeroCrossings - 4) <= 1, Is.True);
        }
        //проверка на входные нулевые параметры
        [Test]
        public void GenerateMeander_WhenInputsParamIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(1, 0, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(0, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(1, 1, 0));
        }
        //проверка на отрицательные параметры
        [Test]
        public void GenerateMeander_WhenInputsParamIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(-1, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(1, -1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateMeander(1, 1, -100));
        }

        // -------- GenerateTriangular --------
        // проверка количества точек
        [Test]
        public void GenerateTriangular_PointsCount()
        {
            int points = 100;
            var result = generator.GenerateTriangular(1, 1, points);

            Assert.That(result.Count, Is.EqualTo(points + 1));
        }
        // Проверка значений точек
        [Test]
        public void GenerateTriangular_PointsAreCorrect()
        {
            var result = generator.GenerateTriangular(1, 1, 4);

            Assert.That(result[0].Value, Is.EqualTo(-1).Within(1e-12));
            Assert.That(result[1].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[2].Value, Is.EqualTo(1).Within(1e-12));
            Assert.That(result[3].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[4].Value, Is.EqualTo(-1).Within(1e-12));
        }
        // проверка амплитуды
        [Test]
        public void GenerateTriangular_CorrectMinAndMax()
        {
            double amplitude = 5;
            var result = generator.GenerateTriangular(amplitude, 10, 1000);

            double max = result.Max(p => p.Value);
            double min = result.Min(p => p.Value);

            Assert.That(max, Is.EqualTo(amplitude).Within(1e-12));
            Assert.That(min, Is.EqualTo(-amplitude).Within(1e-12));
        }
        //проверка линейности
        [Test]
        public void GenerateTriangular_IsLinear()
        {
            var result = generator.GenerateTriangular(1, 1, 100);

            int mid = result.Count / 2;

            for (int i = 1; i < mid; i++)
            {
                Assert.That(result[i].Value, Is.GreaterThanOrEqualTo(result[i - 1].Value));
            }

            for (int i = mid + 1; i < result.Count; i++)
            {
                Assert.That(result[i].Value, Is.LessThanOrEqualTo(result[i - 1].Value));
            }
        }
        //проверка на входные нулевые параметры
        [Test]
        public void GenerateTriangular_WhenInputsParamIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(1, 0, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(0, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(1, 1, 0));
        }
        //проверка на отрицательные параметры
        [Test]
        public void GenerateTriangular_WhenInputsParamIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(-1, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(1, -1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateTriangular(1, 1, -100));
        }

        // -------- GenerateSawtooth --------
        // проверка количества точек
        [Test]
        public void GenerateSawtooth_PointsCount()
        {
            int points = 100;
            var result = generator.GenerateSawtooth(1, 1, points);

            Assert.That(result.Count, Is.EqualTo(points + 1));
        }
        // Проверка значений точек
        [Test]
        public void GenerateSawtooth_PointsAreCorrect()
        {
            var result = generator.GenerateSawtooth(1, 1, 4);

            Assert.That(result[0].Value, Is.EqualTo(-1).Within(1e-12));
            Assert.That(result[1].Value, Is.EqualTo(-0.5).Within(1e-12));
            Assert.That(result[2].Value, Is.EqualTo(0).Within(1e-12));
            Assert.That(result[3].Value, Is.EqualTo(0.5).Within(1e-12));
            Assert.That(result[4].Value, Is.EqualTo(-1).Within(1e-12));
        }
        // проверка амплитуды
        [Test]
        public void GenerateSawtooth_CorrectMinAndMax()
        {
            double amplitude = 5;
            var result = generator.GenerateSawtooth(amplitude, 10, 1000);

            double max = result.Max(p => p.Value);
            double min = result.Min(p => p.Value);

            Assert.That(max, Is.EqualTo(amplitude).Within(1e-12));
            Assert.That(min, Is.EqualTo(-amplitude).Within(1e-12));
        }
        //проверка линейности и спада
        [Test]
        public void GenerateSawtooth_IsLinearAndDrop()
        {
            var result = generator.GenerateSawtooth(1, 1, 100);

            bool hasDrop = false;

            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].Value < result[i - 1].Value)
                {
                    hasDrop = true;
                    continue;
                }

                if (!hasDrop)
                {
                    Assert.That(result[i].Value, Is.GreaterThanOrEqualTo(result[i - 1].Value));
                }
            }

            Assert.That(hasDrop, Is.True);
        }
        //проверка на входные нулевые параметры
        [Test]
        public void GenerateSawtooth_WhenInputsParamIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(1, 0, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(0, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(1, 1, 0));
        }
        //проверка на отрицательные параметры
        [Test]
        public void GenerateSawtooth_WhenInputsParamIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(-1, 1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(1, -1, 100));

            Assert.Throws<ArgumentException>(() =>
                generator.GenerateSawtooth(1, 1, -100));
        }
    }
}
