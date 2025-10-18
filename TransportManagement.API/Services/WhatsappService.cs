namespace TransportManagement.API.Services
{
    public class WhatsappService : IWhatsappService
    {
        // لو عندك Config أو API Key بتاع Twilio أو WhatsApp Cloud API ضيف هنا

        public async Task SendOtp ( string mobileNumber, string otp )
        {
            // هنا بتحط الاستدعاء الفعلي للـ API للواتساب
            // لو بتستخدم Twilio WhatsApp API مثلًا:
            /*
            var client = new TwilioRestClient(accountSid, authToken);
            await MessageResource.CreateAsync(
                body: $"Your verification code is: {otp}",
                from: new PhoneNumber("whatsapp:+14155238886"),
                to: new PhoneNumber("whatsapp:" + mobileNumber),
                client: client);
            */

            // أثناء التطوير (للتجربة فقط)
            await Task.Run(() =>
            {
                Console.WriteLine($"[WhatsApp-Mock] Send OTP {otp} to {mobileNumber}");
            });
        }
    }
}
