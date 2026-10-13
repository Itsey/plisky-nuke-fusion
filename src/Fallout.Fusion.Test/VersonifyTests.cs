namespace Fallout.Fusion.Test;

public class VersonifyTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void VersonifyOutputParser_Parse_ShouldExtractAllSupportedMarkers(bool legacy) {
        string output = """
            PNFV]1.2.3.4
            PNF2]1.2
            PNFN]beta
            PNF3]1.2.3
            PN4D]1.2.3.4
            PNQF]1.2.3.5
            PN3D]123
            PNF4]1.2.3.4-beta
            """;
        if (!legacy) {
            output = output.Replace("PN", "P", StringComparison.Ordinal);
        }
        var result = VersonifyOutputParser.Parse(output);

        result.VersionLiteral.ShouldBe("1.2.3.4");
        result.ShortVersion.ShouldBe("1.2");
        result.ReleaseName.ShouldBe("beta");
        result.ThreeDigit.ShouldBe("1.2.3");
        result.FourDigitNumeric.ShouldBe("1.2.3.4");
        result.QueuedFull.ShouldBe("1.2.3.5");
        result.ThreeDigitNumeric.ShouldBe("123");
        result.FourDigit.ShouldBe("1.2.3.4-beta");
        result.FoundMarkers.Count.ShouldBe(8);
        result.FoundMarkers.ShouldContain("PFV]");
    }

    [Fact]
    public void VersonifyOutputParser_Parse_ShouldIgnoreUnrecognisedOutput() {
        var result = VersonifyOutputParser.Parse("Versonify completed without version output.");

        result.VersionLiteral.ShouldBeEmpty();
        result.FoundMarkers.ShouldBeEmpty();
    }

    [Fact]
    public void VersonifyOutputParser_Parse_ShouldUseTheLastValueForRepeatedMarkers() {
        var result = VersonifyOutputParser.Parse("PNFV]1.2.3.4\r\nPFV]1.2.3.5");

        result.VersionLiteral.ShouldBe("1.2.3.5");
    }

    [Theory]
    [InlineData(VersonifyCommand.Passive)]
    [InlineData(VersonifyCommand.UpdateFiles)]
    public void VersonifyTasks_ValidateVersionOutput_ShouldRejectMissingRequiredVersion(VersonifyCommand command) {
        var exception = Should.Throw<InvalidOperationException>(() =>
            VersonifyTasks.ValidateVersionOutput(command, string.Empty, "No version marker"));

        exception.Message.ShouldContain("did not return a PFV] or PNFV] version value");
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
            Command = "--command=Passive",
            VersionPersistanceValue = "2.0.0",
            Root = "src",
            QuickValue = "quick",
            Release = "Bronte",
            MultiMatchFile = "match.txt",
            Debug = true,
            NoOverride = true,
            Digits = "0,1",
            DigitGroup = "default",
            DryRun = true,
            PerformIncrement = true,
            AlwaysReturnZero = true,
            TraceConfiguration = "trace",
            OutputStyle = "azdo"
        };

        string args = settings.GetArgsAsString201().ToStringAndClear();

        args.ShouldBe("--command=passive --version-source=2.0.0 --root=src --quick-value=quick --release=Bronte --min-match=match.txt --debug --no-override --digits=0,1 --digit-group=default --dry-run --increment --no-error --trace=trace --output=azdo");
    }

    [Fact]
    public void VersonifySettings_GetArgsAsString200_ShouldIncludeConfiguredFlags() {
        var settings = new VersonifySettings {
            Command = "-Command=UpdateFiles",
            VersionPersistanceValue = "1.2.3",
            Root = "src",
            QuickValue = "qa",
            Release = "Austen",
            MultiMatchFile = "release.txt",
            Debug = true,
            DryRun = true,
            PerformIncrement = true,
            AlwaysReturnZero = true,
            TraceConfiguration = "trace-config",
            OutputStyle = "env"
        };

        string args = settings.GetArgsAsString200().ToStringAndClear();

        args.ShouldBe("-Command=UpdateFiles -v=1.2.3 -Root=src -Q=qa -Release=Austen -m=release.txt -Debug -DryRun -Increment -z -Trace=trace-config -O=env-nf");
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
            Command = "--command=Passive",
            VersionPersistanceValue = "2.0.0",
            Root = "src"
        };

        string args = settings.GetArgsAsString201().ToStringAndClear();

        args.ShouldBe("--command=passive --version-source=2.0.0 --root=src --output=con-pf");
    }

    [Fact]
    public void VersonifySettings_SetReleaseWithoutValue_ShouldRequestStoredReleaseName() {
        var settings = new VersonifySettings();
        settings.SetCommand(VersonifyCommand.Passive, useLongOption: true);
        settings.SetRelease();

        settings.GetArgsAsString201().ToStringAndClear()
            .ShouldBe("--command=passive --version-source= --root= --release --output=con-pf");
        settings.SetCommand(VersonifyCommand.Passive);
        settings.GetArgsAsString200().ToStringAndClear()
            .ShouldBe("-Command=Passive -v= -Root= -Release -O=con-nf");
    }

    [Theory]
    [InlineData(false, "-Command")]
    [InlineData(true, "--command")]
    public void VersonifySettings_SetCommand_ShouldSupportUnknownCommand(bool usePosix, string expected) {
        var settings = new VersonifySettings();

        settings.SetCommand(VersonifyCommand.Unknown, usePosix);

        settings.Command.ShouldBe($"{expected}=Unknown");
    }

    [Fact]
    public void VersonifySettings_SetCommand_ShouldUseBrontesCommandOptionWhenRequested() {
        var settings = new VersonifySettings();

        settings.SetCommand(VersonifyCommand.Passive, useLongOption: true);

        settings.Command.ShouldBe("--command=Passive");
    }

    [Theory]
    [InlineData(VersonifyCommand.UpdateFiles)]
    [InlineData(VersonifyCommand.Passive)]
    [InlineData(VersonifyCommand.CreateVersion)]
    [InlineData(VersonifyCommand.Override)]
    [InlineData(VersonifyCommand.Set)]
    public void VersonifySettings_SetCommand_ShouldSetCommandArgument_IsLegacy(VersonifyCommand command) {
        var settings = new VersonifySettings();

        settings.SetCommand(command);

        settings.Command.ShouldBe($"-Command={command}");
        settings.GetArgsAsString200().ToStringAndClear().ShouldStartWith($"-Command={command}");
        settings.GetArgsString().ToStringAndClear().ShouldStartWith($"-Command={command}");
    }
    [Theory]
    [InlineData(VersonifyCommand.UpdateFiles)]
    [InlineData(VersonifyCommand.Passive)]
    [InlineData(VersonifyCommand.CreateVersion)]
    [InlineData(VersonifyCommand.Override)]
    [InlineData(VersonifyCommand.Set)]
    public void VersonifySettings_SetCommand_ShouldSetCommandArgument_Bronte(VersonifyCommand command) {
        var settings = new VersonifySettings();

        settings.SetCommand(command, useLongOption: true);

        settings.Command.ShouldBe($"--command={command}");
        settings.GetArgsAsString201().ToStringAndClear().ShouldStartWith(settings.Command);
    }

    [Theory]
    [InlineData("con")]
    [InlineData("con-pf")]
    [InlineData("con-nf")]
    [InlineData("jcon")]
    [InlineData("env")]
    [InlineData("azdo:BuildVersion")]
    [InlineData("file:version.txt")]
    public void VersonifySettings_GetArgsAsString201_ShouldPreserveOutputMode(string outputStyle) {
        var settings = new VersonifySettings().SetOutputStyle(outputStyle);

        settings.GetArgsAsString201().ToStringAndClear().ShouldEndWith($"--output={outputStyle}");
        settings.GetArgsAsString201().ToStringAndClear().ShouldEndWith($"--output={outputStyle}");
        settings.OutputStyle.ShouldBe(outputStyle);
    }

    [Fact]
    public void VersonifySettings_GetArgsAsString201_ShouldQuotePathsWithSpaces() {
        var settings = new VersonifySettings {
            VersionPersistanceValue = @"C:\version stores\release.vstore",
            Root = @"C:\source code",
            MultiMatchFile = @"C:\source code\matches.txt"
        };
        settings.SetCommand(VersonifyCommand.UpdateFiles);

        string args = settings.GetArgsAsString201().ToStringAndClear();

        args.ShouldContain("--version-source=\"C:\\version stores\\release.vstore\"");
        args.ShouldContain("--root=\"C:\\source code\"");
        args.ShouldContain("--min-match=\"C:\\source code\\matches.txt\"");
    }

    [Fact]
    public void VersonifySettings_GetArgsAsString201_ShouldIncludePreReleaseShortcut() {
        var settings = new VersonifySettings().SetPreRelease(true);

        settings.GetArgsAsString201().ToStringAndClear().ShouldContain(" --pre-release");
    }

    [Fact]
    public void VersonifySettings_LegacyArguments_ShouldMapFusionOutputWithoutChangingSettings() {
        var settings = new VersonifySettings().SetOutputStyle("con-pf");
        settings.SetCommand(VersonifyCommand.Passive);

        settings.GetArgsAsString200().ToStringAndClear().ShouldEndWith("-O=con-nf");
        settings.GetArgsString().ToStringAndClear().ShouldEndWith("-O=con-nf");
        settings.GetArgsAsString201().ToStringAndClear().ShouldEndWith("--output=con-pf");
        settings.OutputStyle.ShouldBe("con-pf");
    }

    [Theory]
    [InlineData(true, "")]
    [InlineData(false, "default")]
    public void VersonifySettings_LegacyArguments_ShouldRejectUnsupportedGroups(bool preRelease, string digitGroup) {
        var settings = new VersonifySettings().SetPreRelease(preRelease).SetDigitGroup(digitGroup);

        Should.Throw<InvalidOperationException>(() => settings.GetArgsAsString200())
            .Message.ShouldContain("digit groups require compatibility level 201 or above");
        Should.Throw<InvalidOperationException>(() => settings.GetArgsString())
            .Message.ShouldContain("digit groups require compatibility level 201 or above");
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
            .SetDigits("*")
            .SetDigitGroup("default")
            .SetPreRelease(false)
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
        settings.Digits.ShouldBe("*");
        settings.DigitGroup.ShouldBe("default");
        settings.PreRelease.ShouldBeFalse();
        settings.Framework.ShouldBe("net10.0");
    }
}
