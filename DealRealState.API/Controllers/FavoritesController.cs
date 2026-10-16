using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DTOs;
using DealRealState.Data.DataAccess;
using BusinessLayer.Services;

[Route("api/favorites")]
[ApiController]
[Authorize] // تأكد من وضع هذه الخاصية لضمان أن المستخدم مسجل الدخول
public class FavoritesController : ControllerBase
{
    [Authorize] // Enforces token authorization
    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequestDto request)
    {
        if (request == null || request.PropertyID <= 0)
        {
            return BadRequest(new { message = "Invalid request data." });
        }

        try
        {
            // 1. Extract UserID from token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst("UserId")
                           ?? User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserID))
            {
                return Unauthorized(new { message = "Invalid or missing authentication token." });
            }

            // 2. Call the Business Layer using async/await
            bool isFavorite = await FavoriteService.ToggleFavoriteAsync(currentUserID, request.PropertyID, request.Mode);

            // 3. Return updated favorite state for UI synchronization
            return Ok(new
            {
                success = true,
                isFavorite = isFavorite,
                message = "Favorite status updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("my-favorites")]
    [Authorize]
    public async Task<IActionResult> GetUserFavorites()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized("User ID not found in token claims.");
        }

        if (!int.TryParse(userIdString, out int userId))
        {
            return BadRequest("Invalid User ID format.");
        }

        var favorites = await BusinessLayer.Services.FavoriteService.GetUserFavoritesAsync(userId);

        return Ok(favorites);
    }


}