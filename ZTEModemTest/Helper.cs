using System;
using System.IO;
using System.Text;

namespace ZTEModemTest
{
    public static class Helper
    {
        public static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        public static String UnicodeStr2HexStr( this string strMessage)
        {
            byte[] ba = Encoding.BigEndianUnicode.GetBytes(strMessage);
            String strHex = BitConverter.ToString(ba);
            strHex = strHex.Replace("-", "");
            return strHex;
        }

        public static String HexStr2UnicodeStr(this string strHex)
        {
            byte[] ba = HexStr2HexBytes(strHex);
            return HexBytes2UnicodeStr(ba);
        }

        //================> Used to decoding GSM UCS2 message  
        public static String HexBytes2UnicodeStr(byte[] ba)
        {
            var strMessage = Encoding.BigEndianUnicode.GetString(ba, 0, ba.Length);
            return strMessage;
        }

        public static byte[] HexStr2HexBytes(this string strHex)
        {
            strHex = strHex.Replace(" ", "");
            int nNumberChars = strHex.Length / 2;
            byte[] aBytes = new byte[nNumberChars];
            using (var sr = new StringReader(strHex))
            {
                for (int i = 0; i < nNumberChars; i++)
                    aBytes[i] = Convert.ToByte(new String(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return aBytes;
        }

    }
}