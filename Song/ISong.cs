namespace MusicPlayer;

public interface ISong
{
    byte[] decodeSong(FileStream songFile);
    byte[] getSongInfo();
};