
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtmosphericSensors.HardwareIO;
using AtmosphericSensors;
using AtmosphericSensors.BME280;
using Moq;

namespace AtmosphericSensors.tests
{
    [TestClass]
    public class AtmosphericSensorFactory_tests
    {
        [TestMethod]
        public void BME280_ThrowsWhenAddressIsOutsideLimit()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                AtmosphericSensorFactory.CreateSensor(AtmosphericSensorTypes.BME280, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                AtmosphericSensorFactory.CreateSensor(AtmosphericSensorTypes.BME280, 256));
        }
        [TestMethod]
        public void CreateSensorThrowsAtUnknownType()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                AtmosphericSensorFactory.CreateSensor((AtmosphericSensorTypes)(-1), 0));
        }
    }
}
