using Avalonia.Input;
using Xune.Backend;
using Xune.ViewModels;

namespace Xune.Behaviors
{
    /// <summary>
    ///     Project editor drop handler.
    /// </summary>
    public class TrackDropHandler : DefaultDropHandler
    {
        /// <inheritdoc />
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext,
            object state)
        {
            if (sourceContext is AlbumViewModel avm && avm.Model is ITrackList tl &&
                targetContext is TrackStatusViewModel ts) return true;
            return false;
        }

        /// <inheritdoc />
        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext,
            object state)
        {
            if (sourceContext is AlbumViewModel avm && avm.Model is ITrackList tl &&
                targetContext is TrackStatusViewModel ts)
            {
                ts.Model.AppendTrackList(avm.Model);
                return true;
            }

            return false;
        }
    }
}