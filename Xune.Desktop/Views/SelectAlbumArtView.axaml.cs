using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xune.Views
{
    public class SelectAlbumArtView : UserControl
    {
        public SelectAlbumArtView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}