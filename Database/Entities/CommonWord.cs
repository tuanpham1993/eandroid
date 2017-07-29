namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CommonWord")]
    public partial class CommonWord
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string EN { get; set; }
    }
}
