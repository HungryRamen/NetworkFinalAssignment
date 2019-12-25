using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
public class Mail : MonoBehaviour
{
    private void Start()
    {
    }

    void MailSend()
    {
        StringBuilder codeBuilder = new StringBuilder();

        for (int i = 0; i < 6; i++)
        {
            char a = ' ';
            int code = Random.Range(0, 35);
            if (code < 10)
            {
                a = code.ToString().ToCharArray()[0];
            }
            else
            {
                code += 55;
                a = (char)code;
            }
            codeBuilder.Append(a);
        }
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("boardgamebang@gmail.com");  //보내는 사람
        mail.To.Add("Gohan2255@gmail.com");      // 받는 사람
        mail.Subject = "Security Code";                // 제목
        mail.Body = codeBuilder.ToString();                     // 내용
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("boardgamebang@gmail.com", "asdfghjkl22@") as ICredentialsByHost;     // 아이디 비번 이 부분은 DB에서 받아오게 하기
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtpServer.Send(mail);
        Debug.Log("success");
    }
}
