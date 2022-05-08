namespace JSONStash.Common.Models
{
    public class StashDetail
    {
        public string Name { get; set; }

        public Guid Key { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashId { get; set; }

        public Guid CollectionId { get; set; }

        public StashDetail(Stash stash)
        {
            Name = stash.Name;
            Created = stash.Created;
            Modified = stash.Modified;
            Key = stash.Key;
            StashId = stash.StashGuid;
            CollectionId = stash.Collection.CollectionGuid;
        }
    }
}
