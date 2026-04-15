namespace pawledger.Services;

public static class HapticService
{
    public static void Vibrate(int milliseconds = 80)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(milliseconds));
        }
        catch
        {
            // Device may not support vibration
        }
    }
}