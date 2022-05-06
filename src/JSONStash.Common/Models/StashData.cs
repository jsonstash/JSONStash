namespace JSONStash.Common.Models
{
    public class StashData
    {
        public string Name { get; set; }

        public string Data { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashGuid { get; set; }

        public Guid CollectionGuid { get; set; }

        public StashData(Stash stash)
        {
            Name = stash.Name;
            Data = stash.Data;
            Created = stash.Created;
            Modified = stash.Modified;
            StashGuid = stash.StashGuid;
            CollectionGuid = stash.Collection.CollectionGuid;
        }
    }
}
