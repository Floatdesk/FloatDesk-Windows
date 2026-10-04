using System.Windows;
using System.Windows.Input;
using FloatingB.Core.Interfaces;
using FloatingB.Infrastructure.Windows;

namespace FloatingB
{
    public partial class MainWindow : Window
    {
        private readonly IWindowPositionService _windowPositionService;

        public MainWindow()
        {
            InitializeComponent();

            _windowPositionService =
                new WindowPositionService();

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
                // Allow the entire floating control
                // to be dragged.
                DragMove();


                // After releasing the mouse,
                // snap to the nearest screen edge.
                _windowPositionService
                    .SnapToNearestEdge(this);
            }
            catch
            {
                // Windows may cancel a drag operation.
            }
        }
    }
}