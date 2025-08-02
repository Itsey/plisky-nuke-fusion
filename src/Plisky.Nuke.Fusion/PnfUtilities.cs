using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plisky.Nuke.Fusion {
    public static class PnfUtilities {
        private static string pnfCache = string.Empty;

        public static string GetPnfString() {
            if (string.IsNullOrEmpty(pnfCache)) {
                var ver = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                pnfCache = $"[PNF] ({ver})";
            }
            return pnfCache;
        }
    }
}
