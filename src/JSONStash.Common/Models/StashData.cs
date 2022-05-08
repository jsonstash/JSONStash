namespace JSONStash.Common.Models
{
    public class StashData
    {
        public string Data { get; set; }

        public StashMetadata Metadata { get; set; }

        public StashData(Stash stash)
        {
            Data = stash.Data;
            Metadata = new(stash);
        }
    }
}
