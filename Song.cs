namespace MusicPlayer;

using System.IO;

public class Song
{
    private FileStream _song;
    public string name;

    public Song(string filepath, string songName){
        _song = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None);
        name = songName;
    }
}