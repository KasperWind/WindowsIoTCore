namespace AtmosphericSensors
{
    public interface IBarometricPressureSensor
    {
        double GetBarometricPressure();
    }
    public interface IHumiditySensor
    {
        double GetHumidity();
    }

    public interface ITemperatureSensor
    {
        double GetTemperature();
    }

    public interface IAtmosphericSensor : IBarometricPressureSensor, IHumiditySensor, ITemperatureSensor
    {

    }

}
