using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using FinancialTracker_Web.Models;
using Microsoft.AspNet.Identity;

namespace FinancialTracker_Web.Helpers
{
    public class AvatarHelper
    {
        private const string AVATAR_DIRECTORY = "~/Images/avatars/";

        public static string ProcessUpload(HttpServerUtilityBase Server, HttpPostedFileBase imageFile) {


            try {
                if( imageFile.ContentLength > 3145728 ) { //3MB in binary bytes
                    throw new Exception("File is too big!");
                } 
                if(!imageFile.ContentType.ToLower().StartsWith("image") ) {
                    throw new Exception("File is not an image!");
                } 

                //get file info
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var fileExt = Path.GetExtension(imageFile.FileName);
                var fileNameModded = $"{URLFriendly(fileName)}-{DateTime.Now.Ticks}{fileExt}";
                var localFilePath = Path.Combine(Server.MapPath(AVATAR_DIRECTORY), fileNameModded);

                //save to attachment path
                imageFile.SaveAs(localFilePath);

                //if save was successful, return path of saved file.
                return AVATAR_DIRECTORY + fileNameModded; ;

            } catch {
                return null;
            }
        }

        public static string GetUserAvatar(IPrincipal User) {
            var imgPath = new AppDbContext().Users.Find(User.Identity.GetUserId()).AvatarImagePath;
            if( !string.IsNullOrEmpty(imgPath) ) { return imgPath; }
            return null;
        }



        private static string URLFriendly(string title) {
            if( title == null ) return "";
            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;
            for( int i = 0; i < len; i++ ) {
                c = title[ i ];
                if( ( c >= 'a' && c <= 'z' ) || ( c >= '0' && c <= '9' ) ) {
                    sb.Append(c);
                    prevdash = false;
                } else if( c >= 'A' && c <= 'Z' ) {
                    // tricky way to convert to lowercase
                    sb.Append((char)( c | 32 ));
                    prevdash = false;
                } else if( c == ' ' || c == ',' || c == '.' || c == '/' ||
                           c == '\\' || c == '-' || c == '_' || c == '=' ) {
                    if( !prevdash && sb.Length > 0 ) {
                        sb.Append('-');
                        prevdash = true;
                    }
                } else if( c == '#' ) {
                    if( i > 0 )
                        if( title[ i - 1 ] == 'C' || title[ i - 1 ] == 'F' )
                            sb.Append("-sharp");
                } else if( c == '+' ) {
                    sb.Append("-plus");
                } else if( (int)c >= 128 ) {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if( prevlen != sb.Length ) prevdash = false;
                }
                if( sb.Length == maxlen ) break;
            }
            if( prevdash )
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }
        private static string RemapInternationalCharToAscii(char c) {
            string s = c.ToString().ToLowerInvariant();
            if( "àåáâäãåą".Contains(s) ) {
                return "a";
            } else if( "èéêëę".Contains(s) ) {
                return "e";
            } else if( "ìíîïı".Contains(s) ) {
                return "i";
            } else if( "òóôõöøőð".Contains(s) ) {
                return "o";
            } else if( "ùúûüŭů".Contains(s) ) {
                return "u";
            } else if( "çćčĉ".Contains(s) ) {
                return "c";
            } else if( "żźž".Contains(s) ) {
                return "z";
            } else if( "śşšŝ".Contains(s) ) {
                return "s";
            } else if( "ñń".Contains(s) ) {
                return "n";
            } else if( "ýÿ".Contains(s) ) {
                return "y";
            } else if( "ğĝ".Contains(s) ) {
                return "g";
            } else if( c == 'ř' ) {
                return "r";
            } else if( c == 'ł' ) {
                return "l";
            } else if( c == 'đ' ) {
                return "d";
            } else if( c == 'ß' ) {
                return "ss";
            } else if( c == 'Þ' ) {
                return "th";
            } else if( c == 'ĥ' ) {
                return "h";
            } else if( c == 'ĵ' ) {
                return "j";
            } else {
                return "";
            }
        }
    }
}