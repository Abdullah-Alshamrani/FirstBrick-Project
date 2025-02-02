/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // For JsonIgnore

namespace FirstBrickAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public int OwnerId { get; set; }

        // Add [JsonIgnore] to prevent circular reference issues
        [JsonIgnore]
        public User? Owner { get; set; }

        // Investments navigation 
        [JsonIgnore] // Prevent circular references
        public ICollection<Investment>? Investments { get; set; }
    }
}
