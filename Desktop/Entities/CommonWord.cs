namespace EApp.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommonWord")]
    public partial class CommonWord
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string EN { get; set; }
    }
}
