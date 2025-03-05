
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

    public IReadOnlyCollection<Output> RawExecute(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();

        string tpth = settings.GetPath();
        SetToolPath(tpth);

        var result = Run(settings.GetArgsString());
        return result;
    }

    public IReadOnlyCollection<Output> RawExecute(string commandLine) {
        var settings = new VersonifySettings();
        string tpth = settings.GetPath();
        SetToolPath(tpth);

        var result = Run(commandLine);
        return result;
    }


    public IReadOnlyCollection<Output> FileUpdateCommand(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return FileUpdateCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> FileUpdateCommand(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();
        settings.SetCommand(VersonifyCommand.UpdateFiles);

        string tpth = settings.GetPath();
        SetToolPath(tpth);

        var result = Run(settings.GetArgsString());
        return result;

    }

    public IReadOnlyCollection<Output> OverrideCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return OverrideCommand(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> OverrideCommand(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();
        settings.SetCommand(VersonifyCommand.Override);
        string tpth = settings.GetPath();
        SetToolPath(tpth);
        var result = Run(settings.GetArgsString());
        return result;
    }

    public IReadOnlyCollection<Output> PassiveCommand(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return PassiveCommand(configure(new VersonifySettings()));
    }

    private IReadOnlyCollection<Output> PassiveCommand(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();

        string tpth = settings.GetPath();
        SetToolPath(tpth);

        settings.SetCommand(VersonifyCommand.Passive);

        var result = Run(settings.GetArgsString());
        return result;

    }
}
