using EmailService.MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using static System.Console;


try
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", true, true);
    
    var config = builder.Build();
    var emailHost = config["Host"];
    var port = config["Port"];
    var username = config["Username"];
    var password = config["Password"];
    var from = config["From"];

    EmailConfiguration mailConfig = new()
    {
        SmtpServer = emailHost,
        Port = Convert.ToInt32(port),
        UserName = username,
        Password = password,
        From = from
    };

    MimeMessage mimeMessage = new();
    mimeMessage.From.Add(new MailboxAddress(username, from));
    mimeMessage.Importance = MessageImportance.High;
    mimeMessage.Priority = MessagePriority.Urgent;

    List <MailboxAddress> toList = new() {new MailboxAddress("Mohamed Fazrin", "mfmfazrin1986@gmail.com")};
    mimeMessage.To.AddRange(toList);
    mimeMessage.Subject = "Testing Mail Subject";

    var msBuilder = new BodyBuilder
    {
        HtmlBody = $"<h2 style='color:red;'> Testing Mail Body </h2>"
    };

    mimeMessage.Body = msBuilder.ToMessageBody();


    SendMailAsyncClass mail = new();
    await mail.SendAsync(mimeMessage, mailConfig);
    WriteLine("Mail sent successfully");

    ReadLine();
}
catch (Exception exception)
{
    WriteLine(exception.Message);
}



public class SendMailAsyncClass
{
    public async Task SendAsync(MimeMessage message, EmailConfiguration smtp)
    {
        SmtpClient client = new();
        await client.ConnectAsync(smtp.SmtpServer, smtp.Port, false);
        // Note: since we don't have an OAuth2 token,
        // disable the XOAUTH2 authentication mechanism to speed up the process.
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        await client.AuthenticateAsync(smtp.From, smtp.Password);
        await client.SendAsync(message);
    }
}