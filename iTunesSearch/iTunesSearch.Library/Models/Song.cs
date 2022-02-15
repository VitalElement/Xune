using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iTunesSearch.Library.Models
{
    [DataContract]
    public class Song
    {
        [DataMember(Name = "artistName")]
        public string ArtistName { get; set; }

        [DataMember(Name = "artistId")]
        public long ArtistId { get; set; }

        [DataMember(Name = "trackId")]
        public long TrackId { get; set; }

        [DataMember(Name = "trackName")]
        public string TrackName { get; set; }

        [DataMember(Name = "trackCensoredName")]
        public string TrackCensoredName { get; set; }

        [DataMember(Name = "collectionId")]
        public long CollectionId { get; set; }

        [DataMember(Name = "collectionName")]
        public string CollectionName { get; set; }

        [DataMember(Name = "collectionCensoredName")]
        public string CollectionCensoredName { get; set; }

        [DataMember(Name = "artistViewUrl")]
        public string ArtistViewUrl { get; set; }

        [DataMember(Name = "collectionViewUrl")]
        public string CollectionViewUrl { get; set; }

        [DataMember(Name = "trackViewUrl")]
        public string TrackViewUrl { get; set; }

        [DataMember(Name = "previewUrl")]
        public string PreviewUrl { get; set; }

        [DataMember(Name = "artworkUrl30")]
        public string ArtworkUrl30 { get; set; }

        [DataMember(Name = "artworkUrl60")]
        public string ArtworkUrl60 { get; set; }

        [DataMember(Name = "artworkUrl100")]
        public string ArtworkUrl100 { get; set; }

        [DataMember(Name = "collectionPrice")]
        public double CollectionPrice { get; set; }

        [DataMember(Name = "trackPrice")]
        public double TrackPrice { get; set; }

        [DataMember(Name = "releaseDate")]
        public string ReleaseDate { get; set; }

        [DataMember(Name = "collectionExplicitness")]
        public string CollectionExplicitness { get; set; }

        [DataMember(Name = "trackExplicitness")]
        public string TrackExplicitness { get; set; }

        [DataMember(Name = "discCount")]
        public int DiscCount { get; set; }

        [DataMember(Name = "discNumber")]
        public int DiscNumber { get; set; }

        [DataMember(Name = "trackCount")]
        public int TrackCount { get; set; }

        [DataMember(Name = "trackNumber")]
        public int TrackNumber { get; set; }

        [DataMember(Name = "trackTimeMillis")]
        public long TrackTimeMilliseconds { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "primaryGenreName")]
        public string PrimaryGenreName { get; set; }

        [DataMember(Name = "isStreamable")]
        public bool IsStreamable { get; set; }
    }
}
