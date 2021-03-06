using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PokerBot
{	
	public class TestIdentifyCardsFast : TestBase
	{
        // const
        private const bool USE_SCREEN = true;
		
		public static void Main(string[] args)
		{
            Log.SetLevel(Log.Level.FINEST);

			// patterns
			List<CardPattern> patterns = CardReader.readCardsFromResources();

			Console.WriteLine("read "+patterns.Count+" candidate patterns");
			
			// renderer
			ImagesRenderer renderer = newImageRenderer();
			
			// iterator			
			Iterator<Image> wait;

            if (USE_SCREEN)
            {
                Thread.Sleep(10);
                TableContainer table = new BotAppLogic(new Win32Control()).LocateNewTable(new Settings());
                Rectangle rect = new Rectangle(table.Layout.Offset.X, table.Layout.Offset.Y, table.Layout.Size.Width, table.Layout.Size.Height);
                Iterator<Image> screen = new ScreenImageIterator(new Win32Control(), rect);
                wait = new WaitDeltaImageIterator(screen);
            }
            else
            {
                wait = new MockIterator(toImage(new Bitmap("test/table_no_middle_button.png")));
            }
						
			// proxy
            IteratorProxy<Image> proxyIter = new IteratorProxy<Image>(wait);
			proxyIter.handler += delegate(Image next) 
			{
				setImage(renderer, toBitmap(next));
		    };
			Console.WriteLine("initialized iterator");
			
			// identifier
			CardStrategy strategy = new CardStrategyFast(patterns, new TableLayout9());
			CardIdentifierIterator identifier = new CardIdentifierIterator(proxyIter, 
			                                               strategy.identifyRegions, 
			                                               strategy.identifyCards);			
			// go
			while(identifier.hasNext())
			{
				DateTime start = DateTime.Now;	
				List<Card> cards = identifier.next();
				setText(renderer, toText(cards));
				Console.WriteLine("iteration took " + DateTime.Now.Subtract(start).TotalSeconds + "s");
                Console.ReadKey();
			}
		}
		
		private static string toText(List<Card> cards)
		{
			string text = "";
			foreach(Card card in cards)
			{
				text += "[" + card + "]  ";
			}
			return text;			
		}
		
	}
}
