using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportManagement.API.DTOs;
using TransportManagement.API.Services;
using TransportManagement.Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly TransportDbContext _db;

    public AuthController ( IAuthService authService, IEmailService emailService, TransportDbContext db )
    {
        _authService = authService;
        _emailService = emailService;
        _db = db;
    }

    /// <summary>
    /// Registers a new user (carrier, client, or admin) with the provided information.
    /// </summary>
    /// <param name="model">Registration details (name, email, password, etc.).</param>
    /// <returns>Returns the created user data or an error message if registration fails.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register ( [FromBody] RegisterDto model )
    {
        var result = await _authService.RegisterAsync(model);
        if (!result.Success)
            return BadRequest(new { message = result.Error });

        // لو عايز ترجع الـ Token في الـ Response (اختياري)
        //return Ok(new { result.Data, emailConfirmationToken = result.Data?.EmailConfirmationToken });

        return Ok(result.Data);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token if login is successful.
    /// </summary>
    /// <param name="model">Login credentials (email and password).</param>
    /// <returns>Returns a JWT token and user info if successful, or an error message if login fails.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login ( [FromBody] LoginDto model )
    {
        var result = await _authService.LoginAsync(model);
        if (!result.Success)
            return Unauthorized(new { message = result.Error });

        return Ok(result.Data);
    }


    /// <summary>
    /// Verifies the login code sent to the user's email.
    /// </summary>
    /// <param name="model">Email and code.</param>
    /// <returns>Returns success if the code is valid, or an error message if not.</returns>
    [HttpPost("verify-login-code")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyLoginCode ( [FromBody] VerifyLoginCodeDto model )
    {
        // ابحث عن المستخدم بالكود فقط
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.EmailLoginCode == model.Code && u.EmailLoginCodeExpiry > DateTime.UtcNow);

        if (user == null)
            return BadRequest(new { message = "الكود غير صحيح أو انتهت صلاحيته." });

        user.EmailLoginCode = null;
        user.EmailLoginCodeExpiry = null;
        await _db.SaveChangesAsync();

        // ممكن ترجع JWT Token هنا لو محتاج
        // var token = _jwtTokenHelper.GenerateToken(user.Id.ToString(), user.UserName, user.Email);
        // return Ok(new { token });

        return Ok(new { message = "تم تأكيد الدخول بنجاح." });
    }


    /// <summary>
    /// Sends a password reset email to the user if the email exists.
    /// </summary>
    /// <param name="model">User's email address.</param>
    /// <returns>Returns a success message if the email was sent, or an error message if it fails.</returns>
    [HttpPost("forget-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword ( [FromBody] ForgetPasswordDto model )
    {
        var result = await _authService.ForgetPasswordAsync(model);
        if (!result.Success)
            return BadRequest(new { message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>
    /// Resets the user's password using the provided reset token and new password.
    /// </summary>
    /// <param name="model">Reset token, new password, and user email.</param>
    /// <returns>Returns a success message if the password was reset, or an error message if it fails.</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword ( [FromBody] ResetPasswordDto model )
    {
        var result = await _authService.ResetPasswordAsync(model);
        if (!result.Success)
            return BadRequest(new { message = result.Error });
        return Ok(result.Data);
    }


    /// <summary>
    /// Confirms the user's email address using the provided user ID and confirmation token.
    /// </summary>
    /// <param name="userId">The user's unique ID.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>Returns a success message if the email is confirmed, or an error message if it fails.</returns>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail ( string token )
    {
        var result = await _authService.ConfirmEmailAsync(token);
        if (!result.Success)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "Email confirmed successfully." });
    }
}
