using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Plisky.Nuke.Fusion;
using Serilog;

public partial class Build : NukeBuild {


    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository]
    readonly GitRepository GitRepository;

    [Solution]
    Solution Solution;

    LocalBuildConfig settings;


    AbsolutePath SourceDirectory => RootDirectory / "src";
    //AbsolutePath ArtifactsDirectory => Path.GetTempPath() + "\\artifacts";


    Target Initialise => _ => _
      .Executes(() => {

          var ap = RootDirectory.Parent / "src_TestSolution"; // / "src_TestSolution/NukeTestSolution.sln";
          //ap = ap / "src_TestSolution";
          ap = ap / "NukeTestSolution.sln";

          Solution = ap.ReadSolution();


          settings = new LocalBuildConfig();
          settings.NotifyWebhookUrl = "https://discord.com/api/webhooks/1323758419500339261/lVkTpOPOWBNp15KVOD_yBWVbiBlzQvjShGJdhHIP1W1Vm9A5lLk7pM1EHvYyjijuJwJQ";
          settings.ArtifactsDirectory = @"D:\Scratch\_build\vsfbld\";
          settings.NonDestructive = false;
          settings.VersioningPersistanceToken = @"D:\Scratch\_build\vstore\pnf-testsln-version.vstore";
          settings.MainProjectName = "NukeTestSolution";


          var d = new DiscordTasks();
          var ds = new DiscordSettings();
          ds.WebHookUrl = settings.NotifyWebhookUrl;
          d.SendNotification(ds, "Build Started");


          if (settings.NonDestructive) {
              Log.Information("Initialised - In Non Destructive Mode.");
          }
          else {
              Log.Information("Initialised - In Destructive Mode.");
          }

      });


    public Target Prepare => _ => _
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


    #endregion


    Target Restore => _ => _
        .Executes(() => {
        });


    public Target Compile => _ => _
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

            var readmeFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\readme.md";
            var targetdir = nugetStructure + "\\readme.md";

            targetdir.Copy(targetdir, ExistsPolicy.FileOverwrite);
            //FileSystemTasks.CopyFile(readmeFile, , FileExistsPolicy.Overwrite);

            var nugetPackageFile = Solution.GetProject("_Dependencies").Directory + "\\packaging\\nuke-fusion.nuspec";
            //FileSystemTasks.CopyFile(nugetPackageFile, settings.ArtifactsDirectory + "\\nuke-fusion.nuspec", FileExistsPolicy.Overwrite);
            var destDir = settings.ArtifactsDirectory + "\\nuke-fusion.nuspec";
            nugetPackageFile.Copy(destDir, ExistsPolicy.FileOverwrite);

        });

    Target BuildNugetPackage => _ => _
       .DependsOn(Publish)
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

    Target ReleaseNugetPackage => _ => _
     .DependsOn(BuildNugetPackage)
     .Executes(() => {

         if (settings.NonDestructive) {
             Log.Information("Non destructive, skipping release");
             return;
         }

         throw new NotImplementedException();
         NuGetTasks.NuGetPush(s => s
             .SetTargetPath(settings.ArtifactsDirectory + "\\Plisky.Nuke.Fusion*.nupkg")
             .SetSource("https://api.nuget.org/v3/index.json")
             .SetApiKey(Environment.GetEnvironmentVariable("PLISKY_PUBLISH_KEY")));
     });



    Target Validate => _ => _
       .DependsOn(Initialise)
       .After(Compile)
       .Executes(() => {
           Log.Information("--> Validate <-- ");

       });

    Target Release => _ => _
       .DependsOn(ReleaseNugetPackage)
       .After(Validate)
       .Executes(() => {
           Log.Information("Release Complete.");
       });
}
