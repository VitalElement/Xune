using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace iTunesSearch.Library.Models
{
    [DataContract]
    public class TVSeason
    {
        [DataMember(Name = "artistId")]
        public long ShowId { get; set; }

        [DataMember(Name = "collectionId")]
        public long SeasonId { get; set; }

        [DataMember(Name = "artistName")]
        public string ShowName { get; set; }

        [DataMember(Name = "collectionName")]
        public string SeasonName { get; set; }

        [DataMember(Name = "collectionCensoredName")]
        public string SeasonCensoredName { get; set; }

        [DataMember(Name = "artistViewUrl")]
        public string ShowViewUrl { get; set; }

        [DataMember(Name = "collectionViewUrl")]
        public string SeasonViewUrl { get; set; }

        [DataMember(Name = "artworkUrl100")]
        public string ArtworkUrl { get; set; }

        [DataMember(Name = "collectionPrice")]
        public decimal SeasonPrice { get; set; }

        [DataMember(Name = "collectionHdPrice")]
        public decimal SeasonPriceHD { get; set; }

        [DataMember(Name = "releaseDate")]
        public string ReleaseDate { get; set; }

        [DataMember(Name = "collectionExplicitness")]
        public string SeasonExplicitness { get; set; }

        [DataMember(Name = "trackCount")]
        public int SeasonEpisodeCount { get; set; }

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
