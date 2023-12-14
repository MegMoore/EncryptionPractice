using System.ComponentModel.DataAnnotations;

namespace EncryptionPractice.Models
{
    public class Kid
    {
        public int Id { get; set; }
        [StringLength(30)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(30)]
        public string LastName { get; set; } = string.Empty;
        [StringLength(80)]
        public string Email { get; set; } = string.Empty;
        [StringLength(80)]
        public string HashPassword { get; set; } = string.Empty;
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [StringLength(12)]
        public string Phone { get; set; } = string.Empty;
    }  
}
