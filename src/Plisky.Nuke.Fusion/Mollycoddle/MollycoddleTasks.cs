namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using global::Nuke.Common.Tooling;


public class MollycoddleTasks : ToolTasks {


    public IReadOnlyCollection<Output> PerformScan(Configure<MollycoddleSettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return PerformScan(configure(new MollycoddleSettings()));
    }

    public IReadOnlyCollection<Output> PerformScan(MollycoddleSettings settings) {


        string tpth = settings.GetPath();
        SetToolPath(tpth);
        return Run(settings.GetArgsString());  //process.Output;
    }

}
