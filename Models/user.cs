using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace FirstBrickAPI.Models
{
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public string Role { get; set; } = "User"; // Default role is "User"

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // navigation
        // Preventing circular references
        [JsonIgnore]
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        [JsonIgnore] 
        public ICollection<Investment> Investments { get; set; } = new List<Investment>();

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        [JsonIgnore] 
        public UserBalance? UserBalance { get; set; }
    }
}
