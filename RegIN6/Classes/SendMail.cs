using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RegIN6.Classes
{
    public class SendMail
    {
        public static void SendMessage(string Message, string To)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential("veber.vasilisa2006@yandex.ru", "togjvzmpwymyycyq"),
                EnableSsl = true,
            };
            smtpClient.Send("veber.vasilisa2006@yandex.ru", To, "Проект RegIn", Message);
        }

    }
}
