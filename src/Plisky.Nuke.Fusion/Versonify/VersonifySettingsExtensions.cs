namespace Plisky.Nuke.Fusion;
public static partial class VersonifySettingsExtensions {


    public static T SetNoOverride<T>(this T toolSettings, bool doNotOverride) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.NoOverride = doNotOverride;
        return toolSettings;
    }

    public static T AddMultimatchFile<T>(this T toolSettings, string filename) where T : VersonifySettings {
        // toolSettings = toolSettings.NewInstance();
        toolSettings.MultiMatchFile = filename;
        return toolSettings;
    }

    public static T AsDryRun<T>(this T toolSettings, bool dryRunOnly) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.DryRun = dryRunOnly;
        return toolSettings;
    }

    public static T PerformIncrement<T>(this T toolSettings, bool doIncrement) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.PerformIncrement = doIncrement;
        return toolSettings;
    }

    public static T SetDebug<T>(this T toolSettings, bool setDebug) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Debug = setDebug;
        return toolSettings;
    }

    public static T SetTrace<T>(this T toolSettings, string newTraceValue) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.TraceConfiguration = newTraceValue;
        return toolSettings;
    }

    public static T SetRoot<T>(this T toolSettings, string newRootValue) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Root = newRootValue;
        return toolSettings;
    }

    public static T SetRelease<T>(this T toolSettings, string newReleaseValue) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.Release = newReleaseValue;
        return toolSettings;
    }

    public static T SetVersionPersistanceValue<T>(this T toolSettings, string newVersionPersistanceValue) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.VersionPersistanceValue = newVersionPersistanceValue;
        return toolSettings;
    }

    public static T SetQuickValue<T>(this T toolSettings, string newQuickValue) where T : VersonifySettings {
        // toolSettings = toolSettings.NewInstance();
        toolSettings.QuickValue = newQuickValue;
        return toolSettings;
    }


    public static T SetOutputStyle<T>(this T toolSettings, string outputStyle) where T : VersonifySettings {
        //toolSettings = toolSettings.NewInstance();
        toolSettings.OutputStyle = outputStyle;
        return toolSettings;
    }

    public static T SetFramework<T>(this T toolSettings, string framework) where T : VersonifySettings {
        // toolSettings = toolSettings.NewInstance();
        toolSettings.Framework = framework;
        return toolSettings;
    }


}
