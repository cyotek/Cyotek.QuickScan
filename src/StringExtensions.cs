using System;
using System.Collections.Generic;
using System.Text;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class StringExtensions
  {
    #region Public Methods

    public static bool EqualsIgnoreCase(this string a, string b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);

    public static string MailMerge(this string text, char tokenStart, char tokenEnd, Func<string, string> evaluate)
    {
      if (text != null)
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
              string result;

              key = name.ToString();
              result = evaluate(key);

              if (result != null)
              {
                sb.Append(result);
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