namespace Certitrack.Extensions
{
    public static class AppMode
    {
#if DEBUG
        public const bool isDebug = true;
#else
        public const bool isDebug = false;
#endif
    }
}
