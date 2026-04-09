using Microsoft.AspNetCore.Mvc;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/confirmare")]
    public class ControllerConfirmare : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerConfirmare(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> Confirma(string token)
        {
            var membru = await _db.GetMembruByTokenAsync(token);
            if (membru == null)
                return Content(GetErrorHtml(), "text/html");

            await _db.ConfirmEmailAsync(membru.Id);

            return Content(GetSuccessHtml(), "text/html");
        }

        private string GetSuccessHtml()
        {
            return @"
        <!DOCTYPE html>
        <html lang='ro'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Email Confirmat</title>
            <style>
                body {
                    background-color: #ffffff;
                    font-family: 'Segoe UI', sans-serif;
                    margin: 0;
                    padding: 0;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    justify-content: center;
                    height: 100vh;
                    color: #222;
                    text-align: center;
                }

                h1 {
                    font-size: 28px;
                    font-weight: 700;
                    margin-bottom: 10px;
                }

                p {
                    font-size: 18px;
                    opacity: 0.8;
                    max-width: 380px;
                }
            </style>
        </head>
        <body>
            <h1>Email confirmat!</h1>
            <p>Contul tău a fost activat cu succes. Poți închide această pagină.</p>
        </body>
        </html>";
        }

        private string GetErrorHtml()
        {
            return @"
        <!DOCTYPE html>
        <html lang='ro'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Eroare Confirmare</title>
            <style>
                body {
                    background-color: #ffffff;
                    font-family: 'Segoe UI', sans-serif;
                    margin: 0;
                    padding: 0;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    justify-content: center;
                    height: 100vh;
                    color: #222;
                    text-align: center;
                }

                h1 {
                    font-size: 28px;
                    font-weight: 700;
                    margin-bottom: 10px;
                    color: #b00020;
                }

                p {
                    font-size: 18px;
                    opacity: 0.8;
                    max-width: 380px;
                }
            </style>
        </head>
        <body>
            <h1>Token invalid</h1>
            <p>Linkul de confirmare nu este valid sau a expirat.</p>
        </body>
        </html>";
        }
    }
}
