namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
                .First(a => a.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs
                    .Select(s => new
                    {
                        NameSong = s.Name,
                        SongPrice = s.Price,
                        SongWriterName = s.Writer.Name

                    })
                    .OrderByDescending(s => s.NameSong)
                    .ThenBy(s=>s.SongWriterName),
                    AlbumPrice = a.Price

                })
                .OrderByDescending(s => s.AlbumPrice);

            StringBuilder sb = new StringBuilder();
            foreach (var album in albums) 
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                if (album.Songs.Any())
                {
                    int countSongs = 0;
                    foreach (var song in album.Songs)
                    {
                        countSongs++;
                        sb.AppendLine($"---#{countSongs}");
                        sb.AppendLine($"---SongName: {song.NameSong}");
                        sb.AppendLine($"---Price: {song.SongPrice:f2}");
                        sb.AppendLine($"---Writer: {song.SongWriterName}");
                       
                    }
                }
                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .AsEnumerable()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    WriterName = s.Writer.Name,
                    Performers = s.SongPerformers
                    .Select(p => new
                    {
                        PerformerFullName = $"{p.Performer.FirstName} {p.Performer.LastName}"
                    })
                    .OrderBy(p => p.PerformerFullName)
                    .ToArray(),
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToArray();
            int countSongs = 0;
            StringBuilder sb= new StringBuilder();
            foreach (var song in songs) 
            {
                countSongs++;
                sb.AppendLine($"-Song #{countSongs}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                if (song.Performers.Any())
                {
                    foreach (var performer in song.Performers)
                    {
                        sb.AppendLine($"---Performer: {performer.PerformerFullName}");
                    }
                }
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
