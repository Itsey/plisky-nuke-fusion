using Nuke.Common.Tooling;

namespace Plisky.Nuke.Fusion;

[Serializable]
public class VersonifySettings : ToolOptions {

    /// <summary>
    /// Corresponds to -z on the versonify tool.
    /// </summary>
    public bool AlwaysReturnZero { get; set; } = false;

    /// <summary>
    /// Corresponds to -O in the versionify tool.  Can be env, con, file, np, npo, azdo
    /// </summary>
    public string OutputStyle { get; set; } = string.Empty;

    public string TraceConfiguration { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Debug in the Versonify tool.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Corresponds to -NoOverride in the Versonify tool.
    /// </summary>
    public bool NoOverride { get; set; }

    /// <summary>
    /// Corresponds to -Digits in the Versonify tool.
    /// </summary>
    public string Digits { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -MM in the Versonify tool.
    /// </summary>
    public string MultiMatchFile { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Increment in the Versonify tool.
    /// </summary>
    public bool PerformIncrement { get; set; }

    /// <summary>
    /// Corresponds to -Q in the Versonify tool.
    /// </summary>
    public string QuickValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Release in the Versonify tool.
    /// </summary>
    public string Release { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Root in the Versonify tool.
    /// </summary>
    public string Root { get; set; } = string.Empty;

    public string VersionPersistanceValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -MM in the Versonify tool.
    /// </summary>
    public string[] VersionTargetMinMatch { get; set; } = new string[0];

    /// <summary>
    /// Corresponds to -Command in the Versonify tool.
    /// </summary>
    public string Command { get; set; } = string.Empty;

    public void SetCommand(VersonifyCommand cmd) {
        Command = "-Command=" + cmd.ToString();
    }

    //public override string ProcessToolPath => GetPath();

    public bool DryRun { get; set; }

    public string? Framework { get; set; }

    public string GetPath() {
        string? fw = string.IsNullOrEmpty(Framework) ? null : Framework;
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Versonify",
          packageExecutable: "Versonify.dll|versonify.exe",
          framework: fw);
    }

    public ArgumentStringHandler GetArgsAsString200() {
        // Compatible with V1.0.2 of Versonify.  This is the current version as the time of writing this code.
        // Indicated by 200 level return code from versonify.

        var result = new ArgumentStringHandler(0, 0, out _);
        result.AppendLiteral(Command);
        result.AppendLiteral($" -v={VersionPersistanceValue}");
        result.AppendLiteral($" -Root={Root}");

        if (!string.IsNullOrEmpty(QuickValue)) {
            result.AppendLiteral($" -Q={QuickValue}");
        }

        if (!string.IsNullOrEmpty(MultiMatchFile)) {
            result.AppendLiteral($" -m={MultiMatchFile}");
        }

        if (Debug) {
            result.AppendLiteral(" -Debug");
        }

        if (DryRun) {
            result.AppendLiteral(" -DryRun");
        }

        if (PerformIncrement) {
            result.AppendLiteral(" -Increment");
        }

        if (AlwaysReturnZero) {
            result.AppendLiteral(" -z");
        }

        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            result.AppendLiteral($" -Trace={TraceConfiguration}");
        }

        if (OutputStyle != string.Empty) {
            if (!OutputStyle.EndsWith("-nf")) {
                OutputStyle += "-nf";
            }
            result.AppendLiteral($" -O={OutputStyle}");
        } else {
            result.AppendLiteral(" -O=con-nf");
        }
        return result;
    }

    public ArgumentStringHandler GetArgsString() {
        // This is the first implementaiton, compatible with versions below 1.0.2 - no return code from versonify.

        var result = new ArgumentStringHandler(0, 0, out _);
        result.AppendLiteral(Command);
        result.AppendLiteral($" -vs={VersionPersistanceValue}");
        result.AppendLiteral($" -Root={Root}");

        if (!string.IsNullOrEmpty(QuickValue)) {
            result.AppendLiteral($" -Q={QuickValue}");
        }

        if (!string.IsNullOrEmpty(MultiMatchFile)) {
            result.AppendLiteral($" -mm={MultiMatchFile}");
        }

        if (Debug) {
            result.AppendLiteral(" -Debug");
        }

        if (DryRun) {
            result.AppendLiteral(" -DryRun");
        }

        if (PerformIncrement) {
            result.AppendLiteral(" -Increment");
        }

        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            result.AppendLiteral($" -Trace={TraceConfiguration}");
        }

        if (OutputStyle != string.Empty) {
            if (!OutputStyle.EndsWith("-nf")) {
                OutputStyle += "-nf";
            }
            result.AppendLiteral($" -O={OutputStyle}");
        } else {
            result.AppendLiteral(" -O=con-nf");
        }
        return result;
    }
}