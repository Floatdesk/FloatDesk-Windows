using System.Windows.Media;
using FloatingB.Core.Models;
using FloatingB.Features.FloatingControl.Models;
using FloatingB.Shared.Helpers;

namespace FloatingB.Features.FloatingControl.ViewModels
{
    public class FloatingControlViewModel : ObservableObject
    {
        private string _icon = AppSettings.DefaultIcon;

        public string Icon
        {
            get => _icon;
            private set
            {
                if (SetProperty(ref _icon, value))
                {
                    OnPropertyChanged(nameof(IconFont));
                }
            }
        }

        public FontFamily IconFont => GlyphFont.For(Icon);

        /// <summary>
        /// The buttons in the main menu, top first.
        /// </summary>
        public IReadOnlyList<MenuEntry> Entries { get; private set; } = Array.Empty<MenuEntry>();

        public void Apply(AppSettings settings)
        {
            Icon = string.IsNullOrWhiteSpace(settings.MainIcon)
                ? AppSettings.DefaultIcon
                : settings.MainIcon;

            var entries = new List<MenuEntry>();

            foreach (var button in settings.Buttons)
            {
                var action = ActionCatalog.Find(button.ActionId);
                if (action == null) continue;

                string glyph = string.IsNullOrEmpty(button.Glyph) ? action.Glyph : button.Glyph;
                entries.Add(new MenuEntry(action, glyph));

                if (entries.Count == ActionCatalog.MaxButtons) break;
            }

            Entries = entries;
        }
    }
}
