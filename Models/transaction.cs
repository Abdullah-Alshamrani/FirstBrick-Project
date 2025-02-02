/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstBrickAPI.Models
{
    public class Transaction
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //  Navigation
        public User? User { get; set; }
    }
}
