using System;
using System.Linq;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using Plisky.Nuke.Fusion;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

public partial class Build : NukeBuild {


    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository]
    readonly GitRepository GitRepository;

    [Solution]
    readonly Solution Solution;

    LocalBuildConfig settings;


    AbsolutePath SourceDirectory => RootDirectory / "src";
    //AbsolutePath ArtifactsDirectory => Path.GetTempPath() + "\\artifacts";


    Target Initialise => _ => _
      .Before(Prepare)
      .Executes(() => {
          settings = new LocalBuildConfig();
          settings.ArtifactsDirectory = @"D:\Scratch\_build\vsfbld\";
          settings.NonDestructive = false;
          settings.VersioningPersistanceToken = @"D:\Scratch\_build\vstore\pfn-version.vstore";
          settings.MainProjectName = "Plisky.Nuke.Fusion";
          

          if (settings.NonDestructive) {
              Logger.Info("Initialised - In Non Destructive Mode.");
          } else {
              Logger.Info("Initialised - In Destructive Mode.");
          }
          
      });


    Target Prepare => _ => _
       .Executes(() => {
       });


    #region Build Related Targets
    Target VersionSource => _ => _
       .DependsOn(Initialise)
       .Before(Compile)
       .After(Clean)
       .Executes(() => {

           if (settings == null) {
               throw new InvalidOperationException("Initialisation Has Not Occured - must run the settings initialisation.");
           }

           bool useDryRun = false;
           if (IsLocalBuild) {
               Logger.Info("Local build, using dry run verisoning.");
               useDryRun = settings.DryRunIfLocal;
           }

           VersonifyTasks.PassiveExecute(s => s
             .SetRoot(Solution.Directory)
             .SetVersionPersistanceValue(settings.VersioningPersistanceToken));

           VersonifyTasks.PerformFileUpdate(s => s
            .SetRoot(Solution.Directory)
            .AddMultimatchFile($"{Solution.Directory}\\_Dependencies\\Automation\\AutoVersion.txt")
            .PerformIncrement(true)
            .SetVersionPersistanceValue(settings.VersioningPersistanceToken)
            .AsDryRun(useDryRun)
            .SetRelease(""));

           // Logger.Info($"Version is {version}");
       });


    #endregion


    Target Restore => _ => _
        .Executes(() => {
        });


    Target Compile => _ => _
        .Triggers(UnitTest)
        .DependsOn(Initialise, VersionSource, Clean)
        .Executes(() => {

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetDeterministic(IsServerBuild)
                .SetContinuousIntegrationBuild(IsServerBuild));
        });



    Target UnitTest => _ => _
       .After(Compile)
       .Before(Release)
       .DependsOn(Compile)
       .Executes(() => {
           var testProjects = Solution.GetAllProjects("*.Test");
           if (testProjects.Any()) {
               DotNetTasks.DotNetTest(s => s
                   .EnableNoRestore()
                   .EnableNoBuild()
                   .SetProjectFile(testProjects.First().Directory));
           }
       });



    Target Publish => _ => _
        .DependsOn(Compile)
        .After(Validate)
        .Executes(() => {


            var project = Solution.GetProject(settings.MainProjectName);

            if (project == null) { throw new InvalidOperationException($"Publish -> GetProject -> Project {settings.MainProjectName} was not found."); }

            var publishDirectory = settings.ArtifactsDirectory + "\\publish\\pnf";
            var nugetStructure = settings.ArtifactsDirectory + "\\nuget";

            DotNetTasks.DotNetPublish(s => s
              .SetProject(project)
              .SetConfiguration(Configuration)
              .SetOutput(publishDirectory)
              .EnableNoRestore()
              .EnableNoBuild()
            );

            string readmeFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\readme.md";
            string targetdir = nugetStructure + "\\readme.md";
            FileSystemTasks.CopyFile(readmeFile, targetdir, FileExistsPolicy.Overwrite);

            string nugetPackageFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\nuke-fusion.nuspec";
            FileSystemTasks.CopyFile(nugetPackageFile, settings.ArtifactsDirectory + "\\nuke-fusion.nuspec", FileExistsPolicy.Overwrite);

        });

    Target BuildNugetPackage => _ => _
       .DependsOn(Publish)
       .Executes(() => {
           VersonifyTasks.PassiveExecute(s => s
             .SetRoot(Solution.Directory)
             .SetVersionPersistanceValue(settings.VersioningPersistanceToken)
             .SetDebug(true));

           VersonifyTasks.PerformFileUpdate(s => s
            .SetRoot(settings.ArtifactsDirectory)
            .AddMultimatchFile($"{Solution.Directory}\\_Dependencies\\Automation\\NuspecVersion.txt")
            .SetVersionPersistanceValue(settings.VersioningPersistanceToken));

           NuGetTasks.NuGetPack(s => s
             .SetTargetPath(settings.ArtifactsDirectory + "\\Nuke-Fusion.nuspec")
             .SetOutputDirectory(settings.ArtifactsDirectory));


       });

    Target ReleaseNugetPackage => _ => _
     .DependsOn(BuildNugetPackage)
     .Executes(() => {

         if (settings.NonDestructive) {
             Logger.Info("Non destructive, skipping release");
             return;
         }

         NuGetTasks.NuGetPush(s => s
             .SetTargetPath(settings.ArtifactsDirectory + "\\Plisky.Nuke.Fusion*.nupkg")
             .SetSource("https://api.nuget.org/v3/index.json")
             .SetApiKey(Environment.GetEnvironmentVariable("PLISKY_PUBLISH_KEY")));
     });



    Target Validate => _ => _
       .DependsOn(Initialise)
       .After(Compile)
       .Executes(() => {
           Logger.Info("--> Validate <-- ");

       });

    Target Release => _ => _
       .DependsOn(ReleaseNugetPackage)
       .After(Validate)
       .Executes(() => {
           Logger.Log(LogLevel.Normal, "Release Complete.");
       });
}
