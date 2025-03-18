﻿
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Serilog;

public partial class Build : NukeBuild {

    // Examine is the well known step for post compilation, pre package and deploy. Arrange Construct [Examine] Package Release Test
    public Target ExamineStep => _ => _
        .After(ConstructStep)
        .Before(PackageStep, Wrapup)
        .DependsOn(Initialise, ConstructStep)
        .Triggers(UnitTest)
        .Executes(() => {
            Log.Information("--> Examine Step <-- ");
        });




    public Target UnitTest => _ => _
      .After(ExamineStep)
      .Before(PackageStep)
      .Executes(() => {
          if (Solution == null) {
              Log.Error("Build>UnitTest>Solution is null.");
              throw new InvalidOperationException("The solution must be set");
          }

          var testProjects = Solution.GetAllProjects("*.Test");
          if (testProjects.Any()) {
              DotNetTasks.DotNetTest(s => s
                  .EnableNoRestore()
                  .EnableNoBuild()
                  .SetConfiguration(Configuration)
                  .SetProjectFile(testProjects.First().Directory));
          }
      });
}

