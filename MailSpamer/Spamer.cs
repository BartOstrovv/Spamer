using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace MailSpamer
{
    public class Spamer
    {
        private readonly string sender;
        private SmtpClient _smtp;
        private MailMessage _message;

        public Spamer(string login, string password)
        {
            _smtp = new SmtpClient("smtp.gmail.com", 587);
            _smtp.Credentials = new NetworkCredential(login, password);
            _smtp.EnableSsl = true;
            sender = login;
        }

        public Spamer(Spamer obj)
        {
            this.sender = obj.sender;
            this._smtp = obj._smtp;
            this._message = obj._message;
        }

        public bool InitMessage(string recepient, string subject, string body)
        {
            _message = new MailMessage(sender, recepient, subject, body);
            _message.IsBodyHtml = true;
            return _message != null;
        }

        public bool PutFiles(string file)
        {
            if (_message == null)
                return false;
           
            _message.Attachments.Add(new Attachment(file));

            return _message.Attachments.Count != 0;
        }

        public void ClearFiles()
        {
            if (_message == null)
                return;

            _message.Attachments.Clear();
        }

        public void SendMessage()
        {
            if ((_message != null) && (_smtp != null))
            {
                _smtp.Send(_message);
            }
        }
    }
}
