using System.ComponentModel.DataAnnotations.Schema;

namespace JSONStash.Common.Models
{
    public class Stash
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StashId { get; set; }

        public int? CollectionId { get; set; } = null;

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid StashGuid { get; set; }


        public virtual Collection Collection { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}
