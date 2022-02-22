// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Xune.ViewModels;

namespace Xune
{
    public class ViewLocator : IDataTemplate
    {
        private Dictionary<Type, Type> viewMapping = new()
        {
            // Converntional mapping.
            { typeof(AlbumViewModel), typeof(Views.AlbumView) },
            { typeof(CollectionExplorerViewModel), typeof(Views.CollectionExplorerView) },
            { typeof(TrackStatusViewModel), typeof(Views.TrackStatusView) },
            { typeof(VolumeControlViewModel), typeof(Views.VolumeControlView) },

            // Non-conventional mapping.
            { typeof(MainViewModel), typeof(Gui.Views.MainView) },
            { typeof(SelectArtworkViewModel), typeof(Views.SelectAlbumArtView) },
        };

        public bool SupportsRecycling => false;


        public IControl Build(object data)
        {
            var viewModelType = data.GetType();
            if (viewMapping.TryGetValue(viewModelType, out var viewType))
            {
                return (Control)Activator.CreateInstance(viewType);
            }

            var name = viewModelType.FullName.Replace("ViewModel", "View");
            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}