//using System.Linq;
//using System.Security.Cryptography;
//using UnityEngine;
//
//namespace BFF
//{
//    public class ESDataHelper
//    {
//        private static string _unObfuscateHashKey = "";
//    
//        public static string GetESSignature(string contentStr, string hashKey)
//        {
//            return HmacSha256(contentStr, GetUnObfuscateHashKey(hashKey));
//        }
//        
//        #region Method HmacSha256 and UnObfuscateString
//
//        private static string GetUnObfuscateHashKey(string hashKey)
//        {
//            _unObfuscateHashKey = _unObfuscateHashKey == "" ? UnObfuscateString(hashKey) : _unObfuscateHashKey;
//            return _unObfuscateHashKey;
//        }
//        
//        private static string HmacSha256(string message, string secret)
//        {
//            if (secret == "")
//            {
//                Debug.LogError("no hashkey");
//            }
//            
//            var encoding = new System.Text.UTF8Encoding();
//            var keyByte = encoding.GetBytes(secret);
//            var messageBytes = encoding.GetBytes(message);
//            var hmacsha256 = new HMACSHA256(keyByte);
//            var hashmessage = hmacsha256.ComputeHash(messageBytes);
//    
//            var hexString = hashmessage.Aggregate("", (current, t) => current + $"{t:X2}");
//    
//            return hexString.ToLower();
//        }
//        
//        private static string UnObfuscateString(string str)
//        {
//            if (str.Length != 32)
//            {
//                return null;
//            }
//    
//            var key = new char[33];
//            key[32] = '0';
//    
//            var charArr = str.ToCharArray();
//    
//            for (var i = 0; i < 32; ++i)
//            {
//                var v = charArr[i];
//                if (v <= 'p')
//                {
//                    if (v <= '^')
//                    {
//                        if (v <= 'Q')
//                        {
//                            if (v <= '6')
//                            {
//                                if (v <= '.')
//                                {
//                                    if (v <= ')')
//                                    {
//                                        if (v <= '!')
//                                        {
//                                            if (v <= ' ')
//                                            {
//                                                if (v == ' ')
//                                                {
//                                                    key[i] = 'f';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = (char) 136;
//                                                if (v == '!')
//                                                {
//                                                    key[i] = 'v';
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 162;
//                                            if (v <= '%')
//                                            {
//                                                if (v <= '#')
//                                                {
//                                                    if (v <= '"')
//                                                    {
//                                                        if (v == '"')
//                                                        {
//                                                            key[i] = 'r';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = '6';
//                                                        if (v == '#')
//                                                        {
//                                                            key[i] = 'H';
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 218;
//                                                    if (v <= '$')
//                                                    {
//                                                        if (v == '$')
//                                                        {
//                                                            key[i] = ';';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = '@';
//                                                        if (v == '%')
//                                                        {
//                                                            key[i] = 'W';
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = '0';
//                                                if (v <= '&')
//                                                {
//                                                    if (v == '&')
//                                                    {
//                                                        key[i] = 'V';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 145;
//                                                    if (v <= '(')
//                                                    {
//                                                        if (v <= '\'')
//                                                        {
//                                                            if (v == '\'')
//                                                            {
//                                                                key[i] = 'm';
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            key[i] = (char) 140;
//                                                            if (v == '(')
//                                                            {
//                                                                key[i] = 'I';
//                                                            }
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = (char) 216;
//                                                        if (v == ')')
//                                                        {
//                                                            key[i] = 'G';
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 179;
//                                        if (v <= '*')
//                                        {
//                                            if (v == '*')
//                                            {
//                                                key[i] = '+';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 227;
//                                            if (v <= ',')
//                                            {
//                                                if (v <= '+')
//                                                {
//                                                    if (v == '+')
//                                                    {
//                                                        key[i] = 'F';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 2;
//                                                    if (v == ',')
//                                                    {
//                                                        key[i] = '`';
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = (char) 146;
//                                                if (v <= '-')
//                                                {
//                                                    if (v == '-')
//                                                    {
//                                                        key[i] = 'Z';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 219;
//                                                    if (v == '.')
//                                                    {
//                                                        key[i] = 'z';
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = 's';
//                                    if (v <= '5')
//                                    {
//                                        if (v <= '1')
//                                        {
//                                            if (v <= '0')
//                                            {
//                                                if (v <= '/')
//                                                {
//                                                    if (v == '/')
//                                                    {
//                                                        key[i] = 'k';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = 'C';
//                                                    if (v == '0')
//                                                    {
//                                                        key[i] = 'E';
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = (char) 160;
//                                                if (v == '1')
//                                                {
//                                                    key[i] = 'n';
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = '\r';
//                                            if (v <= '4')
//                                            {
//                                                if (v <= '3')
//                                                {
//                                                    if (v <= '2')
//                                                    {
//                                                        if (v == '2')
//                                                        {
//                                                            key[i] = '/';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = 'M';
//                                                        if (v == '3')
//                                                        {
//                                                            key[i] = '9';
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 185;
//                                                    if (v == '4')
//                                                    {
//                                                        key[i] = '-';
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = 'L';
//                                                if (v == '5')
//                                                {
//                                                    key[i] = '2';
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = '9';
//                                        if (v == '6')
//                                        {
//                                            key[i] = '|';
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                key[i] = (char) 169;
//                                if (v <= 'C')
//                                {
//                                    if (v <= '@')
//                                    {
//                                        if (v <= '7')
//                                        {
//                                            if (v == '7')
//                                            {
//                                                key[i] = '%';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = ')';
//                                            if (v <= '8')
//                                            {
//                                                if (v == '8')
//                                                {
//                                                    key[i] = '\\';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = '9';
//                                                if (v <= ':')
//                                                {
//                                                    if (v <= '9')
//                                                    {
//                                                        if (v == '9')
//                                                        {
//                                                            key[i] = 'S';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = 'S';
//                                                        if (v == ':')
//                                                        {
//                                                            key[i] = '}';
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = 'T';
//                                                    if (v <= '=')
//                                                    {
//                                                        if (v <= ';')
//                                                        {
//                                                            if (v == ';')
//                                                            {
//                                                                key[i] = 'T';
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            key[i] = (char) 213;
//                                                            if (v <= '<')
//                                                            {
//                                                                if (v == '<')
//                                                                {
//                                                                    key[i] = 't';
//                                                                }
//                                                            }
//                                                            else
//                                                            {
//                                                                key[i] = (char) 210;
//                                                                if (v == '=')
//                                                                {
//                                                                    key[i] = 'y';
//                                                                }
//                                                            }
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = (char) 176;
//                                                        if (v <= '>')
//                                                        {
//                                                            if (v == '>')
//                                                            {
//                                                                key[i] = 'd';
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            key[i] = '_';
//                                                            if (v <= '?')
//                                                            {
//                                                                if (v == '?')
//                                                                {
//                                                                    key[i] = 'l';
//                                                                }
//                                                            }
//                                                            else
//                                                            {
//                                                                key[i] = 'U';
//                                                                if (v == '@')
//                                                                {
//                                                                    key[i] = ' ';
//                                                                }
//                                                            }
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = '}';
//                                        if (v <= 'B')
//                                        {
//                                            if (v <= 'A')
//                                            {
//                                                if (v == 'A')
//                                                {
//                                                    key[i] = 'o';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = (char) 150;
//                                                if (v == 'B')
//                                                {
//                                                    key[i] = 'g';
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 184;
//                                            if (v == 'C')
//                                            {
//                                                key[i] = '[';
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 26;
//                                    if (v <= 'M')
//                                    {
//                                        if (v <= 'J')
//                                        {
//                                            if (v <= 'F')
//                                            {
//                                                if (v <= 'E')
//                                                {
//                                                    if (v <= 'D')
//                                                    {
//                                                        if (v == 'D')
//                                                        {
//                                                            key[i] = 'a';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = (char) 24;
//                                                        if (v == 'E')
//                                                        {
//                                                            key[i] = 's';
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 142;
//                                                    if (v == 'F')
//                                                    {
//                                                        key[i] = 'Y';
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = 'h';
//                                                if (v <= 'H')
//                                                {
//                                                    if (v <= 'G')
//                                                    {
//                                                        if (v == 'G')
//                                                        {
//                                                            key[i] = 'X';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = (char) 15;
//                                                        if (v == 'H')
//                                                        {
//                                                            key[i] = ')';
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = '&';
//                                                    if (v <= 'I')
//                                                    {
//                                                        if (v == 'I')
//                                                        {
//                                                            key[i] = 'u';
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        key[i] = (char) 198;
//                                                        if (v == 'J')
//                                                        {
//                                                            key[i] = '4';
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 213;
//                                            if (v <= 'L')
//                                            {
//                                                if (v <= 'K')
//                                                {
//                                                    if (v == 'K')
//                                                    {
//                                                        key[i] = '>';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = (char) 224;
//                                                    if (v == 'L')
//                                                    {
//                                                        key[i] = 'U';
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = '`';
//                                                if (v == 'M')
//                                                {
//                                                    key[i] = '\'';
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 0;
//                                        if (v <= 'P')
//                                        {
//                                            if (v <= 'N')
//                                            {
//                                                if (v == 'N')
//                                                {
//                                                    key[i] = 'Q';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = ' ';
//                                                if (v <= 'O')
//                                                {
//                                                    if (v == 'O')
//                                                    {
//                                                        key[i] = '?';
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    key[i] = 'm';
//                                                    if (v == 'P')
//                                                    {
//                                                        key[i] = '#';
//                                                    }
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 247;
//                                            if (v == 'Q')
//                                            {
//                                                key[i] = 'O';
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        else
//                        {
//                            key[i] = (char) 141;
//                            if (v <= 'W')
//                            {
//                                if (v <= 'T')
//                                {
//                                    if (v <= 'R')
//                                    {
//                                        if (v == 'R')
//                                        {
//                                            key[i] = 'A';
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 131;
//                                        if (v <= 'S')
//                                        {
//                                            if (v == 'S')
//                                            {
//                                                key[i] = '{';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 219;
//                                            if (v == 'T')
//                                            {
//                                                key[i] = ':';
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = 'L';
//                                    if (v <= 'V')
//                                    {
//                                        if (v <= 'U')
//                                        {
//                                            if (v == 'U')
//                                            {
//                                                key[i] = '3';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = '{';
//                                            if (v == 'V')
//                                            {
//                                                key[i] = '"';
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = '\n';
//                                        if (v == 'W')
//                                        {
//                                            key[i] = 'M';
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                key[i] = (char) 233;
//                                if (v <= '[')
//                                {
//                                    if (v <= 'Y')
//                                    {
//                                        if (v <= 'X')
//                                        {
//                                            if (v == 'X')
//                                            {
//                                                key[i] = 'B';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 152;
//                                            if (v == 'Y')
//                                            {
//                                                key[i] = 'i';
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 242;
//                                        if (v <= 'Z')
//                                        {
//                                            if (v == 'Z')
//                                            {
//                                                key[i] = '=';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = 'F';
//                                            if (v == '[')
//                                            {
//                                                key[i] = 'q';
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = 'V';
//                                    if (v <= '\\')
//                                    {
//                                        if (v == '\\')
//                                        {
//                                            key[i] = 'P';
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 166;
//                                        if (v <= ']')
//                                        {
//                                            if (v == ']')
//                                            {
//                                                key[i] = 'h';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 189;
//                                            if (v == '^')
//                                            {
//                                                key[i] = '(';
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        key[i] = (char) 223;
//                        if (v <= 'c')
//                        {
//                            if (v <= 'b')
//                            {
//                                if (v <= 'a')
//                                {
//                                    if (v <= '`')
//                                    {
//                                        if (v <= '_')
//                                        {
//                                            if (v == '_')
//                                            {
//                                                key[i] = '0';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = '-';
//                                            if (v == '`')
//                                            {
//                                                key[i] = 'x';
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 180;
//                                        if (v == 'a')
//                                        {
//                                            key[i] = 'c';
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 3;
//                                    if (v == 'b')
//                                    {
//                                        key[i] = '7';
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                key[i] = '>';
//                                if (v == 'c')
//                                {
//                                    key[i] = 'j';
//                                }
//                            }
//                        }
//                        else
//                        {
//                            key[i] = (char) 157;
//                            if (v <= 'g')
//                            {
//                                if (v <= 'd')
//                                {
//                                    if (v == 'd')
//                                    {
//                                        key[i] = '_';
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 187;
//                                    if (v <= 'e')
//                                    {
//                                        if (v == 'e')
//                                        {
//                                            key[i] = 'J';
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = '!';
//                                        if (v <= 'f')
//                                        {
//                                            if (v == 'f')
//                                            {
//                                                key[i] = '6';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 240;
//                                            if (v == 'g')
//                                            {
//                                                key[i] = '&';
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                key[i] = (char) 167;
//                                if (v <= 'k')
//                                {
//                                    if (v <= 'j')
//                                    {
//                                        if (v <= 'i')
//                                        {
//                                            if (v <= 'h')
//                                            {
//                                                if (v == 'h')
//                                                {
//                                                    key[i] = 'L';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = (char) 240;
//                                                if (v == 'i')
//                                                {
//                                                    key[i] = 'R';
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 250;
//                                            if (v == 'j')
//                                            {
//                                                key[i] = ',';
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 26;
//                                        if (v == 'k')
//                                        {
//                                            key[i] = 'w';
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 2;
//                                    if (v <= 'n')
//                                    {
//                                        if (v <= 'l')
//                                        {
//                                            if (v == 'l')
//                                            {
//                                                key[i] = 'e';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = 'X';
//                                            if (v <= 'm')
//                                            {
//                                                if (v == 'm')
//                                                {
//                                                    key[i] = 'b';
//                                                }
//                                            }
//                                            else
//                                            {
//                                                key[i] = 't';
//                                                if (v == 'n')
//                                                {
//                                                    key[i] = 'N';
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = '*';
//                                        if (v <= 'o')
//                                        {
//                                            if (v == 'o')
//                                            {
//                                                key[i] = 'D';
//                                            }
//                                        }
//                                        else
//                                        {
//                                            key[i] = (char) 128;
//                                            if (v == 'p')
//                                            {
//                                                key[i] = '<';
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    key[i] = (char) 211;
//                    if (v <= 'x')
//                    {
//                        if (v <= 's')
//                        {
//                            if (v <= 'q')
//                            {
//                                if (v == 'q')
//                                {
//                                    key[i] = '~';
//                                }
//                            }
//                            else
//                            {
//                                key[i] = (char) 7;
//                                if (v <= 'r')
//                                {
//                                    if (v == 'r')
//                                    {
//                                        key[i] = '.';
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 131;
//                                    if (v == 's')
//                                    {
//                                        key[i] = 'p';
//                                    }
//                                }
//                            }
//                        }
//                        else
//                        {
//                            key[i] = (char) 185;
//                            if (v <= 'u')
//                            {
//                                if (v <= 't')
//                                {
//                                    if (v == 't')
//                                    {
//                                        key[i] = '!';
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 146;
//                                    if (v == 'u')
//                                    {
//                                        key[i] = '*';
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                key[i] = 'Y';
//                                if (v <= 'w')
//                                {
//                                    if (v <= 'v')
//                                    {
//                                        if (v == 'v')
//                                        {
//                                            key[i] = ']';
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 213;
//                                        if (v == 'w')
//                                        {
//                                            key[i] = '^';
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 217;
//                                    if (v == 'x')
//                                    {
//                                        key[i] = '1';
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        key[i] = (char) 22;
//                        if (v <= 'z')
//                        {
//                            if (v <= 'y')
//                            {
//                                if (v == 'y')
//                                {
//                                    key[i] = '@';
//                                }
//                            }
//                            else
//                            {
//                                key[i] = 'l';
//                                if (v == 'z')
//                                {
//                                    key[i] = '$';
//                                }
//                            }
//                        }
//                        else
//                        {
//                            key[i] = ')';
//                            if (v <= '{')
//                            {
//                                if (v == '{')
//                                {
//                                    key[i] = 'C';
//                                }
//                            }
//                            else
//                            {
//                                key[i] = (char) 161;
//                                if (v <= '|')
//                                {
//                                    if (v == '|')
//                                    {
//                                        key[i] = 'K';
//                                    }
//                                }
//                                else
//                                {
//                                    key[i] = (char) 14;
//                                    if (v <= '}')
//                                    {
//                                        if (v == '}')
//                                        {
//                                            key[i] = '8';
//                                        }
//                                    }
//                                    else
//                                    {
//                                        key[i] = (char) 204;
//                                        if (v == '~')
//                                        {
//                                            key[i] = '5';
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//    
//            return new string(key, 0, 32);
//        }
//        
//        #endregion
//    }
//}
//
