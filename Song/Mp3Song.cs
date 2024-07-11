namespace MusicPlayer;

using System.IO;

public class Mp3Song : ISong
{
    private FileStream _song;
    public string name;

    public Mp3Song(string filepath, string songName){
        _song = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None);
        name = songName;
    }

    public byte[] decodeSong(FileStream songFile){
        byte[] Mp3Bytes = new byte[songFile.Length];
        songFile.Read(Mp3Bytes);
        return Mp3Bytes;
    }

    public override string ToString(){
        return name;
    }
}