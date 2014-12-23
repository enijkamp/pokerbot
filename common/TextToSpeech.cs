using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.Threading;

namespace PokerBot
{
    public class TextToSpeech
    {
        private static SpeechSynthesizer speech = new SpeechSynthesizer();

        public static void SayAsnc(Settings settings, string text)
        {
            if (settings.Speech)
            {
                PromptBuilder builder = new PromptBuilder();
                PromptStyle style = new PromptStyle();
                style.Rate = PromptRate.Slow;
                builder.StartStyle(style);
                builder.AppendText(text);
                builder.EndStyle();
                speech.Volume = 100;
                speech.SpeakAsync(builder);
            }
        }

        public static void SayAsnc(Settings settings, HandTypes hand)
        {
            SayAsnc(settings, HandTypesText.Texts[(int)hand]); 
        }

        public static void Main(string[] args)
        {
            Console.Beep(300, 200);
            SayAsnc(new Settings(), HandTypes.Low_flush);
            Console.ReadKey();
        }
    }
}
