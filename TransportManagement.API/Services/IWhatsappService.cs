namespace TransportManagement.API.Services
{
    public interface IWhatsappService
    {
        Task SendOtp ( string mobileNumber, string otp );
    }
}
