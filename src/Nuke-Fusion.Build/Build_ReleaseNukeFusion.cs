using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Plisky.Nuke.Fusion;
using Serilog;

public partial class Build : NukeBuild {
    public Target ReleaseUpdatedNukeFusion => _ => _
     .DependsOn(Initialise)
     .Before(Prepare)
     .Triggers(PnfVersion, PnfCompile, PnfPublish)
     .Executes(() => {


         var ap = RootDirectory / "Plisky.Nuke.Fusion.sln";
         Solution = ap.ReadSolution();

         //ap = ap / "NukeTestSolution.sln";

         DotNetTasks.DotNetClean(s => s
          .SetProject(Solution));

         settings.ArtifactsDirectory.CreateOrCleanDirectory();
         settings.MainProjectName = "Plisky.Nuke.Fusion";

     });

    Target PnfVersion => _ => _
       .DependsOn(Initialise, ReleaseUpdatedNukeFusion)
       .Before(PnfCompile)
       .Executes(() => {

           if (settings == null) {
               throw new InvalidOperationException("Initialisation Has Not Occured - must run the settings initialisation.");
           }

           bool useDryRun = false;
           if (IsLocalBuild) {
               Log.Information("Local build, using dry run verisoning.");
               useDryRun = settings.DryRunIfLocal;
           }

           var vft = new VersonifyTasks();

           vft.PassiveCommand(s => s
             .SetRoot(Solution.Directory)
             .SetVersionPersistanceValue(settings.VersioningPersistanceToken));

           vft.FileUpdateCommand(s => s
            .SetRoot(Solution.Directory)
            .AddMultimatchFile($"{Solution.Directory}\\_Dependencies\\Automation\\AutoVersion.txt")
            .PerformIncrement(true)
            .SetVersionPersistanceValue(settings.VersioningPersistanceToken)
            .AsDryRun(useDryRun)
            .SetRelease(""));

           //Log.Information($"Version is {version}");
       });



    public Target PnfCompile => _ => _
        .DependsOn(Initialise, PnfVersion)
        .Triggers(PnfUnitTest)
        .Executes(() => {

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetDeterministic(IsServerBuild)
                .SetContinuousIntegrationBuild(IsServerBuild));
        });



    Target PnfUnitTest => _ => _
       .DependsOn(PnfCompile)
       .Executes(() => {
           var testProjects = Solution.GetAllProjects("*.Test");
           if (testProjects.Any()) {
               DotNetTasks.DotNetTest(s => s
                   .EnableNoRestore()
                   .EnableNoBuild()
                   .SetProjectFile(testProjects.First().Directory));
           }
       });



    Target PnfPublish => _ => _
        .DependsOn(PnfCompile)
        .After(PnfUnitTest)
    .Triggers(PnfBuildNugetPackage)
        .Executes(() => {


            var project = Solution.GetProject(settings.MainProjectName);

            if (project == null) { throw new InvalidOperationException($"Publish -> GetProject -> Project {settings.MainProjectName} was not found."); }

            var publishDirectory = settings.ArtifactsDirectory + "\\publish\\pnf";
            publishDirectory.CreateOrCleanDirectory();

            var nugetStructure = settings.ArtifactsDirectory + "\\nuget";
            nugetStructure.CreateOrCleanDirectory();

            DotNetTasks.DotNetPublish(s => s
              .SetProject(project)
              .SetConfiguration(Configuration)
              .SetOutput(publishDirectory)
              .EnableNoRestore()
              .EnableNoBuild()
            );

            var readmeFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\readme.md";
            var destinationReadmeFile = nugetStructure + "\\readme.md";

            readmeFile.Copy(destinationReadmeFile, ExistsPolicy.FileOverwrite);
            //FileSystemTasks.CopyFile(readmeFile, , FileExistsPolicy.Overwrite);

            var nugetPackageFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\nuke-fusion.nuspec";
            //FileSystemTasks.CopyFile(nugetPackageFile, settings.ArtifactsDirectory + "\\nuke-fusion.nuspec", FileExistsPolicy.Overwrite);
            var destDir = settings.ArtifactsDirectory + "\\nuke-fusion.nuspec";
            nugetPackageFile.Copy(destDir, ExistsPolicy.FileOverwrite);

        });

    Target PnfBuildNugetPackage => _ => _
       .DependsOn(PnfPublish)
       .Executes(() => {

           var vft = new VersonifyTasks();

           vft.PassiveCommand(s => s
             .SetRoot(Solution.Directory)
             .SetVersionPersistanceValue(settings.VersioningPersistanceToken)
             .SetDebug(true));

           vft.FileUpdateCommand(s => s
            .SetRoot(settings.ArtifactsDirectory)
            .AddMultimatchFile($"{Solution.Directory}\\_Dependencies\\Automation\\NuspecVersion.txt")
            .SetVersionPersistanceValue(settings.VersioningPersistanceToken));

           NuGetTasks.NuGetPack(s => s
             .SetTargetPath(settings.ArtifactsDirectory + "\\Nuke-Fusion.nuspec")
             .SetOutputDirectory(settings.ArtifactsDirectory));


       });

    public Target PnfReleaseNugetPackage => _ => _
     .DependsOn(PnfBuildNugetPackage)
     .Executes(() => {

         if (settings.NonDestructive) {
             Log.Information("Non destructive, skipping release");
             return;
         }


         NuGetTasks.NuGetPush(s => s
             .SetTargetPath(settings.ArtifactsDirectory + "\\Plisky.Nuke.Fusion*.nupkg")
             .SetSource("https://api.nuget.org/v3/index.json")
             .SetApiKey(Environment.GetEnvironmentVariable("PLISKY_PUBLISH_KEY")));
     });


}