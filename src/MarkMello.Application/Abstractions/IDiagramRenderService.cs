using MarkMello.Domain;

namespace MarkMello.Application.Abstractions;

/// <summary>
/// Orchestrates diagram rendering across registered <see cref="IDiagramRenderer"/>s.
/// Validates application composition on construction: every dialect declared
/// supported by the application must have exactly one renderer registered.
/// </summary>
public interface IDiagramRenderService
{
    /// <summary>
    /// True when <paramref name="kind"/> is declared supported by the
    /// application and a renderer for it is registered.
    /// </summary>
    bool IsSupported(MarkdownDiagramKind kind);

    /// <summary>
    /// Renders <paramref name="source"/> using the renderer registered for
    /// <paramref name="kind"/>. Calling this for an unsupported dialect is
    /// a programming error and throws — the parser is responsible for not
    /// emitting diagram blocks of unsupported dialects.
    /// </summary>
    DiagramRenderResult Render(MarkdownDiagramKind kind, string source);
}
