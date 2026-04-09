using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace RoomiesApi.Services
{
    public class ServiciuEmail
    {
        private readonly string _email = "cristianmircea19@gmail.com";
        private readonly string _appPassword = "jynr tcno mhuo ftxf";

        public async Task TrimiteEmailConfirmareAsync(string destinatar, string token)
        {

            var baseUrl = "http://localhost:5137";
            var linkConfirmare = $"{baseUrl}/api/confirmare/{token}";

            var mesaj = new MimeMessage();
            mesaj.From.Add(new MailboxAddress("Roomies", _email));
            mesaj.To.Add(new MailboxAddress("", destinatar));
            mesaj.Subject = "Confirmă contul Roomies";

            mesaj.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Bun venit la Roomies!</h2>
                    <p>Apasă pe butonul de mai jos pentru a confirma contul tău:</p>
                    <a href='{linkConfirmare}'
                       style='background:#4DA3FF;color:white;padding:6px 18px;
                              border-radius:6px;text-decoration:none;'>
                        Confirmă contul
                    </a>
                    <p>Dacă nu ai creat un cont, ignoră acest email.</p>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_email, _appPassword);
            await smtp.SendAsync(mesaj);
            await smtp.DisconnectAsync(true);
        }
    }
}