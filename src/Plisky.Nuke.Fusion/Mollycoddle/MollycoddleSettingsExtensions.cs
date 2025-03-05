namespace Plisky.Nuke.Fusion;
public static partial class MollycoddleSettingsExtensions {


    public static T SetDisabled<T>(this T toolSettings, bool doNotOverride) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Disabled = doNotOverride;
        return toolSettings;
    }

    public static T SetDebug<T>(this T toolSettings, bool debugValue) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Debug = debugValue;
        return toolSettings;
    }

    public static T AsDryRun<T>(this T toolSettings, bool dryRunOnly) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.DryRun = dryRunOnly;
        return toolSettings;
    }

    public static T AddRuleHelp<T>(this T toolSettings, bool shouldAddRuleHelp) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.RuleHelp = shouldAddRuleHelp;
        return toolSettings;
    }

    public static T AddRulesetVersion<T>(this T toolSettings, string rulesetVersionName) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.RulesetVersion = rulesetVersionName;
        return toolSettings;
    }

    public static T SetDirectory<T>(this T toolSettings, string directoryToScan) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Directory = directoryToScan;
        return toolSettings;
    }

    public static T SetRulesFile<T>(this T toolSettings, string directoryToScan) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.RulesFile = directoryToScan;
        return toolSettings;
    }

    public static T SetPrimaryRoot<T>(this T toolSettings, string primaryFileRootPath) where T : MollycoddleSettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.PrimaryRoot = primaryFileRootPath;
        return toolSettings;
    }

    public static T SetFormatter<T>(this T toolSettings, string newFormatterValue) where T : MollycoddleSettings {
        newFormatterValue = newFormatterValue.ToLower();
        switch (newFormatterValue) {
            case "plain":
            case "azdo": break;
            default:
                throw new InvalidOperationException("Formatter must be either 'plain' or 'azdo'");
        }

        // toolSettings = toolSettings.NewInstance();
        toolSettings.Formatter = newFormatterValue;
        return toolSettings;
    }

}
