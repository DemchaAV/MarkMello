using MarkMello.Application.Abstractions;
using MarkMello.Infrastructure.Platform;

namespace MarkMello.Presentation.Tests;

/// <summary>
/// Pins the dual responsibility of <see cref="CommandLineActivation"/>:
/// resolving the startup path from argv (Windows/Linux convention) and
/// bridging runtime «open this file» signals from the platform (macOS
/// Apple Events) into a single source of activation truth.
/// </summary>
public sealed class CommandLineActivationTests : IDisposable
{
    private readonly string _tempFile;

    public CommandLineActivationTests()
    {
        _tempFile = Path.GetTempFileName();
        File.Move(_tempFile, _tempFile + ".md");
        _tempFile += ".md";
        File.WriteAllText(_tempFile, "# Sample");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    [Fact]
    public void EmptyArgsAndNoSignalProduceNullActivationPath()
    {
        var activation = new CommandLineActivation([]);

        Assert.Null(activation.GetActivationFilePath());
    }

    [Fact]
    public void ArgvPathResolvesAsActivation()
    {
        var activation = new CommandLineActivation([_tempFile]);

        Assert.Equal(Path.GetFullPath(_tempFile), activation.GetActivationFilePath());
    }

    [Fact]
    public void RuntimeNotificationBeforeFirstQueryBecomesInitialActivationPath()
    {
        // macOS cold-start: the Apple Event arrives after Avalonia is up
        // but before the view-model finishes initializing. The first call
        // to GetActivationFilePath() must surface it as if argv carried it.
        var activation = new CommandLineActivation([]);

        ((IFileActivationPublisher)activation).NotifyFileActivated(_tempFile);

        Assert.Equal(Path.GetFullPath(_tempFile), activation.GetActivationFilePath());
    }

    [Fact]
    public void RuntimeNotificationAfterFirstQueryFiresEventInstead()
    {
        // macOS warm-state: the user double-clicks another file while
        // MarkMello is already running. The path must surface via the
        // FileActivated event so the view-model can dispatch a load.
        var activation = new CommandLineActivation([]);
        _ = activation.GetActivationFilePath(); // consume the initial slot

        FileActivationEventArgs? received = null;
        ((ICommandLineActivation)activation).FileActivated += (_, e) => received = e;

        ((IFileActivationPublisher)activation).NotifyFileActivated(_tempFile);

        Assert.NotNull(received);
        Assert.Equal(Path.GetFullPath(_tempFile), received!.Path);
    }

    [Fact]
    public void ArgvPathWinsOverCachedRuntimePath()
    {
        // If the platform double-delivers (argv + Apple Event), the explicit
        // command-line path is the authoritative one.
        var activation = new CommandLineActivation([_tempFile]);

        var secondFile = Path.GetTempFileName() + ".md";
        File.WriteAllText(secondFile, "# Other");
        try
        {
            ((IFileActivationPublisher)activation).NotifyFileActivated(secondFile);
            Assert.Equal(Path.GetFullPath(_tempFile), activation.GetActivationFilePath());
        }
        finally
        {
            File.Delete(secondFile);
        }
    }

    [Fact]
    public void NonExistentRuntimePathIsSilentlyIgnored()
    {
        var activation = new CommandLineActivation([]);
        var raised = false;
        ((ICommandLineActivation)activation).FileActivated += (_, _) => raised = true;

        ((IFileActivationPublisher)activation).NotifyFileActivated("/this/path/does/not/exist.md");

        Assert.False(raised);
        _ = activation.GetActivationFilePath();
        Assert.Null(activation.GetActivationFilePath());
    }

    [Fact]
    public void UnsupportedExtensionRuntimePathIsSilentlyIgnored()
    {
        var unsupported = Path.GetTempFileName() + ".bin";
        File.WriteAllBytes(unsupported, [0, 1, 2]);
        try
        {
            var activation = new CommandLineActivation([]);
            var raised = false;
            ((ICommandLineActivation)activation).FileActivated += (_, _) => raised = true;

            ((IFileActivationPublisher)activation).NotifyFileActivated(unsupported);

            Assert.False(raised);
            Assert.Null(activation.GetActivationFilePath());
        }
        finally
        {
            File.Delete(unsupported);
        }
    }
}
