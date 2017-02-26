using System;
using System.Messaging;

namespace Rsx
{
  public partial class Emailer
  {
    /// <summary>
    /// A Class for creating SmtpClients
    /// </summary>

    public static string DecodeMessage(byte[] array)
    {
      System.Text.ASCIIEncoding d = new System.Text.ASCIIEncoding();
      System.Text.Decoder deco = d.GetDecoder();
      int charsconv = 0;
      int bytesconv = 0;
      bool conv = false;
      char[] cont = new char[array.Length];
      deco.Convert(array, 0, array.Length, cont, 0, cont.Length, true, out bytesconv, out charsconv, out conv);
      string bodyMsg = string.Empty;
      foreach (char c in cont) bodyMsg += c;

      return bodyMsg;
    }

    /// <summary>
    /// Creates and sends a default QMessage with the given body, label, using the given MessageQueue.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="label"></param>
    /// <param name="qMsg">Message Queue to use</param>
    /// <returns></returns>

    /// <summary>
    /// Creates a default msg
    /// </summary>
    /// <param name="body"></param>
    /// <param name="label"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static System.Messaging.Message CreateQMsg(object body, string label, string content)
    {
      byte[] cont = EncodeMessage(content);

      System.Messaging.Message w = new System.Messaging.Message(body);
      w.Label = label + " from " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
      w.Formatter = new System.Messaging.BinaryMessageFormatter();
      w.AppSpecific = 1;
      w.Extension = cont;
      w.Recoverable = true;
      return w;
    }

    /// <summary>
    /// Message must be provided, Sends a given msg
    /// </summary>
    /// <param name="qMsg"></param>
    /// <param name="w"></param>
    public static void SendQMsg(ref System.Messaging.MessageQueue qMsg, ref System.Messaging.Message w)
    {
      System.Messaging.MessageQueueTransaction mqt = new System.Messaging.MessageQueueTransaction();
      mqt.Begin();
      qMsg.Send(w, mqt);
      mqt.Commit();
      mqt.Dispose();
      mqt = null;
      w.Dispose();
      w = null;
    }

    public static byte[] EncodeMessage(string content)
    {
      byte[] cont = new byte[content.Length];

      System.Text.ASCIIEncoding coidn = new System.Text.ASCIIEncoding();
      System.Text.Encoder enc = coidn.GetEncoder();
      int charsconv = 0;
      int bytesconv = 0;
      bool conv = false;
      enc.Convert(content.ToCharArray(), 0, content.Length, cont, 0, cont.Length, true, out charsconv, out bytesconv, out conv);

      return cont;
    }

    /// <summary>
    /// Initializes a MessageQueue with binary formatter, with the input path and input OnReceive handler.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="handler">Method to call when the MQueue Receives a Message</param>
    /// <returns></returns>
    public static System.Messaging.MessageQueue CreateMQ(string path, ReceiveCompletedEventHandler handler)
    {
      System.Messaging.MessageQueue qMsg = null;
      try
      {
        if (!System.Messaging.MessageQueue.Exists(path))
        {
          System.Messaging.MessageQueue.Create(path, true);
        }
        qMsg = new MessageQueue(path, QueueAccessMode.SendAndReceive);
        qMsg.Path = path;
        qMsg.Formatter = new System.Messaging.BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
        qMsg.MessageReadPropertyFilter.DestinationQueue = true;
        qMsg.MessageReadPropertyFilter.Extension = true;
        qMsg.MessageReadPropertyFilter.IsFirstInTransaction = true;
        qMsg.MessageReadPropertyFilter.IsLastInTransaction = true;
        qMsg.MessageReadPropertyFilter.LookupId = true;
        qMsg.MessageReadPropertyFilter.Priority = true;
        qMsg.MessageReadPropertyFilter.SentTime = true;
        qMsg.MessageReadPropertyFilter.SourceMachine = true;
        qMsg.MessageReadPropertyFilter.TimeToBeReceived = true;
        qMsg.MessageReadPropertyFilter.TimeToReachQueue = true;
        if (handler != null) qMsg.ReceiveCompleted += new ReceiveCompletedEventHandler(handler);
      }
      catch (SystemException eX)
      {
        string msg = "Please take some time to FULLY install the Microsoft Message Queue Server\n\n";
        msg += "You'll need to hold the Window's Logo Key and press R\n\n";
        msg += "Write 'optionalfeatures' in the box and press Enter\n\nSelect the MSMQ package and click OK\n\n";
        msg += "Wait for the installation and then close this window\n\nThank you, this will activate the Bug Reporter";
        System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(msg, "Important", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
      }
      return qMsg;
    }
  }
}