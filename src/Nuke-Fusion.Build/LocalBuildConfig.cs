﻿using Nuke.Common.IO;

public class LocalBuildConfig {
    public AbsolutePath ArtifactsDirectory { get; set; }
    public bool NonDestructive { get; set; } = true;
    public string VersioningPersistanceToken { get; set; }
    public string MainProjectName { get; internal set; }

    public bool DryRunIfLocal { get; set; }
    public string NotifyWebhookUrl { get; internal set; }

    public AbsolutePath DependenciesDirectory { get; set; }
    public string MollyRulesToken { get; set; }
    public string MollyPrimaryToken { get; set; }
    public string MollyRulesVersion { get; set; }

}