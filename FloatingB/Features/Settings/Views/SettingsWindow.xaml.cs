using System.Windows;
using FloatingB.Core.Models;
using FloatingB.Features.Settings.ViewModels;

namespace FloatingB.Features.Settings.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;

        /// <summary>
        /// The new settings, once the user clicks Save.
        /// </summary>
        public AppSettings? Result { get; private set; }

        public SettingsWindow(AppSettings current)
        {
            InitializeComponent();

            _viewModel = new SettingsViewModel(current);
            DataContext = _viewModel;
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is ActionRow row)
            {
                _viewModel.MoveUp(row);
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is ActionRow row)
            {
                _viewModel.MoveDown(row);
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Load(new AppSettings());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Result = _viewModel.ToSettings();
            DialogResult = true;
        }
    }
}
