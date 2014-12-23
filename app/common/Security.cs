using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace PokerBot
{
    public class Security
    {
        #region unmanaged

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion

        public const int VERSION = 27;

        private static HandlerRoutine handler;
        private static bool handlersAttached = false;

        public static bool CheckLogin(string user, string password)
        {
            string url = "http://poker.eriknijkamp.com/passwords.php?user=" + user + "&version=" + VERSION;

            WebRequest request = HttpWebRequest.Create(url);

            WebResponse response = request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());

            string urlText = reader.ReadToEnd();

            bool ok = password == Decrypt(urlText);
            if (!ok) InvalideLogin(user);
            return ok;
        }

        private static string Decrypt(string password)
        {
            char[] chars = password.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(password[i] + 1);
            }
            return new String(chars);
        }

        public static void StartSession(string user, string pokeruser, int tables, int rules)
        {
            // add exit handler for console
            if (!handlersAttached)
            {
                handler = delegate(CtrlTypes c) { StopSession(user, pokeruser); return true; };
                SetConsoleCtrlHandler(handler, true);

                // add exit handler for form
                AppDomain.CurrentDomain.ProcessExit += delegate(object s, EventArgs a) { StopSession(user, pokeruser); };
                handlersAttached = true;
            }
            // sessions
            string url = "http://poker.eriknijkamp.com/sessions.php?user=" + user + "&action=start_" + pokeruser + "_" + tables + "_" + rules;
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
        }

        private static void StopSession(string user, string pokeruser)
        {
            string url = "http://poker.eriknijkamp.com/sessions.php?user=" + user + "&action=stop_" + pokeruser;
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
        }

        private static void InvalideLogin(string user)
        {
            string url = "http://poker.eriknijkamp.com/sessions.php?user=" + user + "&action=invalid_login";
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
        }

        public static void AddTenMinutes(string user)
        {
            try
            {
                string url = "http://poker.eriknijkamp.com/time.php?user=" + user;
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
            }
            catch (Exception ex)
            {
                Log.Error("AddTenMinutes() Exception: " + ex.ToString());
            }
        }
    }
}
