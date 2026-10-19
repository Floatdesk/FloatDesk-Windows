using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FloatingB.Features.FloatingControl.ViewModels;

namespace FloatingB.Features.FloatingControl.Views
{
    public partial class FloatingControlView : UserControl
    {
        private bool _isOpen;
        private readonly TimeSpan _time = TimeSpan.FromMilliseconds(200);
        private readonly TimeSpan _stagger = TimeSpan.FromMilliseconds(40);
        private readonly Button[] _menuButtons;

        public bool IsMenuOpen => _isOpen;

        public FloatingControlView()
        {
            InitializeComponent();

            DataContext = new FloatingControlViewModel();

            // Each button needs its own transform so it can pop in on its own
            _menuButtons = new[] { ScreenshotButton, PowerButton, LockButton };
            foreach (var button in _menuButtons)
            {
                button.RenderTransform = new ScaleTransform(1, 1);
            }
        }

        /// <summary>
        /// Mirrors the control so the menu always opens away from the
        /// screen edge. Icons are flipped back so they read normally.
        /// </summary>
        public void SetOpensToRight(bool opensToRight)
        {
            double scaleX = opensToRight ? -1 : 1;

            SideFlip.ScaleX = scaleX;
            IconHost.LayoutTransform = new ScaleTransform(scaleX, 1);

            foreach (var button in _menuButtons)
            {
                if (button.Content is UIElement icon)
                {
                    icon.RenderTransform = new ScaleTransform(scaleX, 1);
                }
            }
        }

        public void ToggleMenu()
        {
            if (_isOpen) CloseMenu();
            else OpenMenu();
        }

        public void OpenMenu()
        {
            _isOpen = true;
            Menu.Visibility = Visibility.Visible;

            // Small bounce at the end of the pop-out
            var bounce = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 };

            Menu.BeginAnimation(OpacityProperty, new DoubleAnimation(1, _time));
            MenuScale.BeginAnimation(ScaleTransform.ScaleXProperty,
                new DoubleAnimation(1, _time) { EasingFunction = bounce });
            MenuScale.BeginAnimation(ScaleTransform.ScaleYProperty,
                new DoubleAnimation(1, _time) { EasingFunction = bounce });

            // Buttons pop in one after another, top to bottom
            for (int i = 0; i < _menuButtons.Length; i++)
            {
                var scale = (ScaleTransform)_menuButtons[i].RenderTransform;
                var popIn = new DoubleAnimation(0, 1, _time)
                {
                    BeginTime = TimeSpan.FromTicks(_stagger.Ticks * (i + 1)),
                    EasingFunction = bounce,
                    FillBehavior = FillBehavior.HoldEnd
                };

                scale.BeginAnimation(ScaleTransform.ScaleXProperty, popIn);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, popIn);
            }

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

        private void AnimateIcon(bool open)
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            IconRotate.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(open ? 90 : 0, _time) { EasingFunction = ease });
            OmegaIcon.BeginAnimation(OpacityProperty, new DoubleAnimation(open ? 0 : 1, _time));
            CloseIcon.BeginAnimation(OpacityProperty, new DoubleAnimation(open ? 1 : 0, _time));
        }

        private async void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();

            // Wait a moment so the menu is gone before the snipping starts
            await System.Threading.Tasks.Task.Delay(300);
            ((FloatingControlViewModel)DataContext).Screenshot();
        }

        private void Lock_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();
            ((FloatingControlViewModel)DataContext).Lock();
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();

            // Ask first so a stray click can't turn the PC off
            var result = MessageBox.Show(
                Window.GetWindow(this),
                "Shut down this PC now?",
                "FloatDesk",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                ((FloatingControlViewModel)DataContext).ShutDown();
            }
        }
    }
}
