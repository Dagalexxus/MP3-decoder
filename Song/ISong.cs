namespace MusicPlayer;

public interface ISong
{
    byte[] getSongInfo();
    List<Mp3Frame> decodeSong();
};