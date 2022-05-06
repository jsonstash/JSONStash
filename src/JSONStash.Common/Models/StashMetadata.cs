namespace JSONStash.Common.Models
{
    public class StashMetadata
    {
        public string Name { get; set; }

        public Guid Key { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashGuid { get; set; }

        public Guid CollectionGuid { get; set; }

        public StashMetadata(Stash stash)
        {
            Name = stash.Name;
            Created = stash.Created;
            Modified = stash.Modified;
            Key = stash.Key;
            StashGuid = stash.StashGuid;
            CollectionGuid = stash.Collection.CollectionGuid;
        }
    }
}
