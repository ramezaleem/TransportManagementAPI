using TransportManagement.API.DTOs;

namespace TransportManagement.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Error, object Data)> RegisterAsync ( RegisterDto model );
        Task<(bool Success, string Error, object Data)> LoginAsync ( LoginDto model );
        Task<(bool Success, string Error, object Data)> ForgetPasswordAsync ( ForgetPasswordDto model );
        Task<(bool Success, string Error, object Data)> ResetPasswordAsync ( ResetPasswordDto model );
        Task<(bool Success, string Error)> ConfirmEmailAsync ( string token );

        // إرسال OTP على رقم الموبايل عبر WhatsApp
        //  Task<(bool Success, string Error)> SendMobileOtpAsync ( string mobileNumber );

        // تحقق/تأكيد OTP على رقم الموبايل
        //   Task<(bool Success, string Error)> VerifyMobileOtpAsync ( string mobileNumber, string otp );
    }
}
