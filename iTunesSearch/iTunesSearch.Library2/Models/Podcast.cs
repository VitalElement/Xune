using System;
using System.Runtime.Serialization;

namespace iTunesSearch.Library.Models
{
    [DataContract]
    public class Podcast
    {
        [DataMember(Name = "artistId")]
        public long ArtistId { get; set; }

        [DataMember(Name = "collectionId")]
        public long Id { get; set; }

        [DataMember(Name = "artistName")]
        public string ArtistName { get; set; }

        [DataMember(Name = "collectionName")]
        public string Name { get; set; }

        [DataMember(Name = "collectionCensoredName")]
        public string CensoredName { get; set; }

        [DataMember(Name = "artistViewUrl")]
        public string ArtistViewUrl { get; set; }

        [DataMember(Name = "collectionViewUrl")]
        public string PodcastViewUrl { get; set; }

        [DataMember(Name = "artworkUrl100")]
        public string ArtworkUrl { get; set; }

        [DataMember(Name = "releaseDate")]
        public string ReleaseDate { get; set; }

        [DataMember(Name = "collectionExplicitness")]
        public string Explicitness { get; set; }

        [DataMember(Name = "trackCount")]
        public int EpisodeCount { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "primaryGenreName")]
        public string Genre { get; set; }

        [DataMember(Name = "contentAdvisoryRating")]
        public string Rating { get; set; }

        [DataMember(Name = "copyright")]
        public string Copyright { get; set; }

        [DataMember(Name = "longDescription")]
        public string Description { get; set; }

        [DataMember(Name = "feedUrl")]
        public string FeedUrl { get; set; }

        /// <summary>
        /// The parsed 'large' artwork url
        /// </summary>
        public string ArtworkUrlLarge
        {
            get 
            {
                string retval = string.Empty;

                //  See if we can parse the large artwork url from the regular artwork url
                try
                {
                    retval = this.ArtworkUrl.Replace("100x100", "600x600");
                }
                catch(Exception)
                { }

                return retval;
            }
        }
    }
}