using MarkMello.Application.Abstractions;
using MarkMello.Domain;

namespace MarkMello.Application.Diagrams;

/// <summary>
/// Default <see cref="IDiagramRenderService"/>. Indexes registered renderers
/// by <see cref="IDiagramRenderer.Kind"/> on construction and validates that
/// every <see cref="SupportedDiagramDialects"/> entry has exactly one
/// renderer. Composition failures throw — they are not surfaced as
/// <see cref="DiagramRenderResult.Failure"/>.
/// </summary>
public sealed class DiagramRenderService : IDiagramRenderService
{
    private readonly Dictionary<MarkdownDiagramKind, IDiagramRenderer> _renderers;

    public DiagramRenderService(IEnumerable<IDiagramRenderer> renderers)
    {
        ArgumentNullException.ThrowIfNull(renderers);
        _renderers = BuildIndex(renderers);
    }

    public bool IsSupported(MarkdownDiagramKind kind)
        => _renderers.ContainsKey(kind);

    public DiagramRenderResult Render(MarkdownDiagramKind kind, string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!_renderers.TryGetValue(kind, out var renderer))
        {
            throw new InvalidOperationException(
                $"Diagram dialect '{kind}' is not supported by this application. "
                + "Parser must not emit MarkdownDiagramBlock for unsupported dialects.");
        }

        return renderer.Render(new DiagramRenderRequest(source));
    }

    private static Dictionary<MarkdownDiagramKind, IDiagramRenderer> BuildIndex(
        IEnumerable<IDiagramRenderer> renderers)
    {
        var byKind = new Dictionary<MarkdownDiagramKind, IDiagramRenderer>();

        foreach (var renderer in renderers)
        {
            ArgumentNullException.ThrowIfNull(renderer);

            if (byKind.ContainsKey(renderer.Kind))
            {
                throw new InvalidOperationException(
                    $"Duplicate IDiagramRenderer registration for dialect '{renderer.Kind}'. "
                    + "Each supported dialect must have exactly one renderer.");
            }

            byKind.Add(renderer.Kind, renderer);
        }

        foreach (var supported in SupportedDiagramDialects.Values)
        {
            if (!byKind.ContainsKey(supported))
            {
                throw new InvalidOperationException(
                    $"Supported diagram dialect '{supported}' has no IDiagramRenderer registered. "
                    + "This is an application composition error, not a user-facing state.");
            }
        }

        return byKind;
    }
}
