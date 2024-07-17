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
        songFile.Close();
        // parsing and decoding the MP3 file - start with reading up on the file formats
        // MP3 File consists of MP3 Frames 
        // Header and data. Header is always 4 bytes and contains important meta data 
        

        return Mp3Bytes;
    }

    public byte[] getSongInfo(){
        return decodeSong(_song);
    }

    public override string ToString(){
        return name;
    }
}