using MarkMello.Domain;

namespace MarkMello.Application.Abstractions;

/// <summary>
/// Renders a single diagram dialect (Mermaid, PlantUML, ...) into an
/// UI-agnostic <see cref="DiagramRenderResult"/>. Implementations live in
/// the infrastructure layer and pull in whatever managed backend they need
/// (e.g. Naiad for <see cref="MarkdownDiagramKind.Mermaid"/>). For each
/// dialect declared supported by the application, exactly one
/// <see cref="IDiagramRenderer"/> must be registered.
/// </summary>
public interface IDiagramRenderer
{
    /// <summary>Diagram dialect this renderer handles.</summary>
    MarkdownDiagramKind Kind { get; }

    /// <summary>
    /// Renders the diagram source. The renderer must convert backend
    /// exceptions raised by an individual diagram into a
    /// <see cref="DiagramRenderResult.Failure"/>; only true composition or
    /// programmer errors are allowed to propagate as exceptions.
    /// </summary>
    DiagramRenderResult Render(DiagramRenderRequest request);
}
