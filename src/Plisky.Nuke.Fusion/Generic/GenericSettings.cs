namespace Plisky.Nuke.Fusion;

using System;
using global::Nuke.Common.Tooling;


[Serializable]
public class GenericSettings : ToolOptions {

    private string? ResolvedExePath { get; set; }

    public string? GenericExeName { get; set; }


    //public override string ProcessToolPath => GetPath();
    public bool TryAutoResolve { get; set; } = true;
    public string? DirectSetArguments { get; set; }
    public string? WorkingDirectory { get; set; }

    private string GetPath() {

        if (string.IsNullOrWhiteSpace(GenericExeName)) {
            throw new InvalidOperationException("GenericExeName must be set to the name of the executable to run.");
        }


        if (File.Exists(GenericExeName)) {
            ResolvedExePath = GenericExeName;
        } else {
            if (TryAutoResolve) {
                string sys32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string whereExe = Path.Combine(sys32, "where.exe");
                if (File.Exists(whereExe)) {
                    var p = ProcessTasks.StartProcess(whereExe, GenericExeName, sys32);
                    p.AssertWaitForExit();
                    if (p.Output.Count > 0) {

                        string possiblePath = p.Output.StdToText();
                        if (File.Exists(possiblePath)) {
                            ResolvedExePath = possiblePath;
                        }
                    }

                }
            }
        }
        return ResolvedExePath;
    }

    // public override string ProcessWorkingDirectory => GetWorkingDirectory();

    private string GetWorkingDirectory() {
        if (string.IsNullOrEmpty(WorkingDirectory)) {
            return base.ProcessWorkingDirectory;
        } else {
            return WorkingDirectory;
        }
    }


}
