namespace Plisky.Nuke.Fusion;
public static class GenericTasks {


    /*
    public static IReadOnlyCollection<Output> Run(Configure<GenericSettings> configure) {

        if (configure == null) {
            throw new InvalidOperationException();
        }

        return Run(configure(new GenericSettings()));
    }

    public static IReadOnlyCollection<Output> Run(GenericSettings settings) {

        settings = settings ?? new GenericSettings();

        using var process = ProcessTasks.StartProcess(settings);
        settings.ProcessExitHandler.Invoke(settings, process.AssertWaitForExit());
        return process.Output;
    }

    */

}
