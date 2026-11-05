using NUnit.Framework;

namespace SignalGenerationProgram.Tests
{
    [TestFixture]
    internal class SignalProcessingTests
    {
        private SignalGenerator generator;
        private SignalProcessing sp;

        [SetUp]
        public void Setup()
        {
            generator = new SignalGenerator();
            sp = new SignalProcessing();
        }

        [Test]
        public void NumberZeroLine_Tests()
        {
            double amplitude = 2.0;
            double frequency = 10.0;
            int pointsCount = 1000;

            int result = 19;
            int triangle_sawtooth_result = 20;

            var sinus_points = generator.GenerateSine(amplitude, frequency, pointsCount);
            int sinus_crossings = sp.NumberZeroLine(sinus_points);
            Assert.That(sinus_crossings, Is.EqualTo(result));

            var meander_points = generator.GenerateMeander(amplitude, frequency, pointsCount);
            int meander_crossings = sp.NumberZeroLine(meander_points);
            Assert.That(meander_crossings, Is.EqualTo(result));

            var triangle_points = generator.GenerateTriangular(amplitude, frequency, pointsCount);
            int triangle_crossings = sp.NumberZeroLine(triangle_points);
            Assert.That(triangle_crossings, Is.EqualTo(triangle_sawtooth_result));

            var sawtooth_points = generator.GenerateSawtooth(amplitude, frequency, pointsCount);
            int sawtooth_crossings = sp.NumberZeroLine(sawtooth_points);
            Assert.That(sawtooth_crossings, Is.EqualTo(triangle_sawtooth_result));
        }

        [Test]
        public void MaxSignal_Test()
        {
            double amplitude = 2.0;
            double frequency = 2.0;
            int pointsCount = 1000;

            var sine_points = generator.GenerateSine(amplitude, frequency, pointsCount);
            double sine_max = sp.MaxSignal(sine_points);
            Assert.That(sine_max, Is.EqualTo(amplitude).Within(1e-2));

            var triangle_points = generator.GenerateTriangular(amplitude, frequency, pointsCount);
            double triangle_max = sp.MaxSignal(triangle_points);
            Assert.That(triangle_max, Is.EqualTo(amplitude).Within(1e-2));

            var meander_points = generator.GenerateMeander(amplitude, frequency, pointsCount);
            double meander_max = sp.MaxSignal(meander_points);
            Assert.That(meander_max, Is.EqualTo(amplitude).Within(1e-2));

            var sawtooth_points = generator.GenerateSawtooth(amplitude, frequency, pointsCount);
            double sawtooth_max = sp.MaxSignal(sawtooth_points);
            Assert.That(sawtooth_max, Is.EqualTo(amplitude).Within(1e-2));
        }

        [Test]
        public void MinSignal_Test()
        {
            double amplitude = 2.0;
            double frequency = 2.0;
            int pointsCount = 1000;

            var sine_points = generator.GenerateSine(amplitude, frequency, pointsCount);
            double sine_min = sp.MinSignal(sine_points);
            Assert.That(sine_min, Is.EqualTo(-amplitude).Within(1e-2));

            var meander_points = generator.GenerateMeander(amplitude, frequency, pointsCount);
            double meander_min = sp.MinSignal(meander_points);
            Assert.That(meander_min, Is.EqualTo(-amplitude).Within(1e-2));

            var triangle_points = generator.GenerateTriangular(amplitude, frequency, pointsCount);
            double triangle_min = sp.MinSignal(triangle_points);
            Assert.That(triangle_min, Is.EqualTo(-amplitude).Within(1e-2));

            var sawtooth_points = generator.GenerateSawtooth(amplitude, frequency, pointsCount);
            double sawtooth_min = sp.MinSignal(sawtooth_points);
            Assert.That(sawtooth_min, Is.EqualTo(-amplitude).Within(1e-2));
        }

        [Test]
        public void AverageSignal_Test()
        {
            double amplitude = 2.0;
            double frequency = 1.0;
            int pointsCount = 1000;
            double result = 0;

            var sinus_points = generator.GenerateSine(amplitude, frequency, pointsCount);
            double sinus_avg = sp.AverageSignal(sinus_points);
            Assert.That(sinus_avg, Is.EqualTo(result).Within(1e-2));

            var meander_points = generator.GenerateMeander(amplitude, frequency, pointsCount);
            double meander_avg = sp.AverageSignal(meander_points);
            Assert.That(meander_avg, Is.EqualTo(result).Within(1e-2));

            var triangle_points = generator.GenerateTriangular(amplitude, frequency, pointsCount);
            double triangle_avg = sp.AverageSignal(triangle_points);
            Assert.That(triangle_avg, Is.EqualTo(result).Within(1e-2));

            var sawtooth_points = generator.GenerateSawtooth(amplitude, frequency, pointsCount);
            double sawtooth_avg = sp.AverageSignal(sawtooth_points);
            Assert.That(sawtooth_avg, Is.EqualTo(result).Within(1e-2));
        }
    }
}
