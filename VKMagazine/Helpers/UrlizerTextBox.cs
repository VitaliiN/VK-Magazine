using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VKMagazine.Helpers
{
    public class UrlizerTextBox : RichTextBox
    {
        // Copied from http://geekswithblogs.net/casualjim/archive/2005/12/01/61722.aspx
        private static readonly Regex UrlRegex = new Regex(@"((?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?)|(\[([^\|]+)\|([^\]]+)\])");

        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content",
            typeof(string),
            typeof(UrlizerTextBox),
            new PropertyMetadata(null, OnContentChanged)
        );

        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UrlizerTextBox textBox = d as UrlizerTextBox;
            if (textBox == null)
                return;

            textBox.Blocks.Clear();

            string newText = (string)e.NewValue;
            if (String.IsNullOrEmpty(newText))
                return;

            Paragraph paragraph = new Paragraph();
            try
            {
                int lastPos = 0;
                foreach (Match match in UrlRegex.Matches(newText))
                {
                    // Copy raw string from the last position up to the match
                    if (match.Index != lastPos)
                    {
                        string rawText = newText.Substring(lastPos, match.Index - lastPos);
                        paragraph.Inlines.Add(rawText);
                    }
                    
                    // Add matched url
                    Uri uri = null;
                    string rawUrl = match.Value;
                    if (rawUrl.StartsWith("[club") || rawUrl.StartsWith("[id"))
                    {
                        if (!Uri.TryCreate("http://vk.com/" + match.Groups[3].ToString(), UriKind.Absolute, out uri))
                        {
                            uri = new Uri("http://vk.com/", UriKind.Absolute);
                        }
                        rawUrl = match.Groups[4].ToString();
                    }
                    else
                    {

                        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out uri))
                        {
                            // Attempt to craft a valid url
                            if (!rawUrl.StartsWith("http://"))
                            {
                                Uri.TryCreate("http://" + rawUrl, UriKind.Absolute, out uri);
                            }
                        }
                    }
                    if (uri != null)
                    {
                        Hyperlink link = new Hyperlink()
                        {
                            NavigateUri = uri,
                            TargetName = "_blank",
                           
                        };
                        link.Inlines.Add(rawUrl);
                        paragraph.Inlines.Add(link);
                    }
                    else
                    {
                        paragraph.Inlines.Add(rawUrl);
                    }

                    // Update the last matched position
                    lastPos = match.Index + match.Length;
                }

                // Finally, copy the remainder of the string
                if (lastPos < newText.Length)
                    paragraph.Inlines.Add(newText.Substring(lastPos));
            }
            catch (Exception)
            {
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(newText);
            }

            // Add the paragraph to the RichTextBox.
            textBox.Blocks.Add(paragraph);
        }
    }
}
