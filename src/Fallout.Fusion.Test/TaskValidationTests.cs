namespace Fallout.Fusion.Test;

public class TaskValidationTests {
    [Fact]
    public void MollycoddleTasks_PerformScan_ShouldRejectNullConfiguration() {
        var tasks = new MollycoddleTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.PerformScan((Configure<MollycoddleSettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_RawExecute_ShouldRejectNullConfiguration() {
        var tasks = new VersonifyTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.RawExecute((Configure<VersonifySettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_FileUpdateCommand_ShouldRejectNullConfiguration() {
        var tasks = new VersonifyTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.FileUpdateCommand((Configure<VersonifySettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_OverrideCommand_ShouldRejectNullConfiguration() {
        var tasks = new VersonifyTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.OverrideCommand((Configure<VersonifySettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_PassiveCommand_ShouldRejectNullConfiguration() {
        var tasks = new VersonifyTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.PassiveCommand((Configure<VersonifySettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_SetCommand_ShouldRejectNullConfiguration() {
        var tasks = new VersonifyTasks();

        Should.Throw<InvalidOperationException>(() =>
            tasks.SetCommand((Configure<VersonifySettings>)null!));
    }

    [Fact]
    public void VersonifyTasks_SetCommand_ShouldExecuteSetWithConfiguredSettings() {
        var tasks = new RecordingVersonifyTasks();
        bool configured = false;

        tasks.SetCommand(settings => {
            configured = true;
            settings.Root = "src";
            return settings;
        });

        configured.ShouldBeTrue();
        tasks.Command.ShouldBe(VersonifyCommand.Set);
        tasks.Settings!.Root.ShouldBe("src");
    }

    [Fact]
    public void VersonifyTasks_PassiveCommand_ShouldRejectAlwaysReturnZero() {
        var tasks = new VersonifyTasks();

        var exception = Should.Throw<InvalidOperationException>(() =>
            tasks.PassiveCommand(settings => settings.SetZeroReturnCode(true)));

        exception.Message.ShouldContain("--no-error");
    }

    private sealed class RecordingVersonifyTasks : VersonifyTasks {
        public VersonifyCommand Command { get; private set; }
        public VersonifySettings? Settings { get; private set; }

        public override IReadOnlyCollection<Output> ExecuteVersonify(
            VersonifySettings? settings,
            VersonifyCommand command = VersonifyCommand.Unknown,
            string? replaceCommandLine = null) {
            Command = command;
            Settings = settings;
            return [];
        }
    }
}
