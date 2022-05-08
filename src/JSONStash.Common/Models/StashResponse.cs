namespace JSONStash.Common.Models
{
    public class StashResponse
    {
        public string Data { get; set; }

        public StashMetadata Metadata { get; set; }

        public StashResponse(Stash stash)
        {
            Data = stash.Data;
            Metadata = new(stash);
        }

        public void SetQuota(long max)
        {
            long used = Data.Length * sizeof(char);

            Metadata.Quota = new(max, used);
        }
    }
}
