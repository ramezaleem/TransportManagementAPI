using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TransportManagement.API.DTOs;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly TransportDbContext _context;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IEmailService _emailService;
        private readonly IWhatsappService _whatsappService;
        public AuthService ( TransportDbContext context, JwtTokenHelper jwtTokenHelper, IEmailService emailService, IWhatsappService whatsappService )
        {
            _context = context;
            _jwtTokenHelper = jwtTokenHelper;
            _emailService = emailService;
            _whatsappService = whatsappService;
        }

        private string HashPassword ( string password )
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private string GenerateOtp ()
        {
            var rand = new Random();
            return rand.Next(100000, 999999).ToString(); // 6 أرقام
        }

        public async Task<(bool Success, string Error, object Data)> RegisterAsync ( RegisterDto model )
        {
            // تحقق من أن الإيميل بصيغة صحيحة وينتهي بـ @gmail.com
            if (string.IsNullOrWhiteSpace(model.Email) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@gmail\.com$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return (false, "Email must be a Gmail address (e.g. example@gmail.com).", null);
            }
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return (false, "Email already exists.", null);

            // تحقق من أن الاسم الأول حروف فقط
            if (string.IsNullOrWhiteSpace(model.FirstName) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.FirstName, @"^[A-Za-z]+$"))
            {
                return (false, "First name must contain letters only (no numbers).", null);
            }

            // تحقق من أن الاسم الأخير حروف فقط
            if (string.IsNullOrWhiteSpace(model.LastName) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.LastName, @"^[A-Za-z]+$"))
            {
                return (false, "Last name must contain letters only (no numbers).", null);
            }

            // تحقق من رقم الجوال السعودي وصيغته
            if (string.IsNullOrWhiteSpace(model.MobileNumber) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.MobileNumber, @"^\+9665[0-9]{8}$"))
            {
                return (false, "Mobile number must be Saudi format (e.g. +9665XXXXXXXX).", null);
            }
            if (await _context.Users.AnyAsync(u => u.MobileNumber == model.MobileNumber))
                return (false, "Mobile number already exists.", null);

            // تحقق من أن الـ NationalId Unique
            if (await _context.Users.AnyAsync(u => u.NationalId == model.NationalId))
                return (false, "National ID already exists.", null);

            // تحقق من أن الـ TransporterType حروف فقط
            if (string.IsNullOrWhiteSpace(model.TransporterType) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.TransporterType, @"^[A-Za-z]+$"))
            {
                return (false, "TransporterType must contain letters only.", null);
            }

            // تحقق من الباسوورد
            if (model.Password.Length < 8 || !model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsLetter))
                return (false, "Password must be at least 8 characters and contain both letters and numbers.", null);

            var otp = GenerateOtp();
            var emailToken = Guid.NewGuid().ToString();

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                MobileNumber = model.MobileNumber,
                NationalId = model.NationalId,
                TransporterType = model.TransporterType,
                IsEmailConfirmed = false,
                IsMobileConfirmed = false,
                EmailConfirmationToken = emailToken,
                EmailLoginCode = otp,
                EmailLoginCodeExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();

                var confirmLink = $"https://localhost:7299/api/Auth/confirm-email?token={emailToken}";
                var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; background:#fff; padding:30px; border-radius:10px; max-width:600px; margin:auto; box-shadow:0 2px 8px #eee;'>
                    <h2 style='color:#007bff;'>مرحبًا {user.FirstName},</h2>
                    <p>شكرا لتسجيلك! لتفعيل حسابك اضغط على الزر أدناه:</p>
                    <a href='{confirmLink}' style='background:#007bff; color:#fff; padding:10px 20px; border-radius:6px; display:inline-block; text-decoration:none;'>تأكيد البريد الإلكتروني</a>
                    <p>إذا لم تطلب ذلك، تجاهل هذه الرسالة.</p>
                </div>";
                _emailService.SendEmail(user.Email, "تأكيد البريد الإلكتروني", htmlBody, true);

                return (true, null, new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.MobileNumber,
                    EmailConfirmationToken = emailToken
                });
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }


        public async Task<(bool Success, string Error, object Data)> LoginAsync ( LoginDto model )
        {
            // تحقق من أن الإيميل موجود وصحيح وينتهي بـ @gmail.com
            if (string.IsNullOrWhiteSpace(model.Email) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@gmail\.com$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return (false, "البريد الإلكتروني يجب أن يكون Gmail فقط (مثال: example@gmail.com)", null);
            }

            // البحث عن المستخدم بالإيميل فقط
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return (false, "البريد الإلكتروني غير صحيح أو غير موجود.", null);

            var passwordHash = HashPassword(model.Password);
            if (user.PasswordHash != passwordHash)
                return (false, "كلمة المرور غير صحيحة.", null);

            // توليد JWT Token بعد نجاح تسجيل الدخول
            var token = _jwtTokenHelper.GenerateToken(user.Id.ToString(), $"{user.FirstName} {user.LastName}", user.Email);

            // توليد كود جديد وإرساله على الإيميل (اختياري)
            var code = GenerateOtp();
            user.EmailLoginCode = code;
            user.EmailLoginCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // رسالة HTML بشكل جميل
            var htmlBody = $@"
            <div style='font-family:Tahoma,Arial,sans-serif;background:#f7f7f7;padding:30px;'>
                <div style='background:#fff;padding:30px 40px;border-radius:10px;max-width:500px;margin:auto;box-shadow:0 2px 8px #eee;'>
                    <h2 style='color:#007bff;margin-bottom:20px;'>مرحبًا {user.FirstName} {user.LastName} 👋</h2>
                    <p style='font-size:17px;color:#333;'>كود الدخول الخاص بك:</p>
                    <div style='font-size:32px;letter-spacing:8px;font-weight:bold;background:#f1f1f1;padding:15px 0;border-radius:6px;color:#222;text-align:center;margin-bottom:20px;'>{code}</div>
                    <p style='font-size:15px;color:#888;'>هذا الكود صالح لمدة 10 دقائق فقط.</p>
                    <hr style='margin:30px 0 15px 0;border:none;border-top:1px solid #eee;'/>
                    <p style='font-size:13px;color:#aaa;text-align:center;'>إذا لم تطلب هذا الكود تجاهل هذه الرسالة.</p>
                </div>
            </div>";
            _emailService.SendEmail(user.Email, "كود الدخول", htmlBody, true);

            return (true, null, new
            {
                message = "تم تسجيل الدخول بنجاح. تم إرسال كود الدخول على الإيميل.",
                token,
                user = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email
                }
            });
        }

        // تأكيد كود الدخول (يدخل الكود فقط)
        public async Task<(bool Success, string Error, object Data)> VerifyLoginCodeAsync ( string code )
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.EmailLoginCode == code && u.EmailLoginCodeExpiry > DateTime.UtcNow);

            if (user == null)
                return (false, "الكود غير صحيح أو انتهت صلاحيته.", null);

            user.EmailLoginCode = null;
            user.EmailLoginCodeExpiry = null;
            await _context.SaveChangesAsync();

            var token = _jwtTokenHelper.GenerateToken(
                user.Id.ToString(),
                $"{user.FirstName} {user.LastName}",
                user.Email
            );

            return (true, null, new { token });
        }

        public async Task<(bool Success, string Error, object Data)> ForgetPasswordAsync ( ForgetPasswordDto model )
        {
            // تحقق من أن الإيميل ينتهي بـ @gmail.com
            if (string.IsNullOrWhiteSpace(model.Email) ||
                !System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@gmail\.com$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return (false, "البريد الإلكتروني يجب أن يكون Gmail فقط (مثال: example@gmail.com)", null);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
                return (false, "البريد الإلكتروني غير موجود في النظام.", null);

            user.ResetPasswordToken = Guid.NewGuid().ToString();
            user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var resetLink = $"https://yourfrontend.com/reset-password?email={user.Email}&token={user.ResetPasswordToken}";
            var htmlBody = $@"
            <div style='font-family:Arial;background:#f7f7f7;padding:20px;'>
                <div style='background:#fff;padding:30px;border-radius:8px;max-width:500px;margin:auto;'>
                    <h2>إعادة تعيين كلمة المرور</h2>
                    <p>مرحبًا {user.FirstName}،</p>
                    <p>اضغط على الزر التالي لإعادة تعيين كلمة المرور:</p>
                    <a style='background:#007bff;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;' href='{resetLink}'>إعادة تعيين كلمة المرور</a>
                    <p>إذا لم تطلب ذلك، تجاهل هذه الرسالة.</p>
                </div>
            </div>";
            _emailService.SendEmail(user.Email, "إعادة تعيين كلمة المرور", htmlBody, true);

            // إرجاع التوكن في الـ Response (للاختبار أو للواجهة)
            return (true, null, new
            {
                message = "تم إرسال رابط إعادة تعيين كلمة المرور على الإيميل.",
                resetPasswordToken = user.ResetPasswordToken
            });
        }

        public async Task<(bool Success, string Error, object Data)> ResetPasswordAsync ( ResetPasswordDto model )
        {
            // تحقق من المعايير
            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 8 ||
                !model.NewPassword.Any(char.IsDigit) || !model.NewPassword.Any(char.IsLetter))
                return (false, "Password must be at least 8 characters and contain both letters and numbers.", null);

            if (model.NewPassword != model.ConfirmPassword)
                return (false, "Passwords do not match.", null);

            // ابحث عن المستخدم بالـ Token فقط وتأكد من صلاحية التوكن
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == model.Token && u.ResetPasswordExpiry > DateTime.UtcNow);

            if (user == null)
                return (false, "Invalid or expired token.", null);

            user.PasswordHash = HashPassword(model.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;
            await _context.SaveChangesAsync();

            return (true, null, new { message = "Password has been reset." });
        }


        public async Task<(bool Success, string Error)> ConfirmEmailAsync ( string code )
        {
            // ابحث عن المستخدم بالكود فقط (token)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == code);
            if (user == null)
                return (false, "Invalid or expired confirmation code.");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            await _context.SaveChangesAsync();

            // إرسال رسالة ترحيب بعد التفعيل (اختياري)
            var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: white; border-radius: 10px; padding: 30px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                    <h2 style='color: #2c3e50;'>مرحبًا {user.FirstName}،</h2>
                    <p style='font-size: 16px; color: #34495e;'>تم تفعيل بريدك الإلكتروني بنجاح. شكرًا لانضمامك إلينا!</p>
                </div>
            </div>";
            _emailService.SendEmail(user.Email, "تأكيد بريدك الإلكتروني", htmlBody, true);

            return (true, null);
        }


    }
}
