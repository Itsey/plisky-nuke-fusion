using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Plisky.Diagnostics;
using Serilog;

public partial class Build : NukeBuild {
    protected Bilge b = new Bilge("Pnf-Build");

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Specifies a quick version command for the versioning quick step")]
    readonly string QuickVersion = "";

    [Parameter("Specifies a quick version command for the versioning quick step")]
    readonly string ReleaseName = "";

    [GitRepository]
    readonly GitRepository GitRepository;

    [Solution]
    Solution Solution;

    LocalBuildConfig settings;


    AbsolutePath SourceDirectory => RootDirectory / "src";

    public Target Wrapup => _ => _
      .DependsOn(Initialise)
      .After(Initialise)
      .Executes(() => {
          b.Info.Log("Build >> Wrapup >> All Done.");
          Log.Information("Build>Wrapup>  Finish - Build Process Completed.");
          b.Flush().Wait();
          System.Threading.Thread.Sleep(10);
      });

    Target Initialise => _ => _
      .Executes(() => {

          if (Solution == null) {
              Log.Error("Build>Initialise>Solution is null.");
              throw new InvalidOperationException("The solution must be set");
          }


          //Bilge.AddHandler(new TCPHandler("127.0.0.1", 9060, true));

          Bilge.SetConfigurationResolver((a, b) => {
              return System.Diagnostics.SourceLevels.Verbose;
          });

          b = new Bilge("Nuke", tl: System.Diagnostics.SourceLevels.Verbose);

          Bilge.Alert.Online("Versonify-Build");
          b.Info.Log("Versionify Build Process Initialised, preparing Initialisation section.");



          var ap = RootDirectory.Parent / "src"; // / "src_TestSolution/NukeTestSolution.sln";
          //ap = ap / "src_TestSolution";
          ap = ap / "Plisky.Nuke.Fusion.sln";

          Solution = ap.ReadSolution();

          settings = new LocalBuildConfig() {
              DependenciesDirectory = Solution.Projects.First(x => x.Name == "_Dependencies").Directory,
              ArtifactsDirectory = @"D:\Scratch\_build\vsfbld\",
              NonDestructive = false,
              MainProjectName = "Plisky.Nuke.Fusion",
              MollyPrimaryToken = "%NEXUSCONFIG%[R::plisky[L::http://51.141.43.222:8081/repository/plisky/primaryfiles/XXVERSIONNAMEXX/",
              MollyRulesToken = "%NEXUSCONFIG%[R::plisky[L::http://51.141.43.222:8081/repository/plisky/molly/XXVERSIONNAMEXX/defaultrules.mollyset",
              MollyRulesVersion = "default",
              VersioningPersistanceToken = @"%NEXUSCONFIG%[R::plisky[L::http://51.141.43.222:8081/repository/plisky/vstore/pnf-version.store"
          };


          if (settings.NonDestructive) {
              Log.Information("Initialised - In Non Destructive Mode.");
          } else {
              Log.Information("Initialised - In Destructive Mode.");
          }

      });

}
