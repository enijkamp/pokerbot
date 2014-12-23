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
	public class TestIdentifyHand : TestBase
	{
		
		public static void Main(string[] args)
		{		
			// patterns
			List<CardPattern> patterns = CardReader.readCardsFromResources();

			Console.WriteLine("read "+patterns.Count+" candidate patterns");
			
			// renderer
			ImagesRenderer renderer = newImageRenderer();
			
			// iterator			
			Iterator<Image> wait = new MockIterator(toImage(new Bitmap("hand.png")));
			
			//Iterator<Image> screen = new ScreenImageIterator(new Rectangle(400, 400, 300, 80));
			//Iterator<Image> wait = new WaitDeltaImageIterator(screen);
			Iterator<Image> low = new ReduceColorIterator(wait, new ColorReducers.Card());
						
			// proxy
			IteratorProxy<Image> proxyIter = new IteratorProxy<Image>(low);
			proxyIter.handler += delegate(Image next) 
			{
				setImage(renderer, toBitmap(next));
		    };
			Console.WriteLine("initialized iterator");
			
			// identifier
			PocketIdentifier identifier = new PocketIdentifier(patterns);
			
			// go
			while(proxyIter.hasNext())
			{
				Image screen = proxyIter.next();
				DateTime start = DateTime.Now;	
				List<Card> hand = identifier.identifyCards(screen);
				double ms = DateTime.Now.Subtract(start).TotalMilliseconds;
				Console.WriteLine("identification took " + ms + "ms");
				Console.WriteLine(toText(hand));
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