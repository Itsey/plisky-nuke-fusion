using System.Reflection;

namespace Plisky.Fallout.Fusion {
    public static class PffUtilities {
        private static string pffCache = string.Empty;

        public static string GetPffString() {
            if (string.IsNullOrEmpty(pffCache)) {
                string? ver = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                pffCache = $"[PNF] ({ver})";
            }
            return pffCache;
        }
    }
}
