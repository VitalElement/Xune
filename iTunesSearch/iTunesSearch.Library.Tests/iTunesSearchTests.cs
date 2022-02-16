using iTunesSearch.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;

namespace iTunesSearch.Library.Tests
{
    [TestClass]
    public class iTunesSearchTests
    {
        [TestMethod]
        public void GetArtistById_ValidArtist_ReturnsArtist()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long artistId = 311145;

            // Act
            var item = search.GetSongArtistByArtistIdAsync(artistId).Result;

            // Assert
            Assert.IsTrue(item.ArtistName == "R.E.M.");
        }

        [TestMethod]
        public void GetArtistById_InvalidArtist_ReturnsDefaultInstanceOfSongArtist()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long artistId = 5858500001;

            // Act
            var item = search.GetSongArtistByArtistIdAsync(artistId).Result;

            // Assert
            Assert.IsInstanceOfType(item, typeof(SongArtist));
            Assert.IsTrue(item.ArtistName == default(string));
        }

        [TestMethod]
        public void GetArtistByAMGArtistId_ValidArtist_ReturnsArtist()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long amgArtistId = 116437;

            // Act
            var item = search.GetSongArtistByAMGArtistIdAsync(amgArtistId).Result;

            // Assert
            Assert.IsTrue(item.ArtistName == "R.E.M.");
        }

        [TestMethod]
        public void GetArtistByAMGArtistId_InvalidArtist_ReturnsDefaultInstanceOfSongArtist()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long amgArtistId = 5858500001;

            // Act
            var item = search.GetSongArtistByAMGArtistIdAsync(amgArtistId).Result;

            // Assert
            Assert.IsInstanceOfType(item, typeof(SongArtist));
            Assert.IsTrue(item.ArtistName == default(string));
        }

        [TestMethod]
        public void GetAlbumsByArtistId_ValidArtist_ReturnsAlbums()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long artistId = 311145;

            // Act
            var items = search.GetAlbumsByArtistIdAsync(artistId, 200).Result;

            // Assert
            Assert.IsTrue(items.Albums.Any(al => al.CollectionName == "Life's Rich Pageant"));
        }

        [TestMethod]
        public void GetAlbumsByAMGArtistId_ValidArtist_ReturnsAlbums()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long amgArtistId = 116437;

            // Act
            var items = search.GetAlbumsByAMGArtistIdAsync(amgArtistId, 200).Result;

            // Assert
            Assert.IsTrue(items.Albums.Any(al => al.CollectionName == "Life's Rich Pageant"));
        }

        [TestMethod]
        public void GetSongArtists_ValidArtists_ReturnsArtists()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string artist = "R.E.M.";

            // Act
            var items = search.GetSongArtistsAsync(artist, 200).Result;

            // Assert
            Assert.IsTrue(items.Artists.Any());
        }

        [TestMethod]
        public void GetSongs_ValidSongs_ReturnsSongs()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string song = "Driver 8";

            // Act
            var items = search.GetSongsAsync(song, 200).Result;

            // Assert
            Assert.IsTrue(items.Songs.Any());
            Assert.IsTrue(items.Songs.Any(s => s.TrackName == "Driver 8" && s.ArtistName == "R.E.M."));
        }

        [TestMethod]
        public void GetAlbums_ValidAlbums_ReturnsAlbums()
        {
            // Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string album = "Collapse into Now";

            // Act
            var items = search.GetAlbumsAsync(album, 200).Result;

            // Assert
            Assert.IsTrue(items.Albums.Any());
            Assert.IsTrue(items.Albums.Any(al => al.ArtistName == "R.E.M."));
        }

        [TestMethod]
        public void GetTVEpisodesForShow_ValidShow_ReturnsEpisodes()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Modern Family";

            //  Act
            var items = search.GetTVEpisodesForShow(showName, 200).Result;

            //  Assert
            Assert.IsTrue(items.Episodes.Any());
        }

        [TestMethod]
        public void GetTVEpisodesForShow_ValidShow_GroupedEpisodes()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Modern Family";

            //  Act
            var items = search.GetTVEpisodesForShow(showName, 200).Result;
            var seasons = from episode in items.Episodes
                          orderby episode.Number
                          group episode by episode.SeasonNumber into seasonGroup
                          orderby seasonGroup.Key
                          select seasonGroup;

            //  Assert
            foreach (var seasonGroup in seasons)
            {
                Debug.WriteLine("Season number: {0}", seasonGroup.Key);

                foreach (TVEpisode episode in seasonGroup)
                {
                    Debug.WriteLine("Ep {0}: {1}", episode.Number, episode.Name);
                }
            }
        }

        [TestMethod]
        public void GetTVSeasonsForShow_ValidShow_ReturnsShows()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Modern Family";

            //  Act
            var items = search.GetTVSeasonsForShow(showName).Result;

            //  Assert
            Assert.IsTrue(items.Seasons.Any());
        }

        [TestMethod]
        public void GetTVSeasonById_ValidSeasonId_ReturnsShow()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long seasonId = 327827178;

            //  Act
            var items = search.GetTVSeasonById(seasonId).Result;

            //  Assert
            Assert.IsTrue(items.Seasons.Any());
            Assert.AreEqual("Modern Family", items.Seasons.First().ShowName);
        }

        [TestMethod]
        public void GetTVSeasonById_ValidSeasonId_ReturnsCorrectLargeArtwork()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long seasonId = 316075588;
            string expectedShowName = "Gilmore Girls";
            string expectedLargeArtworkUrl = "https://is1-ssl.mzstatic.com/image/thumb/Music71/v4/82/28/39/8228392e-f1f9-3b64-6c0f-14ba1f958a92/source/600x600bb.jpg";

            //  Act
            var items = search.GetTVSeasonById(seasonId).Result;

            //  Assert
            Assert.IsTrue(items.Seasons.Any());
            Assert.AreEqual(expectedShowName, items.Seasons.First().ShowName);
            Assert.AreEqual(expectedLargeArtworkUrl, items.Seasons.First().ArtworkUrlLarge);
        }

        [TestMethod]
        public void GetTVSeasonsForShow_ValidShowAndCountryCode_ReturnsShows()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Top of the Lake";
            string countryCode = "AU"; /* Australia */

            //  Act
            var items = search.GetTVSeasonsForShow(showName, 20, countryCode).Result;

            //  Assert
            Assert.IsTrue(items.Seasons.Any());
        }

        [TestMethod]
        public void GetTVEpisodesForShow_ValidShowAndCountryCode_ReturnsEpisodes()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Top of the Lake";
            string countryCode = "AU"; /* Australia */

            //  Act
            var items = search.GetTVEpisodesForShow(showName, 200, countryCode).Result;

            //  Assert
            Assert.IsTrue(items.Episodes.Any());
        }

        [TestMethod]
        public void GetPodcasts_ValidPodcast_ReturnsEpisodes()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            string showName = "Radiolab";

            //  Act
            var items = search.GetPodcasts(showName, 200).Result;

            //  Assert
            Assert.IsTrue(items.Podcasts.Any());
        }

        [TestMethod]
        public void GetPodcastById_ValidId_ReturnsPodcast()
        {
            //  Arrange
            iTunesSearchManager search = new iTunesSearchManager();
            long podcastId = 1002937870;

            //  Act
            var items = search.GetPodcastById(podcastId).Result;

            //  Assert
            Assert.IsTrue(items.Podcasts.Any());
            Assert.AreEqual("Dear Hank and John", items.Podcasts.First().Name);
            Assert.AreEqual("http://dearhankandjohn.libsyn.com/rss", items.Podcasts.First().FeedUrl);
        }
    }
}