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

        foreach (string outputLine in lines) {
            // Normalize legacy Nuke Fusion markers to the Plisky Fusion marker names.
            string line = outputLine.StartsWith("PN", StringComparison.Ordinal) ? "P" + outputLine[2..] : outputLine;
            if (line.StartsWith("PFV]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PFV]");
                values.VersionLiteral = line[4..];
            } else if (line.StartsWith("PF2]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PF2]");
                values.ShortVersion = line[4..];
            } else if (line.StartsWith("PFN]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PFN]");
                values.ReleaseName = line[4..];
            } else if (line.StartsWith("PF3]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PF3]");
                values.ThreeDigit = line[4..];
            } else if (line.StartsWith("P4D]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("P4D]");
                values.FourDigitNumeric = line[4..];
            } else if (line.StartsWith("PQF]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PQF]");
                values.QueuedFull = line[4..];
            } else if (line.StartsWith("P3D]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("P3D]");
                values.ThreeDigitNumeric = line[4..];
            } else if (line.StartsWith("PF4]", StringComparison.Ordinal)) {
                values.FoundMarkers.Add("PF4]");
                values.FourDigit = line[4..];
            }
        }
        return values;
    }
}
