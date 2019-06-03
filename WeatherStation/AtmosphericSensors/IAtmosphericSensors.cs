namespace AtmosphericSensors
{
    interface IBarometricPressureSensor
    {
        double GetBarometricPressure();
    }
    interface IHumiditySensor
    {
        double GetHumidity();
    }

    interface ITemperatureSensor
    {
        double GetTemperature();
    }
}
