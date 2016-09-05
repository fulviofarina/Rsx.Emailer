using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Rsx
{
    public partial class Emailer
    {

        /// <summary>
        /// A Class for creating SmtpClients
        /// </summary>
        public static class Clients
        {
            public static System.Net.NetworkCredential MyGMailCredentials
            {
                get
                {
                    return new System.Net.NetworkCredential("k0x.help@gmail.com", "Helpme123");
                }
            }

            // private static System.Net.NetworkCredential myk0NACredentials;

            public static System.Net.NetworkCredential Myk0NACredentials
            {
                get
                {
                    return new System.Net.NetworkCredential("SCK\\k0naa", "Naa123");
                }
            }

            public static class Smtp
            {
                /// <summary>
                /// Infere a Smtp Client from the domain name
                /// </summary>
                /// <param name="sendFrom"></param>
                /// <param name="sendTo"></param>
                /// <returns></returns>
                public static SmtpClient CreateFromDomain(ref string sendFrom)
                {
                    SmtpClient client = null;
                    string host = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName.ToUpper();
                    if (host.Contains("SCK"))
                    {
                        sendFrom = "k0naa@sckcen.be";
                        client = CreateSCK(Myk0NACredentials);
                    }
                    else
                    {
                        sendFrom = "k0x.help@gmail.com";
                        client = CreateGmail(MyGMailCredentials);
                    }
                    return client;
                }

                /// <summary>
                /// Creates a Gmail Smtp Client()
                /// </summary>
                /// <returns></returns>
                public static SmtpClient CreateGmail(System.Net.NetworkCredential SMTPCredentials)
                {
                    //587 no sirve en casa?
                    System.Net.Mail.SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = SMTPCredentials;
                    client.Timeout = 4000;
                    return client;
                }

                public static SmtpClient CreateSCK(System.Net.NetworkCredential SMTPCredentials)
                {
                    System.Net.Mail.SmtpClient client = new SmtpClient("MAILSRV3.sck.be", 25);
                    client.EnableSsl = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = SMTPCredentials;
                    client.Timeout = 8000;

                    return client;
                }
            }
        }



      
    }
}