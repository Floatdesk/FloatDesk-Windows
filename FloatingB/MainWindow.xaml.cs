using System.IO;
using System.Windows;
using System.Windows.Input;
using FloatingB.Core.Interfaces;
using FloatingB.Core.Models;
using FloatingB.Features.Settings.Views;

namespace FloatingB
{
    public partial class MainWindow : Window
    {
        private readonly IWindowPositionService _windowPositionService;
        private readonly ISettingsService _settingsService;
        private AppSettings _settings;

        public MainWindow(
            IWindowPositionService windowPositionService,
            ISettingsService settingsService)
        {
            InitializeComponent();

            _windowPositionService = windowPositionService;
            _settingsService = settingsService;

            _settings = _settingsService.Load();
            FloatingControl.ApplySettings(_settings);

            Loaded += MainWindow_Loaded;

            MouseLeftButtonDown +=
                MainWindow_MouseLeftButtonDown;
        }


        private void Customize_Click(
            object sender,
            RoutedEventArgs e)
        {
            FloatingControl.CloseMenu();

            var settingsWindow = new SettingsWindow(_settings) { Owner = this };

            if (settingsWindow.ShowDialog() != true ||
                settingsWindow.Result == null)
            {
                return;
            }

            _settings = settingsWindow.Result;
            FloatingControl.ApplySettings(_settings);

            try
            {
                _settingsService.Save(_settings);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(
                    this,
                    $"Your changes are applied, but they couldn't be saved for next time.\n\n{ex.Message}",
                    "FloatDesk",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }


        private void Exit_Click(
            object sender,
            RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            PositionInitialWindow();
        }


        private void PositionInitialWindow()
        {
            DockTo(ScreenEdge.Right);

            var workArea =
                SystemParameters.WorkArea;

            // Vertically centered
            Top =
                workArea.Top +
                (workArea.Height - Height) / 2;
        }


        /// <summary>
        /// Turns the control to face the edge, resizes the window to
        /// match, and moves it flush against that edge.
        /// </summary>
        private void DockTo(ScreenEdge edge)
        {
            FloatingControl.SetEdge(edge);

            var size = FloatingControl.LayoutSize;
            Width = size.Width;
            Height = size.Height;

            _windowPositionService.PositionAtEdge(this, edge);
        }


        private void MainWindow_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.LeftButton !=
                MouseButtonState.Pressed)
            {
                return;
            }


            double startLeft = Left;
            double startTop = Top;

            // Remember if the menu was open, then close it
            bool wasOpen = FloatingControl.IsMenuOpen;
            FloatingControl.CloseMenu();

            FloatingControl.IsDragging = true;

            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
                // On a very quick click the button can already be
                // released here. Treat it as a click, not a drag.
            }
            finally
            {
                FloatingControl.IsDragging = false;
            }

            if (Left == startLeft && Top == startTop)
            {
                // The window did not move, so it was a click.
                // If the menu was closed before, open it now.
                if (!wasOpen)
                {
                    FloatingControl.OpenMenu();
                }

                return;
            }

            DockTo(_windowPositionService.GetNearestEdge(this));
        }
    }
}
