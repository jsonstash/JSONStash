﻿namespace JSONStash.Common.Models
{
    public class StashMetadata
    {
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashId { get; set; }

        public Guid CollectionId { get; set; }

        public StashQuota Quota { get; set; }

        public StashMetadata(Stash stash)
        {
            Name = stash.Name;
            Created = stash.Created;
            Modified = stash.Modified;
            StashId = stash.StashGuid;
            CollectionId = stash.Collection.CollectionGuid;
        }
    }
}
