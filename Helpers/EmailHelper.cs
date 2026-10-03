using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Ecommerce_Website_MVC.Helpers
{
    public static class EmailHelper
    {
        private static string From     => ConfigurationManager.AppSettings["SmtpFrom"];
        private static string FromName => ConfigurationManager.AppSettings["SmtpFromName"];
        private static string Host     => ConfigurationManager.AppSettings["SmtpHost"];
        private static int    Port     => int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        private static string User     => ConfigurationManager.AppSettings["SmtpUser"];
        private static string Pass     => ConfigurationManager.AppSettings["SmtpPass"];
        private static bool   EnableSsl => bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
        public static void SendHtml(string toAddress, string subject, string htmlBody)
        {
            using (var mail = new MailMessage())
            {
                mail.From    = new MailAddress(From, FromName);
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body    = htmlBody;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(User, Pass);
                    smtp.EnableSsl   = EnableSsl;
                    smtp.Send(mail);
                }
            }
        }

        public static string BuildOtpEmail(string otp) => $@"
            <div style='font-family: sans-serif; padding: 25px; background-color: #141414; color: #ffffff; border-radius: 8px; max-width: 480px;'>
                <h2 style='color: #e50914; margin-top: 0;'>Show's Garage</h2>
                <p style='color: #aaaaaa; font-size: 14px;'>Welcome to the club! Use the verification security code below to activate your account profile logs:</p>
                <div style='background-color: #1c1c1c; border: 1px solid #2d2d2d; padding: 15px; text-align: center; font-size: 26px; font-weight: 800; color: #fff; letter-spacing: 5px; border-radius: 4px; margin: 20px 0;'>
                    {otp}
                </div>
                <p style='font-size: 11px; color: #666666; margin: 0;'>If you did not request this code, you can safely ignore this email validation trace.</p>
            </div>";

        public static string BuildPasswordResetEmail(string otp) => $@"
            <div style='background-color: #141414; padding: 30px; font-family: Arial, sans-serif; text-align: center; color: #ffffff; border-radius: 8px;'>
                <h2 style='color: #e50914; margin-bottom: 20px;'>Show's Garage</h2>
                <p style='font-size: 16px; color: #ccc;'>Use the verification code below to reset your password:</p>
                <div style='background-color: #222; display: inline-block; padding: 15px 35px; font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #fff; margin: 20px 0; border: 1px solid #444; border-radius: 4px;'>{otp}</div>
            </div>";

        public static string BuildProfileOtpEmail(string otp) => $@"
            <div style='background-color: #141414; padding: 30px; font-family: Arial, sans-serif; text-align: center; color: #ffffff; border-radius: 8px;'>
                <h2 style='color: #e50914; margin-bottom: 20px;'>Show's Garage</h2>
                <p style='font-size: 16px; color: #ccc;'>You initiated a credentials recovery bypass from your active profile portal. Use the verification token code below:</p>
                <div style='background-color: #222; display: inline-block; padding: 15px 35px; font-size: 28px; font-weight: bold; letter-spacing: 5px; color: #fff; margin: 20px 0; border: 1px solid #444; border-radius: 4px;'>{otp}</div>
            </div>";
    }
}

