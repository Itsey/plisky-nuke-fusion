
using System;
using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Serilog;

public partial class Build : NukeBuild {

    // Package Step - Well known step for bundling prior to the app release.   Arrange Construct Examine [Package] Release Test

    private Target PackageStep => _ => _
        .After(ExamineStep)
        .Before(ReleaseStep, Wrapup)
        .DependsOn(Initialise, ExamineStep)
        .Executes(() => {

            if (Solution == null) {
                Log.Error("Build>PackageStep>Solution is null.");
                throw new InvalidOperationException("The solution must be set");
            }

            if (settings == null) {
                Log.Error("Build>PackageStep>Settings is null.");
                throw new InvalidOperationException("The settings must be set");
            }


            var project = Solution.GetProject(settings.MainProjectName);

            if (project == null) { throw new InvalidOperationException($"Publish -> GetProject -> Project {settings.MainProjectName} was not found."); }

            var publishDirectory = settings.ArtifactsDirectory + "\\publish\\lib";
            var nugetStructure = settings.ArtifactsDirectory + "\\nuget";

            foreach (var l in new[] { "net8.0", "net9.0" }) {
                DotNetTasks.DotNetPublish(s => s
                  .SetProject(project)
                  .SetConfiguration(Configuration)
                  .SetOutput(Path.Combine(publishDirectory, l))
                  .SetFramework(l)
                  .EnableNoRestore()
                  .EnableNoBuild()
                );
            }
            //var readmeFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\readme.md";
            //var targetdir = nugetStructure + "\\readme.md";
            //publishDirectory.CopyToDirectory(nugetStructure, ExistsPolicy.MergeAndOverwrite);

            ////targetdir.Copy(targetdir, ExistsPolicy.FileOverwrite);
            //readmeFile.Copy(targetdir, ExistsPolicy.FileOverwrite);

            //var nugetPackageFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\nuke-fusion.nuspec";
            //var destDir = settings.ArtifactsDirectory + "\\nuke-fusion.nuspec";
            //nugetPackageFile.Copy(destDir, ExistsPolicy.FileOverwrite);



            //NuGetTasks.NuGetPack(s => s
            //  .SetTargetPath(settings.ArtifactsDirectory + "\\nuke-fusion.nuspec")
            //  .SetOutputDirectory(settings.ArtifactsDirectory));



            // var project = Solution.GetProject("Versonify");
            //if (project == null) { throw new InvalidOperationException("Project not found"); }

            //var publishDirectory = settings.ArtifactsDirectory + "\\publish\\";
            //var nugetStructure = settings.ArtifactsDirectory + "\\nuget";

            DotNetTasks.DotNetPack(s => s
              .SetProject(project)
              .SetConfiguration(Configuration)
              .SetOutputDirectory(nugetStructure)
              .EnableNoBuild()
              .EnableNoRestore()
            );

        });



}

