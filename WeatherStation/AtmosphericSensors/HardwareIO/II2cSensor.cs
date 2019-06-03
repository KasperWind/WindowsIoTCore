namespace AtmosphericSensors.HardwareIO
{
    public interface II2cSensor
    {
        byte ReadRegister(byte registerAddress);
        long Read16bitRegister(byte addressLSB, byte addressMSB);
        long Read20bitRegister(byte addressLSB, byte address, byte addressMSB);
    }
}