using Aaditya.Services;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _fromEmail = "chococrunch76@gmail.com";
    private readonly string _password = "ipvj sitq etdu iglm"; // Store securely in appsettings.json!

    public async Task SendEmailAsync(string email, string subject, string otp)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("ChocoCrunch", _fromEmail));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;

        // Attractive Email Message
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #9E5B40; color: #F8E4C2; border-radius: 8px; text-align: center;'>
                <h2 style='color: #FFD700;'>Welcome to ChocoCrunch!🍫</h2>
                <p style='font-size: 18px;'>You're just one step away from resetting your password.</p>
                <h3 style='font-size: 24px; color: #fff; background: #D2691E; padding: 10px; display: inline-block; border-radius: 6px;'>{otp}</h3>
                <p>Please enter this OTP on the verification page to proceed.</p>
                <p>If you didn't request this, you can safely ignore this email.</p>
                <hr style='border: 1px solid #FFD700; margin: 20px 0;'/>
                <p style='font-size: 14px;'>Stay sweet !<br/><b>ChocoCrunch Team</b> 🍫</p>
            </div>";

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _port, false);
        await client.AuthenticateAsync(_fromEmail, _password);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
