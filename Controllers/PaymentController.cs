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
    //Handles user payment-related operations, including balance top-ups and transaction retrieval.

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly FirstBrickContext _context;

        public PaymentController(FirstBrickContext context)
        {
            _context = context;
        }

        // Apple Pay Top-Up
        [HttpPost("ApplepayTopup")]
        public async Task<IActionResult> ApplePayTopUp([FromBody] TopUpRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest(new { message = "Invalid top-up amount." });

            var userId = GetUserIdFromClaims();

            // Get user's balance record or create a new one if it doesn't exist.

            var userBalance = await _context.UserBalances.FirstOrDefaultAsync(ub => ub.UserId == userId);

            if (userBalance == null)
            {
                userBalance = new UserBalance
                {
                    UserId = userId,
                    Balance = request.Amount
                };
                _context.UserBalances.Add(userBalance);
            }
            else
            {
                userBalance.Balance += request.Amount;
                _context.UserBalances.Update(userBalance);
            }


            //recording the transaction
            var transaction = new Transaction
            {
                UserId = userId,
                Type = "top-up",
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Top-up successful.", newBalance = userBalance.Balance });
        }

        // retrive User Balance currently
        [HttpGet("balance")]
        public async Task<IActionResult> GetUserBalance()
        {
            var userId = GetUserIdFromClaims();
            var userBalance = await _context.UserBalances.FirstOrDefaultAsync(ub => ub.UserId == userId);

            if (userBalance == null)
                return NotFound(new { message = "User balance not found." });

            return Ok(new { balance = userBalance.Balance });
        }

        // Get User Transactions
        [HttpGet("transactions")]
        public async Task<IActionResult> GetUserTransactions()
        {
            var userId = GetUserIdFromClaims();
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            if (!transactions.Any())
                return NotFound(new { message = "No transactions found." });

            return Ok(transactions);
        }

        // Internal Helper: Get User ID from JWT Claims
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

    // Request DTO for Top-Up // the structure of payload
    public class TopUpRequest
    {
        public decimal Amount { get; set; }
    }
}
