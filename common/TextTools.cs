using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PokerBot
{
    public class TextTools
    {
        public static double ParseDouble(string value)
        {
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberGroupSeparator = ".";
                return double.Parse(value, nfi);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to convert '" + value + "' to double", ex);
            }
        }

        public static bool ContainsChars(string chars)
        {
            foreach (char chr in chars.ToCharArray())
            {
                if (chr != '?' && chr != ' ')
                {
                    return true;
                }
            }
            return false;
        }


        public static bool ContainsDigit(string text)
        {
            foreach (char chr in text.ToCharArray())
            {
                if (IsNumeric(chr)) return true;
            }
            return false;
        }

        public static bool IsNumeric(char chr)
        {
            if (chr == '0') return true;
            if (chr == '1') return true;
            if (chr == '2') return true;
            if (chr == '3') return true;
            if (chr == '4') return true;
            if (chr == '5') return true;
            if (chr == '6') return true;
            if (chr == '7') return true;
            if (chr == '8') return true;
            if (chr == '9') return true;
            return false;
        }

        public static bool IsPoint(char chr)
        {
            if (chr == '.') return true;
            return false;
        }
    }
}
