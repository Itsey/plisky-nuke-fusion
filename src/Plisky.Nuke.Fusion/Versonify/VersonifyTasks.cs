namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using global::Nuke.Common.Tooling;
using Octokit;





public static class VersonifyTasks {

    public static IReadOnlyCollection<Output> PerformFileUpdate(Configure<VersonifySettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return PerformFileUpdate(configure(new VersonifySettings()));
    }

    public static IReadOnlyCollection<Output> PerformFileUpdate(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();
        settings.SetCommand(VersonifyCommand.UpdateFiles);
        settings.PerformIncrement = true;
        using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());
        return process.Output;
    }

    public static IReadOnlyCollection<Output> PassiveExecute(Configure<VersonifySettings> configure) {
        if (configure == null) {
            throw new InvalidOperationException();
        }
        return PassiveExecute(configure(new VersonifySettings()));
    }

    private static IReadOnlyCollection<Output> PassiveExecute(VersonifySettings settings) {
        settings = settings ?? new VersonifySettings();

        settings.SetCommand(VersonifyCommand.Passive);

        using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());
        var output = process.Output;

        return output;
    }
}
