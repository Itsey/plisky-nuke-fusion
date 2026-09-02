using System.Reflection;

namespace Plisky.Nuke.Fusion {
    public static class PnfUtilities {
        private static string pnfCache = string.Empty;

        public static string GetPnfString() {
            if (string.IsNullOrEmpty(pnfCache)) {
                string? ver = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                pnfCache = $"[PNF] ({ver})";
            }
            return pnfCache;
        }
    }
}
