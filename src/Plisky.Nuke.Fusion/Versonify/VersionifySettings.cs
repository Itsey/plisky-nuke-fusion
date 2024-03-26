using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plisky.Nuke.Fusion;

[Serializable]
public class VersonifySettings : ToolSettings {
    public override Action<OutputType, string> ProcessLogger => base.ProcessLogger ?? ProcessTasks.DefaultLogger;
    public override Action<ToolSettings, IProcess> ProcessExitHandler => base.ProcessExitHandler ?? ProcessTasks.DefaultExitHandler;

    public string TraceConfiguration { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Debug in the Versonify tool.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Corresponds to -NoOverride in the Versonify tool.
    /// </summary>
    public bool NoOverride { get; set; }

    /// <summary>
    /// Corresponds to -Digits in the Versonify tool.
    /// </summary>
    public string Digits { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -MM in the Versonify tool.
    /// </summary>
    public string MultiMatchFile { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Increment in the Versonify tool.
    /// </summary>
    public bool PerformIncrement { get; set; }

    /// <summary>
    /// Corresponds to -Q in the Versonify tool.
    /// </summary>
    public string QuickValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -Release in the Versonify tool.
    /// </summary>
    public string Release { get; set; } = string.Empty;
    
    /// <summary>
    /// Corresponds to -Root in the Versonify tool.
    /// </summary>
    public string Root { get; set; } = string.Empty;


    public string VersionPersistanceValue { get; set; } = string.Empty;

    /// <summary>
    /// Corresponds to -MM in the Versonify tool.
    /// </summary>
    public string[] VersionTargetMinMatch { get; set; } = new string[0];

    /// <summary>
    /// Corresponds to -Command in the Versonify tool.
    /// </summary>
    public string Command { get; set; } = string.Empty;

    public void SetCommand(VersonifyCommand cmd) {
        Command = "-Command=" + cmd.ToString();

    }

    public override string ProcessToolPath => GetPath();

    public bool DryRun { get; set; }

    private string GetPath() {
        return NuGetToolPathResolver.GetPackageExecutable(
          packageId: "Plisky.Versonify",
          packageExecutable: "Versonify.exe",
          framework: null);
    }


    protected override Arguments ConfigureProcessArguments(Arguments arguments) {
        arguments
          .Add(Command)
          .Add($"-vs={VersionPersistanceValue}")
          .Add($"-Root={Root}");

        if (!string.IsNullOrEmpty(QuickValue)) {
            arguments.Add($"-Q={QuickValue}");
        }

        if (!string.IsNullOrEmpty(MultiMatchFile)) {
            arguments.Add($"-mm={MultiMatchFile}");
        }

        if (Debug) {
            arguments.Add("-Debug");
        }

        if (DryRun) {
            arguments.Add("-DryRun");
        }

        if (PerformIncrement) {
            arguments.Add("-Increment");
        }

        if (!string.IsNullOrEmpty(TraceConfiguration)) {
            arguments.Add($"-Trace={TraceConfiguration}");
        }

        return base.ConfigureProcessArguments(arguments);
    }
}
