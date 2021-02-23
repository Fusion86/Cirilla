namespace Cirilla.Core.Interfaces
{
    public interface ISaveData
    {
        public long SteamId { get; set; }

        public void Save(string path, bool encrypt = true, bool fixChecksum = true);
    }
}
