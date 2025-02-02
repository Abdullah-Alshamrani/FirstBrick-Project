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
public class Portfolio
{
    [Key]
    [Column("portfolio_id")]
    public int PortfolioId { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    [Column("project_id")]
    public int ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public Project? Project { get; set; }

    [Required]
    [Column("amount")]
    public decimal Amount { get; set; }

    [Required]
    [Column("invested_at")]
    public DateTime InvestedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
}


