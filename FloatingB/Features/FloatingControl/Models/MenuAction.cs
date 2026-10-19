using System.Windows.Media;

namespace FloatingB.Features.FloatingControl.Models
{
    /// <summary>
    /// Something a menu button can do. An action with children opens
    /// a second ring of buttons instead of running.
    /// </summary>
    public sealed class MenuAction
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Glyph { get; init; }
        public required Color Accent { get; init; }
        public Action? Run { get; init; }
        public IReadOnlyList<MenuAction> Children { get; init; } = Array.Empty<MenuAction>();
    }

    /// <summary>
    /// A button as it appears in the menu: the action plus the icon
    /// the user picked for it.
    /// </summary>
    public sealed record MenuEntry(MenuAction Action, string Glyph);
}
