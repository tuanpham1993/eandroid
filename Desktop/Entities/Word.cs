namespace EApp.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Word")]
    public partial class Word
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string EN { get; set; }

        [Column(TypeName = "ntext")]
        public string VI { get; set; }

        public int Order { get; set; }

        public bool IsForgeted { get; set; }
    }
}
