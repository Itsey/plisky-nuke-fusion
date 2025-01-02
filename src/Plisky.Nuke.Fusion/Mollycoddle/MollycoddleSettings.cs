namespace Plisky.Nuke.Fusion;

using System;
using global::Nuke.Common.Tooling;


[Serializable]
public class MollycoddleSettings : ToolOptions {

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




    public string GetPath() {
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Mollycoddle",
          packageExecutable: "Mollycoddle.exe",
          framework: null);
    }

#if true
    public ArgumentStringHandler GetArgsString() {
        var result = new ArgumentStringHandler();
        result.AppendLiteral($" -dir={Directory}");
        result.AppendLiteral($" -rulesFile={RulesFile}");
        result.AppendLiteral($" -formatter={Formatter}");

        if (!string.IsNullOrEmpty(RulesetVersion)) {
            result.AppendLiteral($" -version={RulesetVersion}");
        }
        if (!string.IsNullOrEmpty(PrimaryRoot)) {
            result.AppendLiteral($" -primaryRoot={PrimaryRoot}");
        }
        if (Debug) {
            result.AppendLiteral(" -Debug=v-**");
        }
        if (DryRun) {
            result.AppendLiteral(" -WarnOnly");
        }
        if (Disabled) {
            result.AppendLiteral(" -Disabled");
        }
        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            result.AppendLiteral($" -Trace={TraceConfiguration}");
        }
        return result;
    }
#else
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

#endif
    public MollycoddleSettings() : base() {
        // this.SetProcessToolPath(GetPath());
    }
}
