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
	public class TestIdentifyCards : TestBase
	{
        // const
        private const bool USE_SCREEN = false;
		
		public static void Main(string[] args)
		{
            Log.SetLevel(Log.Level.FINE);

			// patterns
			List<CardPattern> patterns = CardReader.readCardsFromResources();

			Console.WriteLine("read "+patterns.Count+" candidate patterns");
			
			// renderer
			ImagesRenderer renderer = newImageRenderer();
			
			// iterator			
			Iterator<Image> wait;

            if (USE_SCREEN)
            {
                Iterator<Image> screen = new ScreenImageIterator(new Win32Control(), new Rectangle(400, 400, 300, 80));
                wait = new WaitDeltaImageIterator(screen);
            }
            else
            {
                wait = new MockIterator(toImage(new Bitmap("test/cards_56.png")));
            }
			
			Iterator<Image> low = new ReduceColorIterator(wait, new ColorReducers.Card());
						
			// proxy
			IteratorProxy<Image> proxyIter = new IteratorProxy<Image>(low);
			proxyIter.handler += delegate(Image next) 
			{
				setImage(renderer, toBitmap(next));
		    };
			Console.WriteLine("initialized iterator");
			
			// identifier
			CardStrategy strategy = new CardStrategySlow(patterns);
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
