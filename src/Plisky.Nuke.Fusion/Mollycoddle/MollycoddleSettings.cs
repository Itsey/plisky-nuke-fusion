namespace Plisky.Nuke.Fusion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using global::Nuke.Common.Tooling;

using Octokit;


[Serializable]
public class MollycoddleSettings : ToolSettings {
    public override Action<OutputType, string> ProcessLogger => base.ProcessLogger ?? ProcessTasks.DefaultLogger;
    public override Action<ToolSettings, IProcess> ProcessExitHandler => base.ProcessExitHandler ?? ProcessTasks.DefaultExitHandler;

    public bool Debug { get; set; }
    public bool DryRun { get; set; }

    public string TraceConfiguration { get; set; }
    
    public bool Disabled { get; set; }
    public string RulesetVersion { get; set; } = "default";

    public string Directory { get; set; }

    public bool RuleHelp { get; set; }

    public string Formatter { get; set; } = "plain";

    public string PrimaryRoot { get; set; }

    public string RulesFile { get; set; }

    public override string ProcessToolPath => GetPath();

    

    private string GetPath() {
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Mollycoddle",
          packageExecutable: "Mollycoddle.exe",
          framework: null);
    }


    protected override Arguments ConfigureProcessArguments(Arguments arguments) {
        

        arguments
          .Add($"-dir={Directory}")
          .Add($"-rulesFile={RulesFile}")
          .Add($"-formatter={Formatter}");


        if (!string.IsNullOrEmpty(RulesetVersion)) {
            arguments.Add($"-version={RulesetVersion}");
        }

        if (!string.IsNullOrEmpty(PrimaryRoot)) {
            arguments.Add($"-primaryRoot={PrimaryRoot}");
        }

        if (Debug) {
            arguments.Add("-Debug=v-**");
        }

        if (DryRun) {
            arguments.Add("-WarnOnly");
        }

        if (Disabled) {
            arguments.Add("-Disabled");
        }

        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            arguments.Add($"-Trace={TraceConfiguration}");
        }

        return base.ConfigureProcessArguments(arguments);
    }
}
