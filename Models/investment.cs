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
    public class Investment
    {
        [Key]
        [Column("investment_id")]
        public int InvestmentId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("project_id")]
        public int ProjectId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("invested_at")]
        public DateTime InvestedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Project? Project { get; set; }
    }
}
