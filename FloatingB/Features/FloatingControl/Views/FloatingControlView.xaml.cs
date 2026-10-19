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

        public bool IsMenuOpen => _isOpen;

        public FloatingControlView()
        {
            InitializeComponent();

            DataContext = new FloatingControlViewModel();
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
            MenuScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.3, _time));
            MenuScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.3, _time));
        }

        private async void Screenshot_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            CloseMenu();

            // Wait a moment so the menu is gone before the snipping starts
            await System.Threading.Tasks.Task.Delay(300);
            ((FloatingControlViewModel)DataContext).Screenshot();
        }

        private void Lock_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            CloseMenu();
            ((FloatingControlViewModel)DataContext).Lock();
        }

        private void Power_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
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