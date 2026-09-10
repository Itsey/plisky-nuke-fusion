namespace Nuke.Fusion.Test;

public class VersonifyTests {
    [Fact]
    public void VersonifySettings_GetArgsAsString201_ShouldUseBrontesArgumentNames() {
        var settings = new VersonifySettings {
            Command = "-Command=Passive",
            VersionPersistanceValue = "2.0.0",
            Root = "src",
            QuickValue = "quick",
            MultiMatchFile = "match.txt",
            Debug = true,
            DryRun = true,
            PerformIncrement = true,
            AlwaysReturnZero = true,
            TraceConfiguration = "trace",
            OutputStyle = "azdo"
        };

        string args = settings.GetArgsAsString201().ToStringAndClear();

        args.ShouldBe("-Command=Passive --version-source=2.0.0 --root=src --quick-value=quick --min-match=match.txt -debug --dry-run --increment --no-error --trace=trace --output=azdo-nf");
    }

    [Fact]
    public void VersonifySettings_GetArgsAsString200_ShouldIncludeConfiguredFlags() {
        var settings = new VersonifySettings {
            Command = "-Command=UpdateFiles",
            VersionPersistanceValue = "1.2.3",
            Root = "src",
            QuickValue = "qa",
            MultiMatchFile = "release.txt",
            Debug = true,
            DryRun = true,
            PerformIncrement = true,
            AlwaysReturnZero = true,
            TraceConfiguration = "trace-config",
            OutputStyle = "env"
        };

        string args = settings.GetArgsAsString200().ToStringAndClear();

        args.ShouldBe("-Command=UpdateFiles -v=1.2.3 -Root=src -Q=qa -m=release.txt -Debug -DryRun -Increment -z -Trace=trace-config -O=env-nf");
    }

    [Fact]
    public void VersonifySettings_GetArgsString_ShouldUseLegacyVersionSourceAndDefaultOutput() {
        var settings = new VersonifySettings {
            Command = "-Command=Passive",
            VersionPersistanceValue = "1.0.0",
            Root = "src"
        };

        string args = settings.GetArgsString().ToStringAndClear();

        args.ShouldBe("-Command=Passive -vs=1.0.0 -Root=src -O=con-nf");
    }

    [Fact]
    public void VersonifySettings_GetArgsString_ShouldOmitUnsetOptionalArguments() {
        var settings = new VersonifySettings {
            Command = "-Command=Passive",
            VersionPersistanceValue = "1.0.0",
            Root = "src",
            OutputStyle = "env-nf"
        };

        string args = settings.GetArgsString().ToStringAndClear();

        args.ShouldBe("-Command=Passive -vs=1.0.0 -Root=src -O=env-nf");
    }

    [Fact]
    public void VersonifySettings_GetArgsAsString201_ShouldUseDefaultOutputWhenUnset() {
        var settings = new VersonifySettings {
            Command = "-Command=Passive",
            VersionPersistanceValue = "2.0.0",
            Root = "src"
        };

        string args = settings.GetArgsAsString201().ToStringAndClear();

        args.ShouldBe("-Command=Passive --version-source=2.0.0 --root=src --output=con-nf");
    }

    [Fact]
    public void VersonifySettings_SetCommand_ShouldSupportUnknownCommand() {
        var settings = new VersonifySettings();

        settings.SetCommand(VersonifyCommand.Unknown);

        settings.Command.ShouldBe("-Command=Unknown");
    }

    [Theory]
    [InlineData(VersonifyCommand.UpdateFiles)]
    [InlineData(VersonifyCommand.Passive)]
    [InlineData(VersonifyCommand.CreateVersion)]
    [InlineData(VersonifyCommand.Override)]
    public void VersonifySettings_SetCommand_ShouldSetCommandArgument(VersonifyCommand command) {
        var settings = new VersonifySettings();

        settings.SetCommand(command);

        settings.Command.ShouldBe($"-Command={command}");
    }

    [Fact]
    public void VersonifySettingsExtensions_ShouldSetFlagsAndKeepFluentInterface() {
        var settings = new VersonifySettings()
            .SetZeroReturnCode(true)
            .SetNoOverride(true)
            .AsDryRun(true)
            .SetOutputStyle("azdo");

        settings.AlwaysReturnZero.ShouldBeTrue();
        settings.NoOverride.ShouldBeTrue();
        settings.DryRun.ShouldBeTrue();
        settings.OutputStyle.ShouldBe("azdo");
    }

    [Fact]
    public void VersonifySettingsExtensions_ShouldSetAllSupportedValues() {
        var settings = new VersonifySettings()
            .AddMultimatchFile("autoversion.txt")
            .AsDryRun(true)
            .PerformIncrement(true)
            .SetDebug(true)
            .SetTrace("trace")
            .SetRoot("src")
            .SetRelease("release")
            .SetVersionPersistanceValue("1.2.3")
            .SetQuickValue("+.+.+")
            .SetFramework("net10.0");

        settings.MultiMatchFile.ShouldBe("autoversion.txt");
        settings.DryRun.ShouldBeTrue();
        settings.PerformIncrement.ShouldBeTrue();
        settings.Debug.ShouldBeTrue();
        settings.TraceConfiguration.ShouldBe("trace");
        settings.Root.ShouldBe("src");
        settings.Release.ShouldBe("release");
        settings.VersionPersistanceValue.ShouldBe("1.2.3");
        settings.QuickValue.ShouldBe("+.+.+");
        settings.Framework.ShouldBe("net10.0");
    }
}
