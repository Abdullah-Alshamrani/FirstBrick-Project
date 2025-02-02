/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using FirstBrickAPI.Data;
using FirstBrickAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstBrickAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Handles project creation and investment-related operations.
    public class InvestmentController : ControllerBase
    {
        private readonly FirstBrickContext _context;

        public InvestmentController(FirstBrickContext context)
        {
            _context = context;
        }

        //  Create a New Real Estate Project.
        [HttpPost("project")]
        [Authorize] // Only authorized users can create a project.
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            var userId = GetUserIdFromClaims();
            project.OwnerId = userId;
            project.CreatedAt = DateTime.UtcNow;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllProjects), new { id = project.ProjectId }, project);
        }

        // Get All the Projects.
        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Investments)
                .ToListAsync();
            return Ok(projects);
        }

        // Invest in a Project
        [HttpPost("invest")]
        [Authorize] // Only authorized users can invest
        public async Task<IActionResult> InvestInProject([FromBody] InvestmentRequest request)
        {
            var userId = GetUserIdFromClaims();

            var userBalance = await _context.UserBalances.FirstOrDefaultAsync(ub => ub.UserId == userId);
            if (userBalance == null || userBalance.Balance < request.Amount)
                return BadRequest(new { message = "Insufficient balance. Please top-up before investing." });

            var project = await _context.Projects.FindAsync(request.ProjectId);
           // var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            var investment = new Investment
            {
                UserId = userId,
                ProjectId = request.ProjectId,
                Amount = request.Amount,
                InvestedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Investments.Add(investment);
            userBalance.Balance -= request.Amount;
            _context.UserBalances.Update(userBalance);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Investment successful!", investment });
        }

        // Helper Function to Get User ID
        private int GetUserIdFromClaims()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
                throw new UnauthorizedAccessException("No identity found in the current user context.");

            var userIdClaim = identity.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("The 'id' claim is missing in the token.");

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("The 'id' claim is invalid.");

            return userId;
        }
    }

    // Request DTO for Investment || Structure of the payload
    public class InvestmentRequest
    {
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
    }
}
