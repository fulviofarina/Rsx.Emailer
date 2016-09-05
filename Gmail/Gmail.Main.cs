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
    public  static partial class Gmail
    {
        /// <summary>
        /// Gets the list of commands stored in the gmailFeed since the given input time and labeled under the given filter
        /// </summary>
        /// <param name="since">Start Datetime to filter the list of commands</param>
        /// <param name="gmailFeed">feed to read data from</param>
        /// <param name="filter">The commands to look for should start with the given filter string</param>
        /// <returns></returns>
        public static IList<object[]> CheckCmds(ref System.DateTime since, ref AtomFeed gmailFeed, string filter)
        {
            gmailFeed.GetFeed(5);

            // Access the feeds XmlDocument
            System.Xml.XmlDocument myXml = gmailFeed.FeedXml;

            // Access the raw feed as a string
            string feedString = gmailFeed.RawFeed;
            // Access the feed through the object
            string feedTitle = gmailFeed.Title;
            string feedTagline = gmailFeed.Message;
            DateTime feedModified = gmailFeed.Modified;

            DateTime reference = since;
            filter = filter.ToUpper();
            IEnumerable<AtomFeed.AtomFeedEntry> entries = gmailFeed.FeedEntries.OfType<AtomFeed.AtomFeedEntry>();
            entries = entries.Where(e => e.Received >= reference).ToList();
            if (entries.Count() == 0) return null;
            entries = entries.Where(e => e.Subject.ToUpper().Contains(filter)).ToList();
            if (entries.Count() == 0) return null;
            entries = entries.OrderByDescending(o => o.Received).ToList();
            HashSet<string> hs = new HashSet<string>();
            entries = entries.TakeWhile(o => hs.Add(o.Subject)).ToList();

            List<object[]> ls = new List<object[]>();
            foreach (AtomFeed.AtomFeedEntry e in entries)
            {
                object[] arr = new object[4];
                arr[0] = e.Subject.ToUpper().Remove(0, 2).Trim();
                arr[1] = e.FromEmail.Trim();
                arr[2] = e.Summary;
                arr[3] = e.Received;
                ls.Add(arr);
            }
            entries = null;
            hs.Clear();
            hs = null;
            return ls;
        }

        public static void ReadAtomFeed(string user, string password)
        {
            AtomFeed gmailFeed = new AtomFeed(user, password);
            gmailFeed.GetFeed();

            // Access the feeds XmlDocument
            System.Xml.XmlDocument myXml = gmailFeed.FeedXml;

            // Access the raw feed as a string
            string feedString = gmailFeed.RawFeed;

            // Access the feed through the object
            string feedTitle = gmailFeed.Title;
            string feedTagline = gmailFeed.Message;
            DateTime feedModified = gmailFeed.Modified;

            //Get the entries
            for (int i = 0; i < gmailFeed.FeedEntries.Count; i++)
            {
                string entryAuthorName = gmailFeed.FeedEntries[i].FromName;
                string entryAuthorEmail = gmailFeed.FeedEntries[i].FromEmail;
                string entryTitle = gmailFeed.FeedEntries[i].Subject;
                string entrySummary = gmailFeed.FeedEntries[i].Summary;
                DateTime entryIssuedDate = gmailFeed.FeedEntries[i].Received;
                string entryId = gmailFeed.FeedEntries[i].Id;
            }
        }

        /// <summary>
        /// Provides an easy method of retreiving and programming against gmail atom feeds.
        /// </summary>
      
    }

}