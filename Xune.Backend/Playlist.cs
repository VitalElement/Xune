using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xune.Backend
{
    public class Playlist : ITrackList
    {
        public Playlist()
        {
            Tracks = new ObservableCollection<Track>();
        }

        public ObservableCollection<Track> Tracks { get; set; }

        IList<Track> ITrackList.Tracks => Tracks;

        public void AddTracks(ITrackList tracks)
        {
            foreach (var track in tracks.Tracks) Tracks.Add(track);
        }

        public void Clear()
        {
            Tracks.Clear();
        }
    }
}