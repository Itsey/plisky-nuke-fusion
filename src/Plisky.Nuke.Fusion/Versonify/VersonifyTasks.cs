
namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using global::Nuke.Common.Tooling;

public class VersonifyTasks : ToolTasks, IRequirePathTool {

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

        this.GetLogger().Invoke(OutputType.Std, "Versioning By Versonify!");


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
            }
        }
        return result;
    }

    public string ShortVersion { get; set; } = string.Empty;
    public string ReleaseName { get; set; } = string.Empty;
    public string VersionLiteral { get; set; } = string.Empty;

    public IReadOnlyCollection<Output> RawExecute(VersonifySettings settings) {
        return ExecuteVersonify(settings);
        //var x = 
        //x.Wait();
        //return x.Result;
    }

    public IReadOnlyCollection<Output> RawExecute(string commandLine) {
        return ExecuteVersonify(null, replaceCommandLine: commandLine);

        //var x = 
        //x.Wait();
        //return x.Result;
        //return (IReadOnlyCollection<Output>);
    }


    public IReadOnlyCollection<Output> FileUpdateCommand(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return FileUpdateCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> FileUpdateCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.UpdateFiles);
        //var x = 
        //x.Wait();
        //return x.Result;

        //return (IReadOnlyCollection<Output>)

        //settings = settings ?? new VersonifySettings();


        //string tpth = settings.GetPath();
        //SetToolPath(tpth);

        //var result = Run(settings.GetArgsString());
        //return result;

    }

    public IReadOnlyCollection<Output> OverrideCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return OverrideCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> OverrideCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.Override);
        //var x = 
        //x.Wait();
        //return x.Result;

        //return (IReadOnlyCollection<Output>)
        //settings = settings ?? new VersonifySettings();
        //settings.SetCommand(VersonifyCommand.Override);
        //string tpth = settings.GetPath();
        //SetToolPath(tpth);
        //var result = Run(settings.GetArgsString());
        //return result;
    }

    public IReadOnlyCollection<Output> PassiveCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return PassiveCommand(configure(new VersonifySettings()));
    }

    private IReadOnlyCollection<Output> PassiveCommand(VersonifySettings settings) {
        return ExecuteVersonify(settings, VersonifyCommand.Passive);
        //var x = 
        //x.Wait();
        //return x.Result;
        //return (IReadOnlyCollection<Output>);

        //settings = settings ?? new VersonifySettings();

        //string tpth = settings.GetPath();
        //SetToolPath(tpth);

        //settings.SetCommand(VersonifyCommand.Passive);

        //var result = Run(settings.GetArgsString());
        //return result;

    }
}
