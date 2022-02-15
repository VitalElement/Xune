using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xune.Views
{
    public class TrackStatusView : UserControl
    {
        public TrackStatusView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}