namespace Plisky.Fallout.Fusion;

internal sealed class VersonifyOutputValues {
    public ISet<string> FoundMarkers { get; } = new HashSet<string>(StringComparer.Ordinal);
    public string ShortVersion { get; set; } = string.Empty;
    public string ReleaseName { get; set; } = string.Empty;
    public string VersionLiteral { get; set; } = string.Empty;
    public string FourDigitNumeric { get; set; } = string.Empty;
    public string ThreeDigit { get; set; } = string.Empty;
    public string QueuedFull { get; set; } = string.Empty;
    public string ThreeDigitNumeric { get; set; } = string.Empty;
    public string FourDigit { get; set; } = string.Empty;
}

internal static class VersonifyOutputParser {
    internal static VersonifyOutputValues Parse(string output) {
        var values = new VersonifyOutputValues();
        string[] lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines) {
            if (line.StartsWith("PNFV]")) {
                values.FoundMarkers.Add("PNFV]");
                values.VersionLiteral = line[5..];
            } else if (line.StartsWith("PNF2]")) {
                values.FoundMarkers.Add("PNF2]");
                values.ShortVersion = line[5..];
            } else if (line.StartsWith("PNFN]")) {
                values.FoundMarkers.Add("PNFN]");
                values.ReleaseName = line[5..];
            } else if (line.StartsWith("PNF3]")) {
                values.FoundMarkers.Add("PNF3]");
                values.ThreeDigit = line[5..];
            } else if (line.StartsWith("PN4D]")) {
                values.FoundMarkers.Add("PN4D]");
                values.FourDigitNumeric = line[5..];
            } else if (line.StartsWith("PNQF]")) {
                values.FoundMarkers.Add("PNQF]");
                values.QueuedFull = line[5..];
            } else if (line.StartsWith("PN3D]")) {
                values.FoundMarkers.Add("PN3D]");
                values.ThreeDigitNumeric = line[5..];
            } else if (line.StartsWith("PNF4]")) {
                values.FoundMarkers.Add("PNF4]");
                values.FourDigit = line[5..];
            }
        }
        return values;
    }
}
