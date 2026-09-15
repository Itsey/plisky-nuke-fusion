namespace Plisky.Fallout.Fusion;

using System;
using System.Collections.Generic;
using global::Fallout.Common.Tooling;

public class VersonifyTasks : ToolTasks, IRequirePathTool {
    public const int VERSONIFY_AUSTEN_COMPAT_CONSTANT = 200;
    public const int VERSONIFY_BRONTE_COMPAT_CONSTANT = 201;

    public VersonifyTasks() {
        GetLogger().Invoke(OutputType.Std, $"{PnfUtilities.GetPnfString()} [Versonify Tasks]");
    }

    public static IReadOnlyCollection<Output> Versonify(ArgumentStringHandler arguments, string? workingDirectory = null, IReadOnlyDictionary<string, string>? environmentVariables = null, int? timeout = null, bool? logOutput = null, bool? logInvocation = null, Action<OutputType, string>? logger = null, Func<IProcess, object>? exitHandler = null)
        => new VersonifyTasks().Run(arguments, workingDirectory, environmentVariables, timeout, logOutput, logInvocation, logger, exitHandler);


    public IReadOnlyCollection<Output> RawExecute(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return RawExecute(configure(new VersonifySettings()));
    }


    public IReadOnlyCollection<Output> ExecuteVersonify(VersonifySettings? settings, VersonifyCommand command = VersonifyCommand.Unknown, string? replaceCommandLine = null) {
        settings ??= new VersonifySettings();
        ValidateSettings(settings, command);
        SetCommand(settings, command);
        SetToolPath(settings.GetPath());

        var arguments = GetArguments(settings, replaceCommandLine);
        var result = RunCommand(arguments, command);
        ParseVersionOutput(result.StdToText());
        ValidateVersionOutput(command, VersionLiteral, GetOutputText(result));
        return result;
    }

    private static void ValidateSettings(VersonifySettings settings, VersonifyCommand command) {
        if (command == VersonifyCommand.Passive && settings.AlwaysReturnZero) {
            throw new InvalidOperationException("Versonify Passive must not use --no-error because version-query failures must fail the build.");
        }
    }

    private static void SetCommand(VersonifySettings settings, VersonifyCommand command) {
        if (command != VersonifyCommand.Unknown) {
            settings.SetCommand(command);
        }
    }

    private ArgumentStringHandler GetArguments(VersonifySettings settings, string? replaceCommandLine) {
        if (replaceCommandLine != null) {
            var replacementArguments = new ArgumentStringHandler(0, 0, out _);
            replacementArguments.AppendLiteral(replaceCommandLine);
            return replacementArguments;
        }

        int compatibilityLevel = GetCompatibilityLevel();
        return compatibilityLevel switch {
            VERSONIFY_BRONTE_COMPAT_CONSTANT => settings.GetArgsAsString201(),
            VERSONIFY_AUSTEN_COMPAT_CONSTANT => settings.GetArgsAsString200(),
            _ => GetLegacyArguments(settings, compatibilityLevel)
        };
    }

    private int GetCompatibilityLevel() {
        int compatibilityLevel = 0;
        Run("--QQpnf", exitHandler: process => {
            if (process.ExitCode != 0) {
                GetLogger().Invoke(OutputType.Std, $"Versonify Compat Code: {process.ExitCode}");
                compatibilityLevel = process.ExitCode;
            }
            return process;
        });
        return compatibilityLevel;
    }

    private IReadOnlyCollection<Output> RunCommand(ArgumentStringHandler arguments, VersonifyCommand command) {
        int exitCode = 0;
        var result = Run(arguments, exitHandler: process => {
            exitCode = process.ExitCode;
            return process;
        });

        ThrowIfCommandFailed(command, exitCode, GetOutputText(result));
        return result;
    }

    private void ParseVersionOutput(string output) {
        var values = VersonifyOutputParser.Parse(output);
        ShortVersion = values.ShortVersion;
        ReleaseName = values.ReleaseName;
        VersionLiteral = values.VersionLiteral;
        FourDigitNumeric = values.FourDigitNumeric;
        ThreeDigit = values.ThreeDigit;
        QueuedFull = values.QueuedFull;
        ThreeDigitNumeric = values.ThreeDigitNumeric;
        FourDigit = values.FourDigit;

        if (values.FoundMarkers.Contains("PNFV]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned Default Version As: {VersionLiteral}");
        }
        if (values.FoundMarkers.Contains("PNF2]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned Short Version As: {ShortVersion}");
        }
        if (values.FoundMarkers.Contains("PNFN]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned ReleaseName As: {ReleaseName}");
        }
        if (values.FoundMarkers.Contains("PNF3]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned ThreeDigit As: {ThreeDigit}");
        }
        if (values.FoundMarkers.Contains("PN4D]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned FourDigitNumeric As: {FourDigitNumeric}");
        }
        if (values.FoundMarkers.Contains("PNQF]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned Queued Full Version As: {QueuedFull}");
        }
        if (values.FoundMarkers.Contains("PN3D]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned ThreeDigitNumeric As: {ThreeDigitNumeric}");
        }
        if (values.FoundMarkers.Contains("PNF4]")) {
            GetLogger().Invoke(OutputType.Std, $"Versonify Returned FourDigit As:{FourDigit} ");
        }
    }

    internal static void ValidateVersionOutput(VersonifyCommand command, string versionLiteral, string output) {
        if (RequiresVersion(command) && string.IsNullOrWhiteSpace(versionLiteral)) {
            throw new InvalidOperationException(
                $"Versonify '{command}' completed successfully but did not return a PNFV] version value.{Environment.NewLine}" +
                $"Output:{Environment.NewLine}{output}");
        }
    }

    internal static InvalidOperationException CreateCommandFailureException(VersonifyCommand command, int exitCode, string output) => new(
        $"Versonify '{command}' failed with exit code {exitCode}.{Environment.NewLine}" +
        $"Output:{Environment.NewLine}{output}");

    internal static void ThrowIfCommandFailed(VersonifyCommand command, int exitCode, string output) {
        if (exitCode != 0) {
            throw CreateCommandFailureException(command, exitCode, output);
        }
    }

    private ArgumentStringHandler GetLegacyArguments(VersonifySettings settings, int compatibilityLevel) {
        GetLogger().Invoke(OutputType.Std, $"Warning.  Versonify is out of date, you should update your tools package. CL:{compatibilityLevel}");
        return settings.GetArgsString();
    }

    internal static bool RequiresVersion(VersonifyCommand command) =>
        command is VersonifyCommand.Passive or VersonifyCommand.UpdateFiles;

    private static string GetOutputText(IReadOnlyCollection<Output> output) =>
        string.Join(Environment.NewLine, output.Select(line => $"{line.Type}: {line.Text}"));

    public string ShortVersion { get; set; } = string.Empty;
    public string ReleaseName { get; set; } = string.Empty;
    public string VersionLiteral { get; set; } = string.Empty;
    public string FourDigitNumeric { get; set; } = string.Empty;
    public string ThreeDigit { get; set; } = string.Empty;
    public string QueuedFull { get; set; } = string.Empty;
    public string ThreeDigitNumeric { get; set; } = string.Empty;
    public string FourDigit { get; set; } = string.Empty;

    public IReadOnlyCollection<Output> RawExecute(VersonifySettings settings) {
        return ExecuteVersonify(settings);
    }

    public IReadOnlyCollection<Output> RawExecute(string commandLine) {
        return ExecuteVersonify(null, replaceCommandLine: commandLine);
    }


    public IReadOnlyCollection<Output> FileUpdateCommand(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return FileUpdateCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> FileUpdateCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.UpdateFiles);
    }

    public IReadOnlyCollection<Output> OverrideCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return OverrideCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> OverrideCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.Override);
    }

    public IReadOnlyCollection<Output> PassiveCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return PassiveCommand(configure(new VersonifySettings()));
    }

    private IReadOnlyCollection<Output> PassiveCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.Passive);
    }
}
