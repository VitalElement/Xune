using iTunesSearch.Library.Models;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace iTunesSearch.Library
{
    /// <summary>
    /// A wrapper for the iTunes search API.  More information here:
    /// https://www.apple.com/itunes/affiliates/resources/documentation/itunes-store-web-service-search-api.html
    /// </summary>
    public class iTunesSearchManager
    {
        /// <summary>
        /// The base API url for iTunes search
        /// </summary>
        private string _baseSearchUrl = "https://itunes.apple.com/search?{0}";

        /// <summary>
        /// The base API url for iTunes lookups
        /// </summary>
        private string _baseLookupUrl = "https://itunes.apple.com/lookup?{0}";

        #region Podcast Search

        /// <summary>
        /// Get a list of episodes for a given Podcast
        /// </summary>
        /// <param name="podcast">The Podcast name to search for</param>
        /// <param name="resultLimit">Limit the result count to this number</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<PodcastListResult> GetPodcasts(string podcast, int resultLimit = 100, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", podcast);
            nvc.Add("media", "podcast");
            nvc.Add("attribute", "titleTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<PodcastListResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Looks up a Podcast by its unique iTunes id
        /// </summary>
        /// <param name="podcastId"></param>
        /// <returns></returns>
        public async Task<PodcastListResult> GetPodcastById(long podcastId)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a podcast
            nvc.Add("id", podcastId.ToString());

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the list of podcasts
            var result = await MakeAPICall<PodcastListResult>(apiUrl);
            return result;
        }

        #endregion Podcast Search

        #region TV shows

        /// <summary>
        /// Get a list of episodes for a given TV show
        /// </summary>
        /// <param name="showName">The TV show name to search for</param>
        /// <param name="resultLimit">Limit the result count to this number</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<TVEpisodeListResult> GetTVEpisodesForShow(string showName, int resultLimit = 100, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a TV season.
            nvc.Add("term", showName);
            nvc.Add("media", "tvShow");
            nvc.Add("entity", "tvEpisode");
            nvc.Add("attribute", "showTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<TVEpisodeListResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Get a list of seasons for a given TV show
        /// </summary>
        /// <param name="showName">The TV show name to search for</param>
        /// <param name="resultLimit">Limit the result count to this number</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<TVSeasonListResult> GetTVSeasonsForShow(string showName, int resultLimit = 10, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a TV season.
            nvc.Add("term", showName);
            nvc.Add("media", "tvShow");
            nvc.Add("entity", "tvSeason");
            nvc.Add("attribute", "showTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<TVSeasonListResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Looks up a TV show season by its unique iTunes season id
        /// </summary>
        /// <param name="seasonId"></param>
        /// <returns></returns>
        public async Task<TVSeasonListResult> GetTVSeasonById(long seasonId)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a TV season.
            nvc.Add("id", seasonId.ToString());

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<TVSeasonListResult>(apiUrl);

            return result;
        }

        #endregion TV shows

        #region Music Search

        /// <summary>
        /// Looks up an artist by its unique iTunes id.
        /// </summary>
        /// <param name="artistId">The artist id value.</param>
        /// <returns></returns>
        public async Task<SongArtist> GetSongArtistByArtistIdAsync(long artistId)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("id", artistId.ToString());

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the artist
            var result = (await MakeAPICall<SongArtistResult>(apiUrl)).Artists.FirstOrDefault();

            return result ?? new SongArtist();
        }

        /// <summary>
        /// Looks up an artist by its unique AMG id.
        /// </summary>
        /// <param name="amgArtistId">The AMG id value.</param>
        /// <returns></returns>
        public async Task<SongArtist> GetSongArtistByAMGArtistIdAsync(long amgArtistId)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("amgArtistId", amgArtistId.ToString());

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the artist
            var result = (await MakeAPICall<SongArtistResult>(apiUrl)).Artists.FirstOrDefault();

            return result ?? new SongArtist();
        }

        /// <summary>
        /// Looks up albums for an artist by its unique iTunes id.
        /// </summary>
        /// <param name="artistId">The artist id value.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<AlbumResult> GetAlbumsByArtistIdAsync(long artistId, int resultLimit = 100, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("id", artistId.ToString());
            nvc.Add("entity", "album");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the albums
            var result = await MakeAPICall<AlbumResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Looks up albums for an artist by its unique AMG id.
        /// </summary>
        /// <param name="amgArtistId">The AMG id value.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<AlbumResult> GetAlbumsByAMGArtistIdAsync(long amgArtistId, int resultLimit = 100, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("amgArtistId", amgArtistId.ToString());
            nvc.Add("entity", "album");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseLookupUrl, nvc.ToString());

            //  Get the albums
            var result = await MakeAPICall<AlbumResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Gets a list of artists for a given search term
        /// </summary>
        /// <param name="artist">The artist name to search for.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<SongArtistResult> GetSongArtistsAsync(string artist, int resultLimit = 100, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", artist);
            nvc.Add("media", "music");
            nvc.Add("entity", "musicArtist");
            nvc.Add("attribute", "artistTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<SongArtistResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Gets a list of songs for a given search term.
        /// </summary>
        /// <param name="song">The song name to search for.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<SongResult> GetSongsAsync(string song, int resultLimit = 100, string attribute = null, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", song);
            nvc.Add("media", "music");
            nvc.Add("entity", "musicTrack");
            if (attribute != null)
            {
                nvc.Add("attribute", attribute);
            }
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<SongResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Gets a list of albums for a given search term.
        /// </summary>
        /// <param name="album">The album name to search for.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<AlbumResult> GetAlbumsAsync(string album, int resultLimit = 100, string attribute = null, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", album);
            nvc.Add("media", "music");
            nvc.Add("entity", "album");
            if (attribute != null)
            {
                nvc.Add("attribute", attribute);
            }
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<AlbumResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Gets a list of albums for a given search term.
        /// </summary>
        /// <param name="song">The song name featured in albums to search for.</param>
        /// <param name="resultLimit">Limit the result count to this number.</param>
        /// <param name="countryCode">The two-letter country ISO code for the store you want to search.
        /// See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes</param>
        /// <returns></returns>
        public async Task<AlbumResult> GetAlbumsFromSongAsync(string song, int resultLimit = 100, string attribute = null, string countryCode = "us")
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", song);
            nvc.Add("media", "music");
            nvc.Add("entity", "album");
            if (attribute != null)
            {
                nvc.Add("attribute", attribute);
            }
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<AlbumResult>(apiUrl);

            return result;
        }
        #endregion

        #region API helpers

        /// <summary>
        /// Makes an API call and deserializes return value to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiCall"></param>
        /// <returns></returns>
        async private Task<T> MakeAPICall<T>(string apiCall)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            //  Make an async call to get the response
            var objString = await client.GetStringAsync(apiCall).ConfigureAwait(false);

            //  Deserialize and return
            return (T)DeserializeObject<T>(objString);
        }

        /// <summary>
        /// Deserializes the JSON string to the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objString"></param>
        /// <returns></returns>
        private T DeserializeObject<T>(string objString)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        #endregion API helpers
    }
}
