
namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using global::Nuke.Common.Tooling;

public class VersonifyTasks : ToolTasks, IRequirePathTool {
    public VersonifyTasks() {
        this.GetLogger().Invoke(OutputType.Std,$"{PnfUtilities.GetPnfString()} [Versonify Tasks]");
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
        IReadOnlyCollection<Output> result;

        settings = settings ?? new VersonifySettings();

        if (command != VersonifyCommand.Unknown) {
            settings.SetCommand(command);
        }
        string tpth = settings.GetPath();
        SetToolPath(tpth);


        if (replaceCommandLine != null) {
            result = Run(replaceCommandLine);
        } else {
            result = Run(settings.GetArgsString());
        }

        string[] lines = result.StdToText().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string l in lines) {
            if (l.StartsWith("PNFV]")) {
                VersionLiteral = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned Default Version As: {VersionLiteral}");
            } else if (l.StartsWith("PNF2]")) {
                ShortVersion = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned Short Version As: {ShortVersion}");
            } else if (l.StartsWith("PNFN]")) {
                ReleaseName = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned ReleaseName As: {ReleaseName}");
            } else if (l.StartsWith("PNF3]")) {
                ThreeDigit = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned ThreeDigit As: {ThreeDigit}");
            } else if (l.StartsWith("PN4D]")) {
                FourDigitNumeric = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned FourDigitNumeric As: {FourDigitNumeric}");
            } else if (l.StartsWith("PNQF]")) {
                QueuedFull = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned Queued Full Version As: {QueuedFull}");
            } else if (l.StartsWith("PN3D]")) {
                ThreeDigitNumeric = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned ThreeDigitNumeric As: {ThreeDigitNumeric}");
            } else if (l.StartsWith("PNF4]")) {
                FourDigit = l.Substring(5);
                this.GetLogger().Invoke(OutputType.Std, $"Versonify Returned FourDigit As:{FourDigit} ");
            }

        }
        return result;
    }

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
