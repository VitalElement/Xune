using System.Collections.Generic;

namespace Xune.Backend
{
    public interface ITrackList
    {
        IList<Track> Tracks { get; }
    }
}