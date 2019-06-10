
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AtmosphericSensors.HardwareIO;
using AtmosphericSensors;
using Moq;

namespace AtmosphericSensors.tests
{
    [TestClass]
    public class AtmosphericSensorBME280Compensations_tests
    {
        private long expected = 666L;
        private byte expectedByte = 10;

        private long[] expectedTemperature = (long[])Array.CreateInstance(typeof(long), 4);
        private long[] expectedPressure = (long[])Array.CreateInstance(typeof(long), 10);
        private long[] expectedHumitidy = { 0, 10, 666, 10, (10 << 4) + (10 & 0x0F) , (10 << 4) + ((10 >> 4) & 0x0F), 10 }; //{ 0, 10, 666, 10, 170, 160, 10 };
        
        private long rawTemperatureInput = 100;
        private long expectedFineTemperature = -426;
        private double expectedCalculatedTemperature = -0.08;

        private long rawPressureInput = 100;
        private double expectedCalculatePressure = 11621059.47;

        private long rawHumidityInput = 20000;
        private double expectedCalculateHumidity = 99.012;

        private Mock<II2cSensor> mock = new Mock<II2cSensor>();
        AtmosphericSensorBME280Compensations actualObject;
        

        [TestInitialize()]
        public void TestInitialize()
        {
            Array.Fill(expectedTemperature, expected);
            Array.Fill(expectedPressure, expected);

            mock.Setup(x => x.Read16bitRegister(It.IsAny<byte>(), It.IsAny<byte>())).Returns(expected);
            mock.Setup(x => x.ReadRegister(It.IsAny<byte>())).Returns(expectedByte);

            actualObject = new AtmosphericSensorBME280Compensations(mock.Object);
        }

        [TestMethod]
        public void NewInstanceCreatedWithExpectedValues()
        {
            for (int i = 1; i < actualObject.Temperature.Length; i++)
            {
                Assert.AreEqual(expected, actualObject.Temperature[i]);
            }
            for (int i = 1; i < actualObject.Pressure.Length; i++)
            {
                Assert.AreEqual(expected, actualObject.Pressure[i]);
            }
            for (int i = 1; i < actualObject.Humidity.Length; i++)
            {
                Assert.AreEqual(expectedHumitidy[i], actualObject.Humidity[i]);
            }
        }

        [TestMethod]
        public void ShouldCalculateTemperature()
        {
            var actual = actualObject.CalculateTemperature(rawTemperatureInput);

            Assert.AreEqual(expectedCalculatedTemperature, actual);
        }

        [TestMethod]
        public void ShouldCalculateFineTemperature()
        {
            actualObject.CalculateTemperature(rawTemperatureInput);

            Assert.AreEqual(expectedFineTemperature, actualObject.FineTemperature);
        }

        [TestMethod]
        public void ShouldCalculatePressure()
        {
            actualObject.CalculateTemperature(rawTemperatureInput);
            double actual = Math.Round(actualObject.CalculatePressure(rawPressureInput),2);

            Assert.AreEqual(expectedCalculatePressure, actual);
        }

        [TestMethod]
        public void ShouldCalculateHumidity()
        {
            actualObject.CalculateTemperature(rawTemperatureInput);
            double actual = Math.Round(actualObject.CalculateHumidity(rawHumidityInput), 3);

            Assert.AreEqual(expectedCalculateHumidity, actual);

        }
    }
}
