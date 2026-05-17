namespace MarkMello.Application.Abstractions;

/// <summary>
/// Input for <see cref="IDiagramRenderer.Render"/>. Kept as a discrete record
/// so renderer-agnostic options (e.g. fence-level title, theme hints) can be
/// added without breaking the interface.
/// </summary>
public sealed record DiagramRenderRequest(string Source);
