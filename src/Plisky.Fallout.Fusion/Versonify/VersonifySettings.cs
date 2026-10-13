using Fallout.Common.Tooling;

namespace Plisky.Fallout.Fusion;

[Serializable]
public class VersonifySettings : ToolOptions {

    /// <summary>
    /// Corresponds to -z on the versonify tool.
    /// </summary>
    public bool AlwaysReturnZero { get; set; } = false;

    /// <summary>
    /// Corresponds to --output in the versonify tool. Supports env, con, jcon, azdo[:varname], file[:filename], con-pf and con-nf.
    /// </summary>
    public string OutputStyle { get; set; } = string.Empty;

    public string TraceConfiguration { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --debug in the Versonify tool.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Corresponds to --no-override in the Versonify tool.
    /// </summary>
    public bool NoOverride { get; set; }

    /// <summary>
    /// Corresponds to --digits in the Versonify tool.
    /// </summary>
    public string Digits { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --digit-group in the Versonify tool.
    /// </summary>
    public string DigitGroup { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --pre-release in the Versonify tool.
    /// </summary>
    public bool PreRelease { get; set; }

    /// <summary>
    /// Corresponds to --min-match in the Versonify tool.
    /// </summary>
    public string MultiMatchFile { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --increment in the Versonify tool.
    /// </summary>
    public bool PerformIncrement { get; set; }

    /// <summary>
    /// Corresponds to --quick-value in the Versonify tool.
    /// </summary>
    public string QuickValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --release in the Versonify tool.
    /// </summary>
    public string Release { get; set; } = string.Empty;

    /// <summary>
    /// Requests the stored release name instead of the full version when using Passive.
    /// </summary>
    public bool OutputReleaseName { get; set; }

    /// <summary>
    /// Corresponds to -Root in the Versonify tool.
    /// </summary>
    public string Root { get; set; } = string.Empty;

    public string VersionPersistanceValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to --min-match in the Versonify tool.
    /// </summary>
    public string[] VersionTargetMinMatch { get; set; } = [];

    /// <summary>
    /// Corresponds to --command in the Versonify tool.
    /// </summary>
    public string Command { get; set; } = string.Empty;

    public void SetCommand(VersonifyCommand cmd) {
        SetCommand(cmd, useLongOption: false);
    }

    public void SetCommand(VersonifyCommand cmd, bool useLongOption) {
        Command = (useLongOption ? "--command=" : "-Command=") + cmd;
    }

    /// <summary>
    /// Corresponds to --dry-run in the Versonify tool.
    /// </summary>
    public bool DryRun { get; set; }

    /// <summary>
    /// Corresponds to the target framework for the Versonify tool.  If not specified, the default framework will be used.
    /// </summary>
    public string? Framework { get; set; }

    public string GetPath() {
        string? fw = string.IsNullOrEmpty(Framework) ? null : Framework;
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Versonify",
          packageExecutable: "Versonify.dll|versonify.exe",
          framework: fw);
    }

    public ArgumentStringHandler GetArgsAsString201() {
        // Compatible with V2.0 Bronte release of Versonify.  This is the current version as the time of writing this code.
        // Indicated by 201 level return code from versonify.

        var result = new ArgumentStringHandler(0, 0, out _);
        result.AppendLiteral(GetCommandArgument(legacy: false));
        AppendOption(ref result, "--version-source", VersionPersistanceValue);
        AppendOption(ref result, "--root", Root);

        if (!string.IsNullOrEmpty(QuickValue)) {
            AppendOption(ref result, "--quick-value", QuickValue);
        }

        if (!string.IsNullOrEmpty(Release)) {
            result.AppendLiteral($" --release={Release}");
        } else if (OutputReleaseName) {
            result.AppendLiteral(" --release");
        }

        if (!string.IsNullOrEmpty(MultiMatchFile)) {
            AppendOption(ref result, "--min-match", MultiMatchFile);
        }

        if (Debug) {
            result.AppendLiteral(" --debug");
        }

        if (NoOverride) {
            result.AppendLiteral(" --no-override");
        }

        if (!string.IsNullOrEmpty(Digits)) {
            AppendOption(ref result, "--digits", Digits);
        }

        if (!string.IsNullOrEmpty(DigitGroup)) {
            AppendOption(ref result, "--digit-group", DigitGroup);
        }

        if (PreRelease) {
            result.AppendLiteral(" --pre-release");
        }

        if (DryRun) {
            result.AppendLiteral(" --dry-run");
        }

        if (PerformIncrement) {
            result.AppendLiteral(" --increment");
        }

        if (AlwaysReturnZero) {
            result.AppendLiteral(" --no-error");
        }

        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            AppendOption(ref result, "--trace", TraceConfiguration);
        }

        AppendOption(ref result, "--output", string.IsNullOrEmpty(OutputStyle) ? "con-pf" : OutputStyle);
        return result;
    }

    private static void AppendOption(ref ArgumentStringHandler arguments, string option, string value) {
        arguments.AppendLiteral($" {option}=");
        arguments.AppendFormatted(value);
    }

    private string GetCommandArgument(bool legacy) {
        string commandName = Command;
        foreach (string prefix in new[] { "--command=", "-Command=" }) {
            if (commandName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
                commandName = commandName[prefix.Length..];
                break;
            }
        }

        if (string.IsNullOrEmpty(commandName)) {
            return string.Empty;
        }

        if (legacy) {
            if (Enum.TryParse<VersonifyCommand>(commandName, ignoreCase: true, out var command)) {
                commandName = command.ToString();
            }
            return "-Command=" + commandName;
        }

        return "--command=" + commandName.ToLowerInvariant();
    }

    private string GetLegacyOutputStyle() {
        if (string.IsNullOrEmpty(OutputStyle) || OutputStyle == "con-pf") {
            return "con-nf";
        }
        return OutputStyle.EndsWith("-nf", StringComparison.Ordinal) ? OutputStyle : OutputStyle + "-nf";
    }

    private void ValidateLegacySettings() {
        if (PreRelease || !string.IsNullOrEmpty(DigitGroup)) {
            throw new InvalidOperationException("Versonify digit groups require compatibility level 201 or above. Update your Versonify tool.");
        }
    }

    public ArgumentStringHandler GetArgsAsString200() {
        // Compatible with V1.0.2 of Versonify.  This is the current version as the time of writing this code.
        // Indicated by 200 level return code from versonify.

        ValidateLegacySettings();
        var result = new ArgumentStringHandler(0, 0, out _);
        result.AppendLiteral(GetCommandArgument(legacy: true));
        result.AppendLiteral($" -v={VersionPersistanceValue}");
        result.AppendLiteral($" -Root={Root}");

        if (!string.IsNullOrEmpty(QuickValue)) {
            result.AppendLiteral($" -Q={QuickValue}");
        }

        if (!string.IsNullOrEmpty(Release)) {
            result.AppendLiteral($" -Release={Release}");
        } else if (OutputReleaseName) {
            result.AppendLiteral(" -Release");
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

        result.AppendLiteral($" -O={GetLegacyOutputStyle()}");
        return result;
    }

    public ArgumentStringHandler GetArgsString() {
        // This is the first implementaiton, compatible with versions below 1.0.2 - no return code from versonify.

        ValidateLegacySettings();
        var result = new ArgumentStringHandler(0, 0, out _);
        result.AppendLiteral(GetCommandArgument(legacy: true));
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

        result.AppendLiteral($" -O={GetLegacyOutputStyle()}");
        return result;
    }
}