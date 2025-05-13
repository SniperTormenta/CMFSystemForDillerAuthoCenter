using CMFSystemForDillerAuthoCenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CMFSystemForDillerAuthoCenter.Services
{
    public class EmailService
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _imapHost = "imap.gmail.com";
        private readonly int _imapPort = 993;
        private readonly string _username = "Galochkin666@gmail.com"; // Замени на свой email
        private readonly string _password = "ycmc diqr jhbj xvso"; // Пароль приложения
        private readonly string _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails.json");
        private EmailStorage _storage;

        public EmailService()
        {
            _storage = LoadEmails();
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender", _username));
            foreach (var recipient in emailMessage.Recipients)
            {
                message.To.Add(new MailboxAddress(recipient, recipient));
            }
            message.Subject = emailMessage.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = emailMessage.Body };
            if (emailMessage.Attachments != null)
            {
                foreach (var attachment in emailMessage.Attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient()){
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_username, _password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            

            emailMessage.IsSent = true;
            emailMessage.Date = DateTime.Now;
            _storage.SentEmails.Add(emailMessage);
            _storage.OpenStatistics[emailMessage.Id] = false;
            SaveEmails();
        }

        public async Task<List<EmailMessage>> FetchEmailsAsync()
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync(_imapHost, _imapPort, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_username, _password);
                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadWrite);

                var messages = new List<EmailMessage>();
                var existingIds = _storage.ReceivedEmails.Select(e => e.Id).ToHashSet();

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = await inbox.GetMessageAsync(i);
                    var messageId = message.MessageId ?? Guid.NewGuid().ToString();
                    if (existingIds.Contains(messageId))
                        continue;

                    var emailMessage = new EmailMessage
                    {
                        Id = messageId,
                        Sender = message.From.FirstOrDefault()?.ToString() ?? "Unknown",
                        Recipients = new List<string> { _username },
                        Subject = message.Subject,
                        Body = message.HtmlBody ?? message.TextBody,
                        Date = message.Date.DateTime,
                        Attachments = new List<string>(),
                        IsRead = false,
                        IsSent = false
                    };

                    foreach (var attachment in message.Attachments.OfType<MimePart>())
                    {
                        var fileName = attachment.FileName;
                        var attachmentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(attachmentPath));
                        using (var stream = File.Create(attachmentPath))
                        {
                            await attachment.Content.DecodeToAsync(stream);
                        }
                        emailMessage.Attachments.Add(attachmentPath);
                    }

                    messages.Add(emailMessage);
                    _storage.ReceivedEmails.Add(emailMessage);
                    _storage.OpenStatistics[emailMessage.Id] = false;
                }

                await inbox.CloseAsync();
                await client.DisconnectAsync(true);
                SaveEmails();

                return messages;
            }
        }
           

        public void MarkAsRead(string emailId)
        {
            var email = _storage.ReceivedEmails.FirstOrDefault(e => e.Id == emailId) ??
                        _storage.SentEmails.FirstOrDefault(e => e.Id == emailId);
            if (email != null)
            {
                email.IsRead = true;
                _storage.OpenStatistics[emailId] = true;
                SaveEmails();
            }
        }

        public EmailStorage GetStorage()
        {
            return _storage;
        }

        private EmailStorage LoadEmails()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    string json = File.ReadAllText(_storagePath);
                    return JsonConvert.DeserializeObject<EmailStorage>(json) ?? new EmailStorage();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке писем: {ex.Message}");
            }
            return new EmailStorage();
        }

        private void SaveEmails()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_storage, Formatting.Indented);
                File.WriteAllText(_storagePath, json);
                System.Diagnostics.Debug.WriteLine($"EmailService SaveEmails: Файл {_storagePath} сохранён. Содержимое:\n{json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении писем: {ex.Message}");
            }
        }
    }
}