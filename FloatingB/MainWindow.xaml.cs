using System.Windows;
using System.Windows.Input;
using FloatingB.Core.Interfaces;

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


            try
            {
                double startLeft = Left;
                double startTop = Top;

                // Remember if the menu was open, then close it
                bool wasOpen = FloatingControl.IsMenuOpen;
                FloatingControl.CloseMenu();

                DragMove();

                if (Left == startLeft && Top == startTop)
                {
                    // The window did not move, so it was a click.
                    // If the menu was closed before, open it now.
                    if (!wasOpen)
                    {
                        FloatingControl.ToggleMenu();
                    }
                }

                _windowPositionService.SnapToNearestEdge(this);
            }
            catch
            {
                // Windows may cancel a drag operation.
            }
        }
    }
}