using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace PokerBot
{
    public class ErrorHandler
    {
        private const String ERROR_DIR = "errors";

        private static String ErrorDir
        {
            get
            {
                Directory.CreateDirectory(ERROR_DIR);
                return ERROR_DIR + "/";
            }
        }


        public static void BeepError()
        {
            Console.Beep(1100, 200);
            Thread.Sleep(100);
            Console.Beep(1100, 200);
        }

        public static void ReportException(Exception ex, string error)
        {
            BeepError();

            // console
            Log.Error(error + " -> " + ex.Message);

            // file
            string filename = ErrorDir + "Error_Exception_" + Math.Abs(DateTime.Now.Ticks);

            // save exception
            StreamWriter log = File.CreateText(filename + ".txt");
            log.WriteLine("Date: " + DateTime.Now.ToShortTimeString());
            log.WriteLine("Message: " + error + " -> " + ex.Message);
            log.WriteLine("Inner: " + ex.InnerException);
            log.WriteLine("Source: " + ex.Source);
            log.WriteLine("Trace: " + ex.StackTrace);
            log.Close();
        }


        public static void ReportExceptionWithImage(Exception ex, string error, Image image)
        {
            BeepError();

            // console
            Log.Error(error + " -> " + ex.Message);

            // file
            string filename = ErrorDir + "Error_Exception_" + Math.Abs(DateTime.Now.Ticks);

            // save exception
            StreamWriter log = File.CreateText(filename + ".txt");
            log.WriteLine("Date: " + DateTime.Now.ToShortTimeString());
            log.WriteLine("Message: " + error + " -> " + ex.Message);
            log.WriteLine("Inner: " + ex.InnerException);
            log.WriteLine("Source: " + ex.Source);
            log.WriteLine("Trace: " + ex.StackTrace);
            log.Close();

            // save image
            ImageTools.toBitmap(image).Save(filename + "_Image.bmp");
        }


        public static void ReportBetException(Exception ex, Image table, Image bet, string error)
        {
            // console
            Log.Error("Cannot identify bet");
            Log.Error(error + " -> " + ex.Message);

            // file
            string filename = ErrorDir + "Error_Unable_Identify_Bet_" + Math.Abs(DateTime.Now.Ticks);

            // save exception
            StreamWriter log = File.CreateText(filename + ".txt");
            log.WriteLine("Date: " + DateTime.Now.ToShortTimeString());
            log.WriteLine("Message: " + error + " -> " + ex.Message);
            log.WriteLine("Inner: " + ex.InnerException);
            log.WriteLine("Source: " + ex.Source);
            log.WriteLine("Trace: " + ex.StackTrace);
            log.Close();

            // save image
            ImageTools.toBitmap(table).Save(filename + "_Table.bmp");
            ImageTools.toBitmap(bet).Save(filename + ".bmp");
        }

        public static void ReportTableException(Exception ex, Image image, string error)
        {
            // console
            Log.Error("Cannot identify table");
            Log.Error(error + " -> " + ex.Message);

            // file
            string filename = ErrorDir + "Error_Unable_Identify_Table_" + Math.Abs(DateTime.Now.Ticks);

            // save exception
            StreamWriter log = File.CreateText(filename + ".txt");
            log.WriteLine("Date: " + DateTime.Now.ToShortTimeString());
            log.WriteLine("Message: " + error + " -> " + ex.Message);
            log.WriteLine("Inner: " + ex.InnerException);
            log.WriteLine("Source: " + ex.Source);
            log.WriteLine("Trace: " + ex.StackTrace);
            log.Close();

            // save image
            ImageTools.toBitmap(image).Save(filename + ".bmp");
        }

        public static void ReportCardException(Image image, string error)
        {
            // console
            Log.Error("Cannot identify card");
            Log.Error(error);

            // file
            string filename = ErrorDir + "Error_Unable_Identify_Card_" + Math.Abs(DateTime.Now.Ticks);

            // save exception
            StreamWriter log = File.CreateText(filename + ".txt");
            log.WriteLine("Date: " + DateTime.Now.ToShortTimeString());
            log.WriteLine("Message: " + error);
            log.Close();

            // save image
            ImageTools.toBitmap(image).Save(filename + ".bmp");
        }
    }
}
