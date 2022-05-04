namespace JSONStash.Common.Models
{
    public class StashRecord
    {
        public string Record { get; set; }

        public Metadata Metadata { get; set; }

        public StashRecord(Stash stash, string record)
        {
            Record = record;

            Metadata = new Metadata(stash);
        }
    }


}
