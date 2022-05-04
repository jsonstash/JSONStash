using System.ComponentModel.DataAnnotations.Schema;

namespace JSONStash.Common.Models
{
    public class Record
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        public int StashId { get; set; }

        public int Version { get; set; }

        public string Value { get; set; }

        public DateTimeOffset Created { get; set; }

        public Guid RecordGuid { get; set; }


        public virtual Stash Stash{ get; set; }
    }
}
