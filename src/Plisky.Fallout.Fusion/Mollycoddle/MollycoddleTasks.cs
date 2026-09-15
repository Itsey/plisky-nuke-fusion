namespace Plisky.Fallout.Fusion;

using System;
using System.Collections.Generic;
using global::Fallout.Common.Tooling;


public class MollycoddleTasks : ToolTasks {
    public MollycoddleTasks() {
        this.GetLogger().Invoke(OutputType.Std, $"{PffUtilities.GetPffString()} [Mollycoddle Tasks]");
    }

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
