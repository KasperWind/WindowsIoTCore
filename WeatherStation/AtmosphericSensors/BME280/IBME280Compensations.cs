using AtmosphericSensors.HardwareIO;

namespace AtmosphericSensors.BME280
{
    public interface IBME280Compensations
    {
        int FineTemperature { get; }
        long[] Humidity { get; }
        long[] Pressure { get; }
        long[] Temperature { get; }
        void CalculateFineTemperature(int rawTemperature);
        double CalculateHumidity(int rawHumidity);
        double CalculatePressure(int rawPressure);
        double CalculateTemperature(int rawTemperature);
        void ReadHumitidyCompensation(II2cSensor bme280sensor);
        void ReadPressureCompensation(II2cSensor bme280sensor);
        void ReadTemperatureCompensation(II2cSensor bme280sensor);
        string ToString();
    }
}