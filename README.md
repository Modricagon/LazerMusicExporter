# **LazerMusicExporter**

Music Exporter for Osu! Lazer

This project's aim is to provide a tool for exporting song files from the game **Osu! Lazer**.

**All** valid songs can be exported or a subset from your in game **collections**!

Exports include (where available)
- Song name
- Song title
- Song artwork
- Song metadata, album, artist, bpm... etc.

## Collections

When exporting collections, the album name metadata will be replaced with the name of your ingame Osu! collection! This way you can make your own custom albums which can all be bundled together and recognised in other software or on other devices.

# Configuration / How to use!
To configure the application you should find the appsettings.json file.

```
{
	"osuDirectory": "",
	"outputDirectory": "",
    "overwriteMode": 1,
	"includeArtwork": true,
	"customArtwork": "",
	"namingScheme": "english",
	"trackLengthSimilarity": 10,
	"preferHigherBitrate": true,
	"minimumSongLength": 120,
	"collections": []
}
```

| Configuration Entry | Description | Required | Accepted Value Examples | Default Value |
| ----------- | ----------- | ----------- | ----------- | ----------- |
| osuDirectory | Directory of your Osu! Lazer installation | Yes | C:\OsuLazer | - |
| outputDirectory | Root directory for your file to be outputted | Yes | C:\Users\name\Desktop\output | - |
| overwriteMode | Mode which will be used for overwriting existing files. See OverwriteMode Section | Yes | 0 / 1 | - |
| includeArtwork | Flag for including the beatmap background on outputted files | No | true, false | true |
| customArtwork | Path to custom artwork image which will be used in place of beatmap background images | No | C:\Pictures\MyCustomArtwork.png | - |
| namingScheme | Preferred naming scheme. See NamingScheme Section | No | english / native  | english |
| trackLengthSimilarity | How close, in seconds, two song files (with the same name), length must be to be considered the same song | No | Any number of seconds | 10 |
| preferHigherBitrate | Overwrite lower bitrate versions if a higher bitrate version is found | No | true / false | true |
| minimumSongLength | Minimum length, in seconds, for a song to be exported (to exclude TV Size songs for example) | No | Any number of seconds | - |
| collections | Json Array of Collection Names. Leaving this array empty or omitting this entry will export **ALL** songs | No | [ ] / [ "collection1", "collection2", ... ] | - |


## Overwrite Mode
There are different modes for overwriting existing files. Each is represented by an enum where the number can be used in the configuration file.
```

    /// <summary>
    /// All Files will be kept and numbered if names are the same
    /// </summary>
    None = 0,

    /// <summary>
    /// Files will be overwritten if deemed the same by other parameters from configuration
    /// </summary>
    IgnoreIfSame = 1
```

## Naming Scheme
| Naming Scheme | Description | Example |
| ----- | ----- | ----- |
| english | Prefer song titles and metadata using english characters | Shouta Kageyama - Relic Song |
| native | Prefer song titles and metadata using original language characters | 景山将太 - いにしえのうた |

## Example Configuration for exporting all maps
```
{
	"osuDirectory": "C:\Users\YourUserName\AppData\Roaming\osu",
	"outputDirectory": "C:\Users\YourUserName\Desktop\Output",
    "overwriteMode": 1,
	"includeArtwork": true,
	"customArtwork": "",
	"namingScheme": "english",
	"trackLengthSimilarity": 10,
	"preferHigherBitrate": true,
	"minimumSongLength": 120,
	"collections": [ ]
}
```

# **Now run it!**

### **Important Note**
*I am not responsible for how you use this tool. This tool merely provides a method for exporting files which are already on your computer.*
