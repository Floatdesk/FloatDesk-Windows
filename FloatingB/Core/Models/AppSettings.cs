namespace FloatingB.Core.Models
{
    public class AppSettings
    {
        public const string DefaultIcon = "Ω";

        /// <summary>
        /// Text or Segoe MDL2 glyph shown on the floating button.
        /// </summary>
        public string MainIcon { get; set; } = DefaultIcon;

        /// <summary>
        /// Menu buttons in order, top of the menu first.
        /// </summary>
        public List<MenuButtonSetting> Buttons { get; set; } = new()
        {
            new() { ActionId = "screenshot" },
            new() { ActionId = "power" },
            new() { ActionId = "lock" }
        };
    }

    public class MenuButtonSetting
    {
        public string ActionId { get; set; } = "";

        /// <summary>
        /// Custom icon for this button, or null to use the action's own.
        /// </summary>
        public string? Glyph { get; set; }
    }
}
