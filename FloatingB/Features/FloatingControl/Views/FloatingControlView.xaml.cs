using System.Windows.Controls;
using FloatingB.Features.FloatingControl.ViewModels;

namespace FloatingB.Features.FloatingControl.Views
{
    public partial class FloatingControlView : UserControl
    {
        public FloatingControlView()
        {
            InitializeComponent();

            DataContext = new FloatingControlViewModel();
        }
    }
}