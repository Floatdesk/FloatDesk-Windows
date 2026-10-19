using System.Windows;
using System.Windows.Input;
using FloatingB.Core.Interfaces;
using FloatingB.Core.Models;

namespace FloatingB
{
    public partial class MainWindow : Window
    {
        private readonly IWindowPositionService _windowPositionService;

        public MainWindow(IWindowPositionService windowPositionService)
        {
            InitializeComponent();

            _windowPositionService = windowPositionService;

            Loaded += MainWindow_Loaded;

            MouseLeftButtonDown +=
                MainWindow_MouseLeftButtonDown;
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
            var workArea =
                SystemParameters.WorkArea;

            // Start on the right side
            Left =
                workArea.Right -
                Width;

            // Vertically centered
            Top =
                workArea.Top +
                (workArea.Height - Height) / 2;
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

            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
                // On a very quick click the button can already be
                // released here. Treat it as a click, not a drag.
            }

            if (Left == startLeft && Top == startTop)
            {
                // The window did not move, so it was a click.
                // If the menu was closed before, open it now.
                if (!wasOpen)
                {
                    FloatingControl.ToggleMenu();
                }

                return;
            }

            var edge = _windowPositionService.SnapToNearestEdge(this);

            // Open the menu away from the edge it is docked to
            FloatingControl.SetOpensToRight(edge == ScreenEdge.Left);
        }
    }
}