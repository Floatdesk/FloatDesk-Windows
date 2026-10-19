using System.Collections.ObjectModel;
using System.Windows.Media;
using FloatingB.Core.Models;
using FloatingB.Features.FloatingControl.Models;
using FloatingB.Shared.Helpers;

namespace FloatingB.Features.Settings.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _selectedIcon = AppSettings.DefaultIcon;
        private string _customIcon = "";
        private string _message = "";

        public ObservableCollection<IconOption> IconOptions { get; } = new();

        /// <summary>
        /// Every action, enabled ones first in menu order.
        /// </summary>
        public ObservableCollection<ActionRow> Rows { get; } = new();

        public IReadOnlyList<string> ButtonIconChoices => ActionCatalog.ButtonIconChoices;

        public string SelectedIcon
        {
            get => _selectedIcon;
            private set
            {
                if (SetProperty(ref _selectedIcon, value))
                {
                    OnPropertyChanged(nameof(SelectedIconFont));
                }
            }
        }

        public FontFamily SelectedIconFont => GlyphFont.For(SelectedIcon);

        /// <summary>
        /// Your own icon text. Typing here replaces the picked icon.
        /// </summary>
        public string CustomIcon
        {
            get => _customIcon;
            set
            {
                if (!SetProperty(ref _customIcon, value)) return;
                if (string.IsNullOrWhiteSpace(value)) return;

                foreach (var option in IconOptions) option.IsSelected = false;
                SelectedIcon = value.Trim();
            }
        }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public bool CanSave => Rows.Any(r => r.IsEnabled);

        public SettingsViewModel(AppSettings settings)
        {
            Load(settings);
        }

        public void Load(AppSettings settings)
        {
            IconOptions.Clear();
            foreach (string glyph in ActionCatalog.MainIconChoices)
            {
                IconOptions.Add(new IconOption(glyph, OnIconPicked)
                {
                    IsSelected = glyph == settings.MainIcon
                });
            }

            bool isCustom = !ActionCatalog.MainIconChoices.Contains(settings.MainIcon);
            _customIcon = isCustom ? settings.MainIcon : "";
            OnPropertyChanged(nameof(CustomIcon));
            SelectedIcon = settings.MainIcon;

            Rows.Clear();

            foreach (var button in settings.Buttons)
            {
                var action = ActionCatalog.Find(button.ActionId);
                if (action == null || Rows.Any(r => r.Action == action)) continue;

                string glyph = string.IsNullOrEmpty(button.Glyph) ? action.Glyph : button.Glyph;
                Rows.Add(new ActionRow(action, glyph, isEnabled: true, OnRowToggled));
            }

            foreach (var action in ActionCatalog.All.Where(a => Rows.All(r => r.Action != a)))
            {
                Rows.Add(new ActionRow(action, action.Glyph, isEnabled: false, OnRowToggled));
            }

            Message = "";
            OnPropertyChanged(nameof(CanSave));
        }

        public void MoveUp(ActionRow row)
        {
            int index = Rows.IndexOf(row);
            if (index > 0) Rows.Move(index, index - 1);
        }

        public void MoveDown(ActionRow row)
        {
            int index = Rows.IndexOf(row);
            if (index >= 0 && index < Rows.Count - 1) Rows.Move(index, index + 1);
        }

        public AppSettings ToSettings()
        {
            return new AppSettings
            {
                MainIcon = SelectedIcon,
                Buttons = Rows
                    .Where(r => r.IsEnabled)
                    .Select(r => new MenuButtonSetting
                    {
                        ActionId = r.Action.Id,
                        Glyph = r.Glyph == r.Action.Glyph ? null : r.Glyph
                    })
                    .ToList()
            };
        }

        private void OnIconPicked(IconOption option)
        {
            _customIcon = "";
            OnPropertyChanged(nameof(CustomIcon));
            SelectedIcon = option.Glyph;
        }

        private void OnRowToggled(ActionRow row)
        {
            if (row.IsEnabled && Rows.Count(r => r.IsEnabled) > ActionCatalog.MaxButtons)
            {
                row.IsEnabled = false;
                Message = $"You can show up to {ActionCatalog.MaxButtons} buttons.";
            }
            else
            {
                Message = "";
            }

            OnPropertyChanged(nameof(CanSave));
        }
    }

    public class IconOption : ObservableObject
    {
        private readonly Action<IconOption> _picked;
        private bool _isSelected;

        public IconOption(string glyph, Action<IconOption> picked)
        {
            Glyph = glyph;
            _picked = picked;
        }

        public string Glyph { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value) && value)
                {
                    _picked(this);
                }
            }
        }
    }

    public class ActionRow : ObservableObject
    {
        private readonly Action<ActionRow> _toggled;
        private string _glyph;
        private bool _isEnabled;

        public ActionRow(MenuAction action, string glyph, bool isEnabled, Action<ActionRow> toggled)
        {
            Action = action;
            _glyph = glyph;
            _isEnabled = isEnabled;
            _toggled = toggled;
        }

        public MenuAction Action { get; }

        public string Name => Action.Name;

        public string Glyph
        {
            get => _glyph;
            set => SetProperty(ref _glyph, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value))
                {
                    _toggled(this);
                }
            }
        }
    }
}
