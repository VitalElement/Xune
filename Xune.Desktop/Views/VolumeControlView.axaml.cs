using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xune.Views
{
    public class VolumeControlView : UserControl
    {
        public VolumeControlView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}