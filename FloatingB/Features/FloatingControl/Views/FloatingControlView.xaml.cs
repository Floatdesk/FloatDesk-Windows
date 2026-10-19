using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FloatingB.Core.Models;
using FloatingB.Features.FloatingControl.Models;
using FloatingB.Features.FloatingControl.ViewModels;
using FloatingB.Infrastructure.Windows;
using FloatingB.Shared.Helpers;

namespace FloatingB.Features.FloatingControl.Views
{
    public partial class FloatingControlView : UserControl
    {
        // Layout constants, in the right-edge layout of the XAML.
        private const double CanvasWidth = 128;
        private const double CanvasHeight = 188;
        private const double CenterX = 94;
        private const double CenterY = 94;
        private const double RingRadius = 64;
        private const double ButtonSize = 42;
        private const double MaxSpacingDegrees = 50;
        private const double IconSize = 19;

        // How far the button hides past the edge at rest (half of it).
        private const double PeekOffset = CanvasWidth - CenterX;

        // Pointer distances (DIPs from the button centre) that slide the
        // button out, tuck it back in, and close an open menu.
        private const double RevealDistance = 80;
        private const double HideDistance = 110;
        private const double CloseMenuDistance = 220;

        private readonly TimeSpan _time = TimeSpan.FromMilliseconds(200);
        private readonly TimeSpan _stagger = TimeSpan.FromMilliseconds(40);
        private readonly FloatingControlViewModel _viewModel = new();
        private readonly DispatcherTimer _proximityTimer;

        private bool _isOpen;
        private bool _isExpanded;
        private ScreenEdge _edge = ScreenEdge.Right;
        private Transform _iconTransform = Transform.Identity;

        public bool IsMenuOpen => _isOpen;

        /// <summary>
        /// Set while the window is being dragged so the button stays out.
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Size the window needs for the current edge.
        /// </summary>
        public Size LayoutSize => _edge == ScreenEdge.Top
            ? new Size(CanvasHeight, CanvasWidth)
            : new Size(CanvasWidth, CanvasHeight);

        public FloatingControlView()
        {
            InitializeComponent();

            DataContext = _viewModel;

            Menu.RenderTransformOrigin = new Point(CenterX / CanvasWidth, CenterY / CanvasHeight);
            PeekShift.X = PeekOffset;

            _proximityTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(40),
                DispatcherPriority.Input,
                ProximityTimer_Tick,
                Dispatcher);

            Unloaded += (s, e) => _proximityTimer.Stop();
        }

        public void ApplySettings(AppSettings settings)
        {
            CloseMenu();
            _viewModel.Apply(settings);
        }

        /// <summary>
        /// Mirrors or rotates the control so the button tucks into the
        /// given edge and the menu opens away from it. Icons are turned
        /// back so they stay upright.
        /// </summary>
        public void SetEdge(ScreenEdge edge)
        {
            _edge = edge;

            (Transform layout, Transform icon) = edge switch
            {
                ScreenEdge.Left => ((Transform)new ScaleTransform(-1, 1), (Transform)new ScaleTransform(-1, 1)),
                ScreenEdge.Top => (new RotateTransform(-90), new RotateTransform(90)),
                _ => (Transform.Identity, Transform.Identity)
            };

            Root.LayoutTransform = layout;
            IconOrient.LayoutTransform = icon;
            _iconTransform = icon;

            foreach (var button in ButtonLayer.Children.OfType<Button>())
            {
                if (button.Content is FrameworkElement content)
                {
                    content.LayoutTransform = icon;
                }
            }
        }

        public void OpenMenu()
        {
            if (_viewModel.Entries.Count == 0) return;

            _isOpen = true;
            SetExpanded(true);
            ShowEntries(_viewModel.Entries);

            Menu.Visibility = Visibility.Visible;

            // Small bounce at the end of the pop-out
            var bounce = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 };

            Menu.BeginAnimation(OpacityProperty, new DoubleAnimation(1, _time));
            MenuScale.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(1, _time) { EasingFunction = bounce });
            MenuScale.BeginAnimation(ScaleTransform.ScaleYProperty,
                new DoubleAnimation(1, _time) { EasingFunction = bounce });

            AnimateIcon(open: true);
        }

        public void CloseMenu()
        {
            if (!_isOpen) return;
            _isOpen = false;

            var fade = new DoubleAnimation(0, _time);
            fade.Completed += (s, e) =>
            {
                // Hide it fully once the fade finishes
                if (!_isOpen) Menu.Visibility = Visibility.Collapsed;
            };

            Menu.BeginAnimation(OpacityProperty, fade);
            MenuScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.4, _time));
            MenuScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.4, _time));

            AnimateIcon(open: false);
        }

        /// <summary>
        /// Lays the buttons out along an arc facing away from the edge,
        /// top to bottom, and sizes the ring behind them to fit.
        /// </summary>
        private void ShowEntries(IReadOnlyList<MenuEntry> entries)
        {
            ButtonLayer.Children.Clear();

            int count = entries.Count;
            double spacing = count > 1 ? Math.Min(MaxSpacingDegrees, 180.0 / (count - 1)) : 0;
            double span = spacing * (count - 1);

            Ring.Data = BuildRing(180 - span / 2, 180 + span / 2);

            var bounce = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 };

            for (int i = 0; i < count; i++)
            {
                // 180° points straight away from the edge; larger angles are higher up.
                double angle = (180 + span / 2 - i * spacing) * Math.PI / 180;

                var button = CreateButton(entries[i]);
                Canvas.SetLeft(button, CenterX + RingRadius * Math.Cos(angle) - ButtonSize / 2);
                Canvas.SetTop(button, CenterY + RingRadius * Math.Sin(angle) - ButtonSize / 2);
                ButtonLayer.Children.Add(button);

                // Buttons pop in one after another
                var scale = (ScaleTransform)button.RenderTransform;
                var popIn = new DoubleAnimation(0, 1, _time)
                {
                    BeginTime = TimeSpan.FromTicks(_stagger.Ticks * (i + 1)),
                    EasingFunction = bounce
                };

                scale.BeginAnimation(ScaleTransform.ScaleXProperty, popIn);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, popIn);
            }
        }

        private static Geometry BuildRing(double startDegrees, double endDegrees)
        {
            // A zero-length arc draws nothing, so give a lone button a sliver.
            if (endDegrees - startDegrees < 0.1) endDegrees = startDegrees + 0.1;

            var figure = new PathFigure { StartPoint = PointOnRing(startDegrees), IsFilled = false };
            figure.Segments.Add(new ArcSegment(
                PointOnRing(endDegrees),
                new Size(RingRadius, RingRadius),
                0,
                endDegrees - startDegrees > 180,
                SweepDirection.Clockwise,
                true));

            return new PathGeometry(new[] { figure });
        }

        private static Point PointOnRing(double degrees)
        {
            double radians = degrees * Math.PI / 180;
            return new Point(
                CenterX + RingRadius * Math.Cos(radians),
                CenterY + RingRadius * Math.Sin(radians));
        }

        private Button CreateButton(MenuEntry entry)
        {
            var icon = new TextBlock
            {
                Text = entry.Glyph,
                FontFamily = GlyphFont.For(entry.Glyph),
                FontSize = IconSize,
                LayoutTransform = _iconTransform
            };

            var button = new Button
            {
                Style = (Style)FindResource("MenuButtonStyle"),
                Tag = new SolidColorBrush(entry.Action.Accent),
                ToolTip = entry.Action.Name,
                Content = icon,
                RenderTransform = new ScaleTransform(0, 0)
            };

            button.Click += (s, e) => MenuButton_Click(entry.Action);

            return button;
        }

        private async void MenuButton_Click(MenuAction action)
        {
            if (action == ActionCatalog.Back)
            {
                ShowEntries(_viewModel.Entries);
                return;
            }

            if (action.Children.Count > 0)
            {
                // e.g. Power: swap the ring to its choices plus Back
                var entries = action.Children
                    .Select(child => new MenuEntry(child, child.Glyph))
                    .Append(new MenuEntry(ActionCatalog.Back, ActionCatalog.Back.Glyph))
                    .ToList();

                ShowEntries(entries);
                return;
            }

            CloseMenu();

            // Let the menu fade out first, so it isn't in a screenshot
            await Task.Delay(250);

            try
            {
                action.Run?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    Window.GetWindow(this),
                    $"Couldn't run \"{action.Name}\".\n\n{ex.Message}",
                    "FloatDesk",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ProximityTimer_Tick(object? sender, EventArgs e)
        {
            if (PresentationSource.FromVisual(this) == null) return;

            // Both points are in screen pixels; convert the gap to DIPs.
            var cursor = CursorPosition.GetScreenPosition();
            var anchor = Root.PointToScreen(new Point(CenterX, CenterY));
            double dpiScale = VisualTreeHelper.GetDpi(this).DpiScaleX;
            double distance = (cursor - anchor).Length / dpiScale;

            if (_isOpen && !IsDragging && distance > CloseMenuDistance)
            {
                CloseMenu();
            }

            bool isNear = distance < (_isExpanded ? HideDistance : RevealDistance);
            SetExpanded(_isOpen || IsDragging || isNear);
        }

        /// <summary>
        /// Slides the button fully out, or back to a half circle at the edge.
        /// </summary>
        private void SetExpanded(bool expanded)
        {
            if (expanded == _isExpanded) return;
            _isExpanded = expanded;

            PeekShift.BeginAnimation(TranslateTransform.XProperty,
                new DoubleAnimation(expanded ? 0 : PeekOffset, TimeSpan.FromMilliseconds(180))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                });
        }

        private void AnimateIcon(bool open)
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            IconRotate.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(open ? 90 : 0, _time) { EasingFunction = ease });
            MainIcon.BeginAnimation(OpacityProperty, new DoubleAnimation(open ? 0 : 1, _time));
            CloseIcon.BeginAnimation(OpacityProperty, new DoubleAnimation(open ? 1 : 0, _time));
        }
    }
}
