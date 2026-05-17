using MarkMello.Application.Abstractions;
using MarkMello.Application.Diagrams;
using MarkMello.Domain;

namespace MarkMello.Presentation.Tests;

public sealed class DiagramRenderServiceTests
{
    [Fact]
    public void ConstructorThrowsWhenMermaidRendererIsMissing()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => new DiagramRenderService(Array.Empty<IDiagramRenderer>()));

        Assert.Contains("Mermaid", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ConstructorThrowsWhenDuplicateMermaidRendererIsRegistered()
    {
        var renderers = new IDiagramRenderer[]
        {
            new StubRenderer(MarkdownDiagramKind.Mermaid),
            new StubRenderer(MarkdownDiagramKind.Mermaid),
        };

        var exception = Assert.Throws<InvalidOperationException>(
            () => new DiagramRenderService(renderers));

        Assert.Contains("Duplicate", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Mermaid", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void IsSupportedReturnsTrueForRegisteredSupportedDialect()
    {
        var service = new DiagramRenderService(
            new IDiagramRenderer[] { new StubRenderer(MarkdownDiagramKind.Mermaid) });

        Assert.True(service.IsSupported(MarkdownDiagramKind.Mermaid));
    }

    [Fact]
    public void IsSupportedReturnsFalseForReservedButUnregisteredDialect()
    {
        var service = new DiagramRenderService(
            new IDiagramRenderer[] { new StubRenderer(MarkdownDiagramKind.Mermaid) });

        Assert.False(service.IsSupported(MarkdownDiagramKind.PlantUml));
    }

    [Fact]
    public void RenderDispatchesToRendererForKindAndReturnsItsResult()
    {
        var stub = new StubRenderer(
            MarkdownDiagramKind.Mermaid,
            request => new DiagramRenderResult.Success($"<svg>{request.Source}</svg>"));
        var service = new DiagramRenderService(new IDiagramRenderer[] { stub });

        var result = service.Render(MarkdownDiagramKind.Mermaid, "flowchart LR\nA-->B");

        var success = Assert.IsType<DiagramRenderResult.Success>(result);
        Assert.Equal("<svg>flowchart LR\nA-->B</svg>", success.Svg);
        Assert.Single(stub.Calls);
        Assert.Equal("flowchart LR\nA-->B", stub.Calls[0].Source);
    }

    [Fact]
    public void RenderThrowsForUnsupportedDialect()
    {
        var service = new DiagramRenderService(
            new IDiagramRenderer[] { new StubRenderer(MarkdownDiagramKind.Mermaid) });

        var exception = Assert.Throws<InvalidOperationException>(
            () => service.Render(MarkdownDiagramKind.PlantUml, "@startuml\n@enduml"));

        Assert.Contains("PlantUml", exception.Message, StringComparison.Ordinal);
    }

    private sealed class StubRenderer(
        MarkdownDiagramKind kind,
        Func<DiagramRenderRequest, DiagramRenderResult>? handler = null) : IDiagramRenderer
    {
        private readonly Func<DiagramRenderRequest, DiagramRenderResult> _handler =
            handler ?? (req => new DiagramRenderResult.Success($"<svg>{req.Source}</svg>"));

        public MarkdownDiagramKind Kind { get; } = kind;

        public List<DiagramRenderRequest> Calls { get; } = new();

        public DiagramRenderResult Render(DiagramRenderRequest request)
        {
            Calls.Add(request);
            return _handler(request);
        }
    }
}
