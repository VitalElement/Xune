using Xune.Backend;

namespace Xune.ViewModels
{
    public class TrackViewModel
    {
        public TrackViewModel(Track track)
        {
            Model = track;
        }

        public string Title => Model.Title;

        public Track Model { get; }
    }
}