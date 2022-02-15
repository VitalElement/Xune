using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Xune.Views
{
    public class CollectionExplorerView : UserControl
    {
        public CollectionExplorerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}