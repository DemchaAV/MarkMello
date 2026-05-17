using MarkMello.Domain;

namespace MarkMello.Application.Diagrams;

/// <summary>
/// Diagram dialects MarkMello declares as runtime-supported. Membership
/// implies a mandatory <c>IDiagramRenderer</c> for the dialect must be
/// registered; absence of a renderer for a supported dialect is a
/// composition error caught by <see cref="DiagramRenderService"/>.
/// </summary>
public static class SupportedDiagramDialects
{
    /// <summary>Single source of truth for the supported diagram dialects.</summary>
    public static IReadOnlyList<MarkdownDiagramKind> Values { get; } =
    [
        MarkdownDiagramKind.Mermaid,
    ];

    /// <summary>
    /// Maps a fenced code block info token (e.g. <c>mermaid</c>) to its
    /// <see cref="MarkdownDiagramKind"/> when the dialect is currently
    /// supported. Reserved dialects (e.g. PlantUML) that exist in the enum
    /// but have no renderer registered are not recognized here, keeping
    /// their fences as regular code blocks until a follow-up ADR.
    /// Match is case-insensitive.
    /// </summary>
    public static bool TryParseFenceToken(string? token, out MarkdownDiagramKind kind)
    {
        kind = default;

        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        if (string.Equals(token, "mermaid", StringComparison.OrdinalIgnoreCase))
        {
            kind = MarkdownDiagramKind.Mermaid;
            return Values.Contains(kind);
        }

        return false;
    }
}
