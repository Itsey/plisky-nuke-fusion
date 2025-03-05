using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Plisky.Nuke.Fusion;
using Serilog;

public partial class Build : NukeBuild {
    public Target Clean => _ => _
     .DependsOn(Initialise)
     .Before(Prepare)
     .Executes(() => {

         DotNetTasks.DotNetClean(s => s
          .SetProject(Solution));

         settings.ArtifactsDirectory.CreateOrCleanDirectory();
     });


    public Target MollyCheck => _ => _
       .DependsOn(Initialise)
       .Before(Prepare)
       .After(Clean)
       .Executes(() => {

           Log.Information("Mollycoddle Structure Linting.");

           var mct = new MollycoddleTasks();

           mct.PerformScan(s => s
               .AddRuleHelp(true)
               .SetDebug(true)
               .SetRulesFile(@"C:\files\code\git\mollycoddle\src\_Dependencies\RulesFiles\XXVERSIONNAMEXX\defaultrules.mollyset")
               .SetPrimaryRoot(@"C:\Files\OneDrive\Dev\PrimaryFiles")
               .SetDirectory(GitRepository.LocalDirectory));

       });
}
