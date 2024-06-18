
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;

public static class Emailer
{
    public static bool SendEmail(EmailSender p_sender, EmailData p_data, string p_emailReceiver, List<string> p_pictures)
    {
        if (p_pictures.Count == 0)
        {
            return false;
        }
        var message = new MimeMessage();
        message.From.Add( new MailboxAddress( p_sender.nameSender, p_sender.address) );
        message.To.Add( new MailboxAddress( "HH - Email Receiver", p_emailReceiver ) );
        message.Subject = p_data.emailSubject.GetLocalizedString();

        var multipartBody = new Multipart( "mixed" );
        {
            var textPart = new TextPart( "plain" )
            {
                Text = p_data.emailText.GetLocalizedString()
            };
            multipartBody.Add( textPart );

            foreach (var l_picture in p_pictures)
            {
                string attachmentPath = l_picture;
                var attachmentPart = new MimePart( "image/png" )
                {
                    Content = new MimeContent( File.OpenRead( attachmentPath ), ContentEncoding.Default ),
                    ContentDisposition = new ContentDisposition( ContentDisposition.Attachment ),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName( attachmentPath )
                };
                multipartBody.Add( attachmentPart );
            }
          
            
        }
        message.Body = multipartBody;

        using ( var client = new SmtpClient() )
        {
            // This section must be changed based on your p_sender's email host
            // Do not use Gmail
            // client.Connect( "smtp-mail.outlook.com", 587, false );
            client.Connect( p_sender.service, p_sender.port, false );

            client.AuthenticationMechanisms.Remove( "XOAUTH2" );
            // client.Authenticate( "mugshot-cnam@outlook.com", "mugsh0tG4me!" );
            client.Authenticate( p_sender.address, p_sender.password);
            client.Send( message );
            client.Disconnect( true );
            Debug.Log( "Sent email" );
        }
        multipartBody.Dispose();
        message.Dispose();
        return true;
    }
}

