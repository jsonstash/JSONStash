using System.ComponentModel.DataAnnotations.Schema;

namespace JSONStash.Common.Models
{
    public class Collection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CollectionId { get; set; }

        public int UserId { get; set; }

        public DateTimeOffset Created { get; set; }

        public Guid CollectionGuid { get; set; }


        public virtual User User { get; set; }

        public virtual ICollection<Stash> Stashes{ get; set; }

    }
}
