namespace InfiniStacker.Feedback
{
    public static class FeedbackServices
    {
        public static ScreenShakeService ScreenShake { get; private set; }
        public static IHapticsService Haptics { get; private set; }

        public static void Configure(ScreenShakeService screenShake, IHapticsService haptics)
        {
            ScreenShake = screenShake;
            Haptics = haptics;
        }
    }
}
