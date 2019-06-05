using System;
using Xunit;
using AtmosphericSensors;
using AtmosphericSensors.HardwareIO;

namespace AtmosphericSensors.tests
{
    public class AtmosphericSensorBME280_tests
    {
        [Fact]
        public void DoSomeTest()
        {
            var test = new AtmosphericSensorBME280(new I2cSensor(0x77));
            
        }
    }
}
