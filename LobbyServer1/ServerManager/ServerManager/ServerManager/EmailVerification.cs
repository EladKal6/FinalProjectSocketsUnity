using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace ServerManager
{
    class EmailVerification
    {
        public static void SendEmail(string clientEmail, int verificationCode)
        {
            //create a new message
            MimeMessage message = new MimeMessage();
            //sender info
            message.From.Add(new MailboxAddress("MINIGAMES TOURNAMENT GAME", <enter mail here!!>));
            //receiver info
            message.To.Add(MailboxAddress.Parse(clientEmail));
            //message Title
            message.Subject = "Hello!!";
            //add body
            message.Body = new TextPart("plain")
            {
                Text = @"YOUR VERIFICATION CODE IS: " + verificationCode
            };

            string senderEmailAddress = <enter mail here!!>;
            string senderPassword = <enter the password of the mail here!!>;

            //new SMTP Client
            SmtpClient mailClient = new SmtpClient();

            try
            {
                //connect to gmail server
                mailClient.Connect("smtp.gmail.com", 465, true);
                //authentication is enabled
                mailClient.Authenticate(senderEmailAddress, senderPassword);
                mailClient.Send(message);
                Console.WriteLine("Ëmail sent!");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                mailClient.Disconnect(true);
                mailClient.Dispose();
            }
        }
    }
}
