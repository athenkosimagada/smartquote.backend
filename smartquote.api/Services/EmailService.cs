using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using smartquote.api.Options;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.SmtpUser));
        mimeMessage.To.Add(MailboxAddress.Parse(to));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _emailOptions.SmtpHost, 
            _emailOptions.SmtpPort, 
            MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_emailOptions.SmtpUser, _emailOptions.SmtpPass);
        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);
    }
}
