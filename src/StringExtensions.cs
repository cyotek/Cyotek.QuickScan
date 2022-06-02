using System.Collections.Generic;
using System.Text;

namespace Cyotek.QuickScan
{
  internal static class StringExtensions
  {
    #region Public Methods

    public static string MailMerge(this string text, IDictionary<string, string> tokens, char tokenStart, char tokenEnd)
    {
      if (text != null && tokens != null && tokens.Count != 0)
      {
        int length;

        length = text.Length;

        if (length > 2)
        {
          StringBuilder sb;
          StringBuilder name;
          bool readingField;

          sb = new StringBuilder(length);
          name = new StringBuilder(16);
          readingField = false;

          for (int i = 0; i < length; i++)
          {
            char c;

            c = text[i];

            if (c == tokenStart && !readingField)
            {
              readingField = true;
            }
            else if (c == tokenEnd && readingField)
            {
              string key;

              key = name.ToString();

              if (tokens.TryGetValue(key, out string value))
              {
                sb.Append(value);
              }
              else if (tokens.TryGetValue(tokenStart + key + tokenEnd, out value))
              {
                sb.Append(value);
              }
              else
              {
                sb.Append(tokenStart);
                i -= (key.Length + 1);
              }

              readingField = false;
              name.Length = 0;
            }
            else if (readingField)
            {
              name.Append(c);
            }
            else
            {
              sb.Append(c);
            }
          }

          if (readingField)
          {
            // malformed tag
            sb.Append(tokenStart);
            sb.Append(name);
          }

          text = sb.ToString();
        }
      }

      return text;
    }

    #endregion Public Methods
  }
}