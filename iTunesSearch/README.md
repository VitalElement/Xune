iTunesSearch [![Build status](https://ci.appveyor.com/api/projects/status/qoe200t6mxxwieic?svg=true)](https://ci.appveyor.com/project/danesparza/itunessearch) [![NuGet](https://img.shields.io/nuget/v/iTunesSearch.svg)](https://www.nuget.org/packages/iTunesSearch/)
============

A .NET wrapper to the iTunes search API

### Quick Start

Install the [NuGet package](https://www.nuget.org/packages/iTunesSearch/) from the package manager console:

```powershell
Install-Package iTunesSearch
```

### Examples

##### Get TV episodes for a show name:
```csharp
//  Arrange
iTunesSearchManager search = new iTunesSearchManager();
string showName = "Modern Family";

//  Act
var items = search.GetTVEpisodesForShow(showName, 200).Result;

//  Assert
Assert.IsTrue(items.Episodes.Any());
```

##### Group episodes for a season:
```csharp
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
foreach(var seasonGroup in seasons)
{
    Debug.WriteLine("Season number: {0}", seasonGroup.Key);

    foreach(TVEpisode episode in seasonGroup)
    {
        Debug.WriteLine("Ep {0}: {1}", episode.Number, episode.Name);
    }
}
```

##### Get TV seasons for a show:
```csharp
 //  Arrange
iTunesSearchManager search = new iTunesSearchManager();
string showName = "Modern Family";

//  Act
var items = search.GetTVSeasonsForShow(showName).Result;

//  Assert
Assert.IsTrue(items.Seasons.Any());
```

##### Get TV seasons for a given show name and country code:
```csharp
 //  Arrange
iTunesSearchManager search = new iTunesSearchManager();
string showName = "King of the Hill";
string countryCode = "AU"; /* Australia */

//  Act
var items = search.GetTVSeasonsForShow(showName, 20, countryCode).Result;

//  Assert
Assert.IsTrue(items.Seasons.Any());
```

##### Get podcasts for a given name:
```csharp
//  Arrange
iTunesSearchManager search = new iTunesSearchManager();
string showName = "Radiolab";

//  Act
var items = search.GetPodcasts(showName, 200).Result;

//  Assert
Assert.IsTrue(items.Podcasts.Any());
```
