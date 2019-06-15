using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AtmosphericSensors.HardwareIO;
using AtmosphericSensors.BME280;

namespace AtmosphericSensors.tests.BME280
{
    [TestClass]
    public class BME280Compensations_tests
    {
        private Mock<II2cSensor> sensorMock = new Mock<II2cSensor>();

        [TestInitialize]
        public void MockInitialization()
        {
            SetupDigT();
            SetupDigP();
            SetupDigH();
        }

        private void SetupDigT()
        {
            sensorMock.Setup(x => x.ReadUInt16Register((byte)BME280Registers.DigT1)).Returns(1);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigT2)).Returns(2);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigT3)).Returns(3);
        }

        private void SetupDigP()
        {
            sensorMock.Setup(x => x.ReadUInt16Register((byte)BME280Registers.DigP1)).Returns(1);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP2)).Returns(2);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP3)).Returns(3);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP4)).Returns(4);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP5)).Returns(5);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP6)).Returns(6);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP7)).Returns(7);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP8)).Returns(8);
            sensorMock.Setup(x => x.ReadInt16Register((byte)BME280Registers.DigP9)).Returns(9);
        }

        private void SetupDigH()
        {
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH1)).Returns(1);
            sensorMock.Setup(x => x.ReadUInt16Register((byte)BME280Registers.DigH2)).Returns(2);
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH3)).Returns(3);
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH4)).Returns(4);
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH5)).Returns(5);
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH5 + 1)).Returns(6);
            sensorMock.Setup(x => x.ReadRegister((byte)BME280Registers.DigH6)).Returns(6);
        }

        private long[] expectedTemperature = { 0, 1, 2, 3};
        private long[] expectedHumitidy = { 0, 1, 2, 3, 69, 96, 6 };
        private long[] expectedPressure = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [TestMethod]
        public void CreateNewInstanceCompensationWithExpectedTemperatureValues()
        {
            IBME280Compensations actualObject = new BME280Compensations(sensorMock.Object);
            for (int i = 1; i < actualObject.Temperature.Length; i++)
            {
                Assert.AreEqual(expectedTemperature[i], actualObject.Temperature[i]);
            }
        }

        [TestMethod]
        public void CreateNewInstanceCompensationWithExpectedPressureValues()
        {
            IBME280Compensations actualObject = new BME280Compensations(sensorMock.Object);
            for (int i = 1; i < actualObject.Pressure.Length; i++)
            {
                Assert.AreEqual(expectedPressure[i], actualObject.Pressure[i]);
            }
        }

        [TestMethod]
        public void CreateNewInstanceCompensationWithExpectedHumidityValues()
        {
            IBME280Compensations actualObject = new BME280Compensations(sensorMock.Object);
            for (int i = 1; i < actualObject.Humidity.Length; i++)
            {
                Assert.AreEqual(expectedHumitidy[i], actualObject.Humidity[i]);
            }
        }
    }
}
