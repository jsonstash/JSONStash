namespace JSONStash.Common.Models
{
    public class StashQuota
    {
        public long MaxBytes { get; set; }

        public long UsedBytes { get; set; }

        public StashQuota(long max, long used)
        {
            MaxBytes = max;
            UsedBytes = used;
        }
    }
}
