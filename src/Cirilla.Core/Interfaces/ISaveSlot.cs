namespace Cirilla.Core.Interfaces
{
    public interface ISaveSlot
    {
        public string HunterName { get; set; }
        public string PalicoName { get; set; }
        public int HunterRank { get; set; }
        public int Zenny { get; set; }
        public int ResearchPoints { get; set; }
        public int HunterXp { get; set; }
        public int PlayTime { get; set; }
    }
}
