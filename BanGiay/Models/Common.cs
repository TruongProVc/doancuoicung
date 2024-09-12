using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BanGiay.Models
{
    public class Common
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        private static string PasswdEmail = ConfigurationManager.AppSettings["PasswordEmail"];
        private static string Username = ConfigurationManager.AppSettings["Email"];
        public static string CreateID(int n)
        {
            string result = "";
            Random random = new Random();
            int id;
            for (int i = 0; i < n; i++)
            {
                id = random.Next(9);
                if (i == 1 && id == 0)
                {
                    id = 1;
                }
                result += id;
            }
            return result;
        }
        /// <summary>
        /// Hàm này dùng để gửi mail
        /// </summary>
        /// <param name="name">tên người gửi</param>
        /// <param name="subject">tiêu đề của mail</param>
        /// <param name="content">nội dung mail</param>
        /// <param name="tomail">địa chỉ mail người nhận</param>
        /// <returns></returns>
        public static async Task<bool> Sendmail(string name, string subject, string content, string tomail)
        {
            bool check = false;
            try
            {
                MailMessage mailMessage = new MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential() { UserName = Username, Password = PasswdEmail };

                MailAddress mailAddress = new MailAddress(Username, name);
                mailMessage.From = mailAddress;
                mailMessage.To.Add(tomail);
                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                smtp.Send(mailMessage);
                check = true;
            }
            catch (Exception)
            {
                check = false;
            }
            return check;
        }
        public static string RandomCharacter(int n)
        {
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                int index = random.Next(characters.Length);
                stringBuilder.Append(characters[index]);
            }
            return stringBuilder.ToString();
        }


    }
}