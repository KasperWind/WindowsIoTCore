
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtmosphericSensors.HardwareIO;
using AtmosphericSensors;
using Moq;

namespace AtmosphericSensors.tests
{
    [TestClass]
    public class AtmosphericSensorFactory_tests
    {
        [TestMethod]
        public void GetBME280AtmosphericSensor()
        {
            var bme280 = AtmosphericSensorFactory.CreateSensor(AtmosphericSensors.BME280);

        }
    }
}
