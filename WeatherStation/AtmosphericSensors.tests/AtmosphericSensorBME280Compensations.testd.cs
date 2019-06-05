using System;
using Xunit;
using AtmosphericSensors;
using AtmosphericSensors.HardwareIO;

namespace AtmosphericSensors.tests
{
    public class AtmosphericSensorBME280Compensations_tests
    {
        [Fact]
        public void ThrowsWhenCreatedWithInvalidCensor()
        {
            Assert.Throws<Exception>(
                () => new AtmosphericSensorBME280Compensations(new I2cSensor(0))
                );
        }
    }
}
