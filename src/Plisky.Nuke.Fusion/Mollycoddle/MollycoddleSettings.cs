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

    public string Formatter { get; set; } = "default";

    public string PrimaryRoot { get; set; }

    public string RulesFile { get; set; }




    public string GetPath() {
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Mollycoddle",
          packageExecutable: "Mollycoddle.exe|mollycoddle.dll",
          framework: "9.0");
    }


    public ArgumentStringHandler GetArgsString() {
        var result = new ArgumentStringHandler(0, 0, out _);
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

    public MollycoddleSettings() : base() {
        // this.SetProcessToolPath(GetPath());
    }
}
