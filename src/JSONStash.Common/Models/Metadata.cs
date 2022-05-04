namespace JSONStash.Common.Models
{
    public class Metadata
    {
        public string Name { get; set; }

        public Guid StashId { get; set; }

        public Guid CollectionId { get; set; }

        public int Versions { get; set; }

        public DateTimeOffset Created { get; set; }

        public Metadata(Stash stash)
        {
            CollectionId = stash.Collection.CollectionGuid;
            Name = stash.Name;
            StashId = stash.StashGuid;
            Created = stash.Created;
            Versions = stash.Records.Count;
        }
    }
}
