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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FirstBrickAPI.Controllers
{ //user investment portfolio opreations
    [Route("v1/portfolio")]
    [ApiController]
    [Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly FirstBrickContext _context;

        public PortfolioController(FirstBrickContext context) // injrct DBs context.
        {
            _context = context;
        }

        // v1/portfolio - Get the authenticated user's entire investment portfolio
        [HttpGet]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var userId = GetUserIdFromClaims();
            Console.WriteLine($"Extracted User ID from Claims: {userId}");

            // For debugging
            var query = _context.Portfolios
                .Where(p => p.UserId == userId)
                .Include(p => p.Project);
            Console.WriteLine($"Generated SQL: {query.ToQueryString()}");

            var portfolio = await query.ToListAsync();

            if (!portfolio.Any())
                return NotFound(new { message = "No investments found in the portfolio." });

            return Ok(portfolio);
        }

        // v1/portfolio/{projectId} - Get portfolio details of a specific project for the user.
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetPortfolioByProject(int projectId)
        {
            var userId = GetUserIdFromClaims();
            Console.WriteLine($"Extracted User ID from Claims: {userId}, Requested Project ID: {projectId}");

            // For debugging.
            var query = _context.Portfolios
                .Where(p => p.UserId == userId && p.ProjectId == projectId)
                .Include(p => p.Project);
            Console.WriteLine($"Generated SQL: {query.ToQueryString()}");

            var portfolioItem = await query.FirstOrDefaultAsync();

            if (portfolioItem == null)
                return NotFound(new { message = $"No portfolio details found for project ID {projectId}." });

            return Ok(portfolioItem);
        }

        // Helper method to extract the User ID from claims
        private int GetUserIdFromClaims()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var userIdClaim = identity.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID claim.");

            return userId;
        }
    }
}
