using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace iTunesSearch.Library.Models
{
    [DataContract]
    public class TVEpisode
    {
        [DataMember(Name = "artistId")]
        public long ShowId { get; set; }

        [DataMember(Name = "collectionId")]
        public long SeasonId { get; set; }

        [DataMember(Name = "trackId")]
        public long EpisodeId { get; set; }

        [DataMember(Name = "artistName")]
        public string ShowName { get; set; }

        [DataMember(Name = "collectionName")]
        public string SeasonName { get; set; }

        [DataMember(Name="trackName")]
        public string Name { get; set; }

        [DataMember(Name = "collectionCensoredName")]
        public string SeasonCensoredName { get; set; }

        [DataMember(Name = "trackCensoredName")]
        public string CensoredName { get; set; }

        [DataMember(Name = "artistViewUrl")]
        public string ShowViewUrl { get; set; }

        [DataMember(Name = "collectionViewUrl")]
        public string SeasonViewUrl { get; set; }

        [DataMember(Name = "trackViewUrl")]
        public string ViewUrl { get; set; }

        [DataMember(Name = "previewUrl")]
        public string PreviewUrl { get; set; }

        [DataMember(Name = "artworkUrl100")]
        public string ArtworkUrl { get; set; }

        [DataMember(Name = "collectionPrice")]
        public decimal SeasonPrice { get; set; }

        [DataMember(Name = "trackPrice")]
        public decimal Price { get; set; }

        [DataMember(Name = "collectionHdPrice")]
        public decimal SeasonPriceHD { get; set; }

        [DataMember(Name = "trackHdPrice")]
        public decimal PriceHD { get; set; }

        [DataMember(Name = "releaseDate")]
        public string ReleaseDate { get; set; }

        [DataMember(Name = "collectionExplicitness")]
        public string SeasonExplicitness { get; set; }

        [DataMember(Name = "trackExplicitness")]
        public string Explicitness { get; set; }

        [DataMember(Name = "trackCount")]
        public int SeasonEpisodeCount { get; set; }

        [DataMember(Name = "trackNumber")]
        public int Number { get; set; }

        [DataMember(Name = "trackTimeMillis")]
        public long RuntimeInMilliseconds { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "primaryGenreName")]
        public string Genre { get; set; }

        [DataMember(Name = "contentAdvisoryRating")]
        public string Rating { get; set; }

        [DataMember(Name = "shortDescription")]
        public string DescriptionShort { get; set; }

        [DataMember(Name = "longDescription")]
        public string DescriptionLong { get; set; }

        /// <summary>
        /// The parsed large artwork url, based on the regular artwork url
        /// </summary>
        public string ArtworkLargeUrl 
        {
            get
            {
                string retval = string.Empty;

                if(!string.IsNullOrEmpty(this.ArtworkUrl))
                {
                    retval = this.ArtworkUrl.Replace("100x100", "600x600");
                }

                return retval;
            }
        }

        /// <summary>
        /// The parsed season number, based on the season name
        /// </summary>
        public int SeasonNumber 
        {
            get 
            {
                int retval = 0;

                //  See if we can parse the season number from the season name
                try
                {
                    string newString = Regex.Replace(this.SeasonName, "[^.0-9]", "");
                    retval = Convert.ToInt32(newString);
                }
                catch(Exception)
                { /* Don't do anything */ }

                return retval;
            }
        }
    }
}
