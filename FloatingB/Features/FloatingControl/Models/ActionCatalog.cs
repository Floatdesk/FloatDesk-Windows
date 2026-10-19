using System.Windows.Media;
using FloatingB.Core.Models;
using FloatingB.Features.Lock.Services;
using FloatingB.Features.Power.Services;
using FloatingB.Features.Screenshot.Services;
using FloatingB.Shared.Helpers;

namespace FloatingB.Features.FloatingControl.Models
{
    /// <summary>
    /// Every action the menu can show, plus the icons users can pick from.
    /// </summary>
    public static class ActionCatalog
    {
        public const int MaxButtons = 5;

        private static readonly Color Brand = Color.FromRgb(0x17, 0x62, 0x7D);
        private static readonly Color Danger = Color.FromRgb(0xE5, 0x48, 0x4D);
        private static readonly Color Warning = Color.FromRgb(0xD9, 0x82, 0x2B);
        private static readonly Color Neutral = Color.FromRgb(0x5B, 0x6B, 0x73);

        public static MenuAction Back { get; } = new()
        {
            Id = "back", Name = "Back", Glyph = "\uE72B", Accent = Neutral
        };

        public static IReadOnlyList<MenuAction> All { get; } = new[]
        {
            new MenuAction
            {
                Id = "screenshot", Name = "Screenshot", Glyph = "\uE722", Accent = Brand,
                Run = ScreenshotService.Start
            },
            new MenuAction
            {
                Id = "power", Name = "Power", Glyph = "\uE7E8", Accent = Danger,
                Children = new[]
                {
                    new MenuAction
                    {
                        Id = "restart", Name = "Restart", Glyph = "\uE72C", Accent = Warning,
                        Run = PowerService.Restart
                    },
                    new MenuAction
                    {
                        Id = "shutdown", Name = "Shut down", Glyph = "\uE7E8", Accent = Danger,
                        Run = PowerService.ShutDown
                    }
                }
            },
            new MenuAction
            {
                Id = "lock", Name = "Lock", Glyph = "\uE72E", Accent = Brand,
                Run = LockService.Lock
            },
            new MenuAction
            {
                Id = "desktop", Name = "Show desktop", Glyph = "\uE7F4", Accent = Brand,
                Run = ShellLauncher.ToggleDesktop
            },
            new MenuAction
            {
                Id = "taskmanager", Name = "Task Manager", Glyph = "\uE9D9", Accent = Brand,
                Run = () => ShellLauncher.Open("taskmgr.exe")
            },
            new MenuAction
            {
                Id = "explorer", Name = "File Explorer", Glyph = "\uED25", Accent = Brand,
                Run = () => ShellLauncher.Open("explorer.exe")
            },
            new MenuAction
            {
                Id = "calculator", Name = "Calculator", Glyph = "\uE8EF", Accent = Brand,
                Run = () => ShellLauncher.Open("calc.exe")
            },
            new MenuAction
            {
                Id = "settings", Name = "Windows Settings", Glyph = "\uE713", Accent = Brand,
                Run = () => ShellLauncher.Open("ms-settings:")
            }
        };

        /// <summary>
        /// Icons offered for the floating button itself.
        /// </summary>
        public static IReadOnlyList<string> MainIconChoices { get; } = new[]
        {
            AppSettings.DefaultIcon, "\uE80F", "\uE71D", "\uE945", "\uE735", "\uEB52", "\uE700", "\uE713", "\uE710"
        };

        /// <summary>
        /// Icons offered for menu buttons.
        /// </summary>
        public static IReadOnlyList<string> ButtonIconChoices { get; } = new[]
        {
            "\uE722", "\uE7E8", "\uE72E", "\uE7F4", "\uE9D9", "\uED25", "\uE8EF", "\uE713",
            "\uE80F", "\uE71D", "\uE945", "\uE735", "\uEB52", "\uE767", "\uE774", "\uE715", "\uE72C"
        };

        public static MenuAction? Find(string id) => All.FirstOrDefault(a => a.Id == id);
    }
}
