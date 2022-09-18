namespace HK_Shared.Source
{
    public struct ManifestEntry : IEquatable<ManifestEntry>
    {
        public string Filename;
        public string Filepath;
        public string Checksum;

        public string GetFullName()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), Filepath, Filename);
        }

        public bool Equals(ManifestEntry other)
        {
            return Filename == other.Filename && Filepath == other.Filepath && Checksum == other.Checksum;
        }

        public override bool Equals(object? obj)
        {
            return obj is ManifestEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filename, Filepath, Checksum);
        }
    }
    public class Manifest
    {
        public string CUSTOM_FOLDER_PATH = @"Multiplayer\store\HistoriaKolonii";
        public List<ManifestEntry> ManifestEntries = null!;
    }
}