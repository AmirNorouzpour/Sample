using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; } = true;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public GenderType? GenderType { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + LastName;
    }

    public enum GenderType
    {
        Female = 0,
        Male = 1,
        None = 2
    }
}
