namespace MusicPlayer;

class Program
{
    static void Main(string[] args)
    {
        Song currentSong = new Song("/Users/niklas/Documents/coding/csharp/MusicPlayer/assets/tvari-tokyo-cafe-159065.mp3", "tokyo cafe");
        Console.WriteLine("Song Opened.");
    }
}
