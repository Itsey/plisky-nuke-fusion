﻿

using System;
using Nuke.Common;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.NuGet;
using Serilog;

public partial class Build : NukeBuild {

    public Target ApplyGitTag => _ => _
      .After(ReleaseStep)
      .DependsOn(Initialise)
      .Before(Wrapup)
      .Executes(() => {

          if (settings.NonDestructive) {
              Log.Information("Non destructive, skipping release");
              return;
          }

          if (IsSucceeding) {
              if (string.IsNullOrEmpty(FullVersionNumber)) {
                  Log.Information("No version number, skipping Tag");
              } else {
                  Log.Information("Applying Git Tag");
                  GitTasks.Git($"tag -a {FullVersionNumber} -m \"Release {FullVersionNumber}\"");
              }
          }
      });





    // Well known step for releasing into the selected environment.  Arrange Construct Examine Package [Release] Test
    public Target ReleaseStep => _ => _
      .DependsOn(Initialise, PackageStep)
      .Before(TestStep, Wrapup)
      .Triggers(ApplyGitTag)
      .After(PackageStep)
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

