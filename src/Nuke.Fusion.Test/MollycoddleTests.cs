namespace Nuke.Fusion.Test;

public class MollycoddleTests {
    [Fact]
    public void MollycoddleSettings_GetArgsString_ShouldUseRequiredArgumentsAndDefaults() {
        var settings = new MollycoddleSettings {
            Directory = "src",
            RulesFile = "rules.xml",
            Formatter = "plain"
        };

        string args = settings.GetArgsString().ToStringAndClear();

        args.ShouldBe(" -dir=src -rulesFile=rules.xml -formatter=plain -version=default");
    }

    [Fact]
    public void MollycoddleSettingsExtensions_ShouldAcceptValidFormatterAndValidateInvalidValues() {
        var settings = new MollycoddleSettings();

        var result = settings.SetFormatter("AzDo");

        result.Formatter.ShouldBe("azdo");
        Should.Throw<InvalidOperationException>(() => settings.SetFormatter("xml"));
    }

    [Fact]
    public void MollycoddleSettings_GetArgsString_ShouldIncludeEnabledFlags() {
        var settings = new MollycoddleSettings {
            Directory = "src",
            RulesFile = "rules.xml",
            Formatter = "plain",
            RulesetVersion = "v2",
            PrimaryRoot = "src/main",
            RuleHelp = true,
            Debug = true,
            DryRun = true,
            Disabled = true,
            TraceConfiguration = "verbose"
        };

        string args = settings.GetArgsString().ToStringAndClear();

        args.ShouldBe(" -dir=src -rulesFile=rules.xml -formatter=plain -version=v2 -primaryRoot=src/main -addrulehelp -Debug=v-** -WarnOnly -Disabled -Trace=verbose");
    }

    [Fact]
    public void MollycoddleSettingsExtensions_ShouldSetAllSupportedValues() {
        var settings = new MollycoddleSettings()
            .SetDisabled(true)
            .SetDebug(true)
            .AsDryRun(true)
            .AddRuleHelp(true)
            .AddRulesetVersion("v3")
            .SetDirectory("src")
            .SetRulesFile("rules.xml")
            .SetPrimaryRoot("src/main")
            .SetFormatter("plain")
            .SetFramework("net10.0");

        settings.Disabled.ShouldBeTrue();
        settings.Debug.ShouldBeTrue();
        settings.DryRun.ShouldBeTrue();
        settings.RuleHelp.ShouldBeTrue();
        settings.RulesetVersion.ShouldBe("v3");
        settings.Directory.ShouldBe("src");
        settings.RulesFile.ShouldBe("rules.xml");
        settings.PrimaryRoot.ShouldBe("src/main");
        settings.Formatter.ShouldBe("plain");
        settings.Framework.ShouldBe("net10.0");
    }

    [Fact]
    public void MollycoddleSettingsExtensions_SetFormatter_ShouldRejectInvalidValueWithHelpfulMessage() {
        var settings = new MollycoddleSettings();

        var exception = Should.Throw<InvalidOperationException>(() => settings.SetFormatter("json"));

        exception.Message.ShouldBe("Formatter must be either 'plain' or 'azdo'");
    }
}
