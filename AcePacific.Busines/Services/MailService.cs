using AcePacific.Data.Entities;
using AcePacific.Data.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace AcePacific.Busines.Services
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
        bool SendHTMLMail(HTMLMailData htmlMailData, EmailTemplate emailTemplate);
        Task<bool> SendWelcomeEmailAsync(EmailTemplate newUser);
    }
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly HttpClient _httpClient;
        public MailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
            //_httpClient = httpClientFactory.CreateClient("MailTrapApiClient");
        }

        public bool SendMail(MailData mailData)
        {
            try
            {
                using MimeMessage emailMessage = new();
                MailboxAddress emailFrom = new(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", "cc@example.com"));
                emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

                emailMessage.Subject = mailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new();
                emailBodyBuilder.TextBody = mailData.EmailBody;

                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                using (SmtpClient mailClient = new SmtpClient())
                {
                    mailClient.Connect(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                    mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }

                return true;
            }
            catch (Exception)
            {
                // Exception Details
                return false;
            }
        }
        public bool SendHTMLMail(HTMLMailData htmlMailData, EmailTemplate emailTemplate)
        {
            try
            {
                using (MimeMessage emailMessage = new())
                {
                    MailboxAddress emailFrom = new(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);

                    MailboxAddress emailTo = new(htmlMailData.EmailToName, htmlMailData.EmailToId);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = "Hello";

                    string filePath = Directory.GetCurrentDirectory() + "..\\AcePacific.Common\\Templates\\welcome.html";
                    string emailTemplateText = File.ReadAllText(filePath)
                    .Replace("{UserName}", emailTemplate.UserName)
                    .Replace("{AccountNumber}", emailTemplate.AccountNumber)
                    .Replace("{FirstName}", emailTemplate.FirstName)
                    .Replace("{LastName}", emailTemplate.LastName);

                    emailTemplateText = string.Format(emailTemplateText, htmlMailData.EmailToName, DateTime.Today.Date.ToShortDateString());

                    BodyBuilder emailBodyBuilder = new BodyBuilder
                    {
                        HtmlBody = emailTemplateText,
                        TextBody = "Welcome"
                    };
                    //emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using SmtpClient mailClient = new();
                    mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate(_mailSettings.SenderEmail, _mailSettings.Password);
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Exception Details
                return false;
            }
        }
        public async Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\Hello.html";
            string emailTemplateText = File.ReadAllText(filePath);

            var htmlBody = string.Format(emailTemplateText, htmlMailData.EmailToName, DateTime.Today.Date.ToShortDateString());

            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = htmlMailData.EmailToId, Name = htmlMailData.EmailToName } },
                Subject = "Hello",
                Html = htmlBody
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = mailData.EmailToId, Name = mailData.EmailToName } },
                Subject = mailData.EmailSubject,
                Text = mailData.EmailBody
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment)
        {
            var attachments = new List<object>();
            if (mailDataWithAttachment.EmailAttachments != null)
            {
                foreach (var attachmentFile in mailDataWithAttachment.EmailAttachments)
                {
                    if (attachmentFile.Length == 0)
                    {
                        continue;
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await attachmentFile.CopyToAsync(memoryStream);
                        attachments.Add(new
                        {
                            FileName = attachmentFile.FileName,
                            Content = Convert.ToBase64String(memoryStream.ToArray()),
                            Type = attachmentFile.ContentType,
                            Disposition = "attachment" // or inline
                        });
                    }
                }
            }

            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = mailDataWithAttachment.EmailToId, Name = mailDataWithAttachment.EmailToName } },
                Subject = mailDataWithAttachment.EmailSubject,
                Text = mailDataWithAttachment.EmailBody,
                Attachments = attachments.ToArray()
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }
        public async Task<bool> SendWelcomeEmailAsync(EmailTemplate newUser)
        {
            try
            {
                using (var emailMessage = new MimeMessage())
                {
                    emailMessage.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
                    emailMessage.To.Add(new MailboxAddress($"{newUser.FirstName} {newUser.LastName}", newUser.Email));
                    emailMessage.Subject = "Welcome to Ace Pacific";

                    // Load HTML template from file
                    //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AcePacific.Common", "Templates", "welcome.html");
                    string filePath = Directory.GetCurrentDirectory();
                    string templatesDirectory = Path.Combine(filePath, "AcePacific.Common", "Templates", "welcome.html");


                    // C:\Users\Admin\Desktop\Startup\Ace Pacific\MainApp\AcePacific.Common\Templates\welcome.html
                    /*filePath.
                    , "AcePacific.Common", "Templates", "welcome.html");*/
                    string emailTemplateText = File.ReadAllText(filePath);

                    // Replace placeholders in the HTML template with actual data
                    emailTemplateText = emailTemplateText.Replace("{{User}}", $"{newUser.FirstName} {newUser.LastName}");
                    emailTemplateText = emailTemplateText.Replace("{{accountNumber}}", $"{newUser.AccountNumber}");
                    // Add other replacements as needed

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = emailTemplateText,
                        TextBody = "Welcome to Ace Pacific", // Add plain text content if needed  
                    };

                    emailMessage.Body = bodyBuilder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync("cindi4talk@gmail.com", "kxyzixWF16$$");
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                return false;
            }
        }


    }
}
