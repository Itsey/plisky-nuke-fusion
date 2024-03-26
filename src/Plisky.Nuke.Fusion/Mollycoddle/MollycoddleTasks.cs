namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using global::Nuke.Common.Tooling;

using Octokit;


public static class MollycoddleTasks {


    public static IReadOnlyCollection<Output> PerformScan(Configure<MollycoddleSettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return PerformScan(configure(new MollycoddleSettings()));
    }

    public static IReadOnlyCollection<Output> PerformScan(MollycoddleSettings settings) {

        settings = settings ?? new MollycoddleSettings();        
        using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());
        return process.Output;
    }

}
