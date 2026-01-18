using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.PowerShell;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Serilog;

public partial class Build : NukeBuild {
    protected Bilge b = new Bilge("Pnf-Build");

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Specifies a quick version command for the versioning quick step")]
    private readonly string QuickVersion = "";

    [Parameter("Specifies a quick version command for the versioning quick step")]
    private readonly string ReleaseName = "";

    [Parameter("PreRelease will only release a pre-release verison of the package.  Uses pre-release versioning.")]
    private readonly bool PreRelease = true;

    [GitRepository]
    private readonly GitRepository GitRepository;

    [Solution]
    private Solution Solution;

    private LocalBuildConfig settings;

    private AbsolutePath SourceDirectory => RootDirectory / "src";

    public Target Wrapup => _ => _
      .DependsOn(Initialise)
      .After(Initialise)
      .Executes(() => {
          b.Info.Log("Build >> Wrapup >> All Done.");
          Log.Information("Build>Wrapup>  Finish - Build Process Completed.");
          b.Flush().Wait();
          System.Threading.Thread.Sleep(10);
      });

    public Target NexusLive => _ => _
     .After(Initialise)
     .DependsOn(Initialise)
     .Executes(() => {
         string? dotb = Environment.GetEnvironmentVariable("DOTB_BUILDTOOLS");
         if (!string.IsNullOrWhiteSpace(dotb)) {
             Log.Information($"Build> Ensure Nexus Is Live>  Build Tools Directory: {dotb}");

             string nexusInitScript = Path.Combine(dotb, "scripts", "nexusInit.ps1");
             if (File.Exists(nexusInitScript)) {
                 PowerShellTasks.PowerShell(x =>
                    x.SetFile(nexusInitScript)
                    .SetFileArguments("checkup")
                    .SetProcessToolPath("pwsh")
                 );
             } else {
                 Log.Error($"Build>Initialise>  Build Tools Directory: {nexusInitScript} - Nexus Init Script not found.");
             }
         } else {
             Log.Information("Build>Initialise>  Build Tools Directory: Not Set, no additional initialisation taking place.");
         }
     });

    private Target Initialise => _ => _
      .Executes(() => {
          if (Solution == null) {
              Log.Error("Build>Initialise>Solution is null.");
              throw new InvalidOperationException("The solution must be set");
          }

          var th = new TCPHandler("127.0.0.1", 9060, true);
          th.SetFormatter(new FlimFlamV4Formatter());
          Bilge.AddHandler(th);
          Bilge.SetConfigurationResolver((a, b) => {
              return System.Diagnostics.SourceLevels.Verbose;
          });

          b = new Bilge("Nuke", tl: System.Diagnostics.SourceLevels.Verbose);

          Bilge.Alert.Online("Pnf-Build");
          b.Info.Log("Pnf Build Process Initialised, preparing Initialisation section.");

          var ap = RootDirectory.Parent / "src";
          ap = ap / "Plisky.Nuke.Fusion.sln";

          Solution = ap.ReadSolution();

          settings = new LocalBuildConfig() {
              DependenciesDirectory = Solution.Projects.First(x => x.Name == "_Dependencies").Directory,
              ArtifactsDirectory = Path.Combine(Path.GetTempPath(), "_build\\pnfbld\\"),
              NonDestructive = false,
              MainProjectName = "Plisky.Nuke.Fusion",
              MollyPrimaryToken = "%NEXUSCONFIG%[R::plisky[L::https://pliskynexus.yellowwater-365987e0.uksouth.azurecontainerapps.io/repository/plisky/primaryfiles/XXVERSIONNAMEXX/",
              MollyRulesToken = "%NEXUSCONFIG%[R::plisky[L::https://pliskynexus.yellowwater-365987e0.uksouth.azurecontainerapps.io/repository/plisky/molly/XXVERSIONNAMEXX/defaultrules.mollyset",
              MollyRulesVersion = "default",
              VersioningPersistanceToken = @"%NEXUSCONFIG%[R::plisky[L::https://pliskynexus.yellowwater-365987e0.uksouth.azurecontainerapps.io/repository/plisky/vstore/pnf.vstore",
              VersioningPreReleasePersistanceToken = @"%NEXUSCONFIG%[R::plisky[L::https://pliskynexus.yellowwater-365987e0.uksouth.azurecontainerapps.io/repository/plisky/vstore/pnf-pre.vstore"
          };

          if (settings.NonDestructive) {
              Log.Information("Initialised - In Non Destructive Mode.");
          } else {
              Log.Information("Initialised - In Destructive Mode.");
          }
      });
}