namespace Fallout.Fusion.Test;

public class VersonifyTests {
    [Fact]
    public void VersonifyOutputParser_Parse_ShouldExtractAllSupportedMarkers() {
        var result = VersonifyOutputParser.Parse("""
            PNFV]1.2.3.4
            PNF2]1.2
            PNFN]beta
            PNF3]1.2.3
            PN4D]1.2.3.4
            PNQF]1.2.3.5
            PN3D]123
            PNF4]1.2.3.4-beta
            """);

        result.VersionLiteral.ShouldBe("1.2.3.4");
        result.ShortVersion.ShouldBe("1.2");
        result.ReleaseName.ShouldBe("beta");
        result.ThreeDigit.ShouldBe("1.2.3");
        result.FourDigitNumeric.ShouldBe("1.2.3.4");
        result.QueuedFull.ShouldBe("1.2.3.5");
        result.ThreeDigitNumeric.ShouldBe("123");
        result.FourDigit.ShouldBe("1.2.3.4-beta");
    }

    [Fact]
    public void VersonifyOutputParser_Parse_ShouldIgnoreUnrecognisedOutput() {
        var result = VersonifyOutputParser.Parse("Versonify completed without version output.");

        result.VersionLiteral.ShouldBeEmpty();
        result.FoundMarkers.ShouldBeEmpty();
    }

    [Fact]
    public void VersonifyOutputParser_Parse_ShouldUseTheLastValueForRepeatedMarkers() {
        var result = VersonifyOutputParser.Parse("PNFV]1.2.3.4\r\nPNFV]1.2.3.5");

        result.VersionLiteral.ShouldBe("1.2.3.5");
    }

    [Theory]
    [InlineData(VersonifyCommand.Passive)]
    [InlineData(VersonifyCommand.UpdateFiles)]
    public void VersonifyTasks_ValidateVersionOutput_ShouldRejectMissingRequiredVersion(VersonifyCommand command) {
        var exception = Should.Throw<InvalidOperationException>(() =>
            VersonifyTasks.ValidateVersionOutput(command, string.Empty, "No version marker"));

        exception.Message.ShouldContain("did not return a PNFV] version value");
        exception.Message.ShouldContain("No version marker");
    }

    [Theory]
    [InlineData(VersonifyCommand.Override)]
    [InlineData(VersonifyCommand.CreateVersion)]
    [InlineData(VersonifyCommand.Unknown)]
    public void VersonifyTasks_ValidateVersionOutput_ShouldAllowCommandsWithoutVersionOutput(VersonifyCommand command) {
        Should.NotThrow(() => VersonifyTasks.ValidateVersionOutput(command, string.Empty, "No version marker"));
    }

    [Fact]
    public void VersonifyTasks_ThrowIfCommandFailed_ShouldThrowForNonZeroExitCode() {
        var exception = Should.Throw<InvalidOperationException>(() =>
            VersonifyTasks.ThrowIfCommandFailed(
                VersonifyCommand.Passive,
                7,
                "Std: unable to read version store\nErr: access denied"));

        exception.Message.ShouldContain("Versonify 'Passive' failed with exit code 7.");
        exception.Message.ShouldContain("Std: unable to read version store");
        exception.Message.ShouldContain("Err: access denied");
    }

    [Fact]
    public void VersonifyTasks_ThrowIfCommandFailed_ShouldAllowZeroExitCode() {
        Should.NotThrow(() =>
            VersonifyTasks.ThrowIfCommandFailed(
                VersonifyCommand.Passive,
                0,
                "PNFV]1.2.3.4"));
    }

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
