using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xune.Views
{
    public class AlbumView : UserControl
    {
        public AlbumView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}