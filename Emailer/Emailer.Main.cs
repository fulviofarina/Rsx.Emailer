using System;
using System.Collections;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Rsx
{
  public partial class Emailer
  {
    /// <summary>
    /// Prepares a Mail Message for delivery with the given information.
    /// </summary>
    /// <param name="sendFrom"></param>
    /// <param name="sendSubject"></param>
    /// <param name="sendMessage"></param>
    /// <param name="attachments"></param>
    /// <param name="sendTo"></param>
    /// <returns></returns>
    public static System.Net.Mail.MailMessage PrepareMessage(string sendFrom, string sendSubject, string sendMessage, ArrayList attachments, string sendTo)
    {
      System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(sendFrom, sendTo, sendSubject, sendMessage);
      message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

      if (attachments != null)
      {
        foreach (string attach in attachments)
        {
          try
          {
            //"application/octet-stream"
            if (System.IO.File.Exists(attach))
            {
              Attachment attached = new Attachment(attach);
              message.Attachments.Add(attached);
            }
          }
          catch (SystemException ex)
          {
            System.IO.File.WriteAllText("Error.txt", attach + "\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            Attachment attached = new Attachment("Error.txt");
            message.Attachments.Add(attached);
          }
        }
      }
      return message;
    }

    public static void Dispose(ref SmtpClient client, ref System.Net.Mail.MailMessage message)
    {
      if (message != null)
      {
        for (int i = 0; i < message.Attachments.Count; i++)
        {
          Attachment a = message.Attachments[i];
          a.Dispose();
          a = null;
        }
        message.Attachments.Clear();
        message.Dispose();
        message = null;
      }
      if (client != null)
      {
        client.Dispose();
        client = null;
      }
    }

    /// <summary>
    /// For Asynchroneous Reporting...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected internal static void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      string result = "Message sent at " + DateTime.Now.ToString();
      Type tipo = e.UserState.GetType();

      if (tipo.Equals(typeof(object[])))
      {
        object[] pkg = e.UserState as object[];

        System.Messaging.MessageQueue qu = pkg[0] as System.Messaging.MessageQueue;
        System.Net.Mail.MailMessage mail = pkg[1] as System.Net.Mail.MailMessage;
        System.Net.Mail.SmtpClient client = sender as System.Net.Mail.SmtpClient;

        System.Messaging.Message m = new System.Messaging.Message();
        if (e.Error != null)
        {
          m.Body = new Exception(e.Error.Message);
          m.Label = "AsyncEmail FAILED";
        }
        else
        {
          if (mail.Attachments.Count > 0)
          {
            Attachment at = mail.Attachments[0];
            System.IO.FileStream stream = (System.IO.FileStream)at.ContentStream;
            m.Body = stream.Name;
            // stream.Unlock(0, stream.Length);
            stream.Close();
            stream.Dispose();
            ///NOT USED BUT COULD BE
            /*
           byte[] array = new byte[stream.Length];
           stream.Read(array, 0, (int)stream.Length);
           stream.Close();
           stream.Dispose();
           m.Extension = array;
               */
          }
          m.Label = "AsyncEmail OK";
        }
        m.Formatter = new System.Messaging.BinaryMessageFormatter();
        m.AppSpecific = 1;
        System.Messaging.MessageQueueTransaction mqt = new System.Messaging.MessageQueueTransaction();
        mqt.Begin();
        qu.Send(m, mqt);
        mqt.Commit();
        mqt.Dispose();
        mqt = null;
        m.Dispose();
        m = null;

        Dispose(ref client, ref mail);

        qu.BeginReceive();
      }
      else
      {
        if (e.Cancelled)
        {
          System.Windows.Forms.MessageBox.Show(e.Error.Message, "Email Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }
        if (e.Error != null)
        {
          System.Windows.Forms.MessageBox.Show(e.Error.Message, "Email NOT Sent!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
        else if (!e.Cancelled)
        {
          System.Windows.Forms.MessageBox.Show(result, "Email Sent!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
      }
    }

    /// <summary>
    /// Validates an email address
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    public static bool ValidateEmailAddress(string emailAddress)
    {
      bool go = false;
      try
      {
        string TextToValidate = emailAddress;
        Regex expression = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");
        if (expression.IsMatch(TextToValidate))
        {
          return true;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
      return go;
    }
  }
}