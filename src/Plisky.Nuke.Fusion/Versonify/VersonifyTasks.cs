namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using global::Nuke.Common.Tooling;





public class VersonifyTasks : ToolTasks, IRequirePathTool {

    public static IReadOnlyCollection<Output> Versonify(ArgumentStringHandler arguments, string workingDirectory = null, IReadOnlyDictionary<string, string> environmentVariables = null, int? timeout = null, bool? logOutput = null, bool? logInvocation = null, Action<OutputType, string> logger = null, Func<IProcess, object> exitHandler = null)
        => new VersonifyTasks().Run(arguments, workingDirectory, environmentVariables, timeout, logOutput, logInvocation, logger, exitHandler);

    public IReadOnlyCollection<Output> PerformFileUpdate(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return PerformFileUpdate(configure(new VersonifySettings()));
    }

    public IReadOnlyCollection<Output> PerformFileUpdate(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();
        settings.SetCommand(VersonifyCommand.UpdateFiles);
        settings.PerformIncrement = true;



        string tpth = settings.GetPath();
        SetToolPath(tpth);

        var result = Run(settings.GetArgsString());
        return result;
        /*using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());*/

    }

    public IReadOnlyCollection<Output> PassiveExecute(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return PassiveExecute(configure(new VersonifySettings()));
    }

    private IReadOnlyCollection<Output> PassiveExecute(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();

        string tpth = settings.GetPath();
        SetToolPath(tpth);

        settings.SetCommand(VersonifyCommand.Passive);

        var result = Run(settings.GetArgsString());
        return result;

        /*using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());
        var output = process.Output;

        return output;*/
    }
}
