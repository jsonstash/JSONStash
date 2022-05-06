namespace JSONStash.Common.Models
{
    public class StashData
    {
        public string Name { get; set; }

        public string Data { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashId { get; set; }

        public Guid CollectionId { get; set; }

        public StashData(Stash stash)
        {
            Name = stash.Name;
            Data = stash.Data;
            Created = stash.Created;
            Modified = stash.Modified;
            StashId = stash.StashGuid;
            CollectionId = stash.Collection.CollectionGuid;
        }
    }
}
