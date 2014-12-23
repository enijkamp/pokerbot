using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PokerBot
{	
	public class CardReader
	{
		private static ColorReducer reducer = new ColorReducers.Card();
		
		public static List<CardPattern> readCards(String path)
		{
			string[] cards = Directory.GetFiles(path, "card_*.png");
			List<CardPattern> patterns = new List<CardPattern>();
			foreach(String filepath in cards)
			{
				// get png
				Image png = readPng(filepath);
				
				// filename only
				string filename = Path.GetFileName(filepath);
				filename = filename.Replace("card_", "");
				
				// crop
				Image image = cropTransparentLines(png);
				
				// colors
				image = reducer.reduceColors(image);
				
				// new pattern
				Card.SuitEnum suit = toSuit(filename);
				Card.RankEnum rank = toRank(filename);
				CardPattern pattern = new CardPattern(image, suit, rank);
				patterns.Add(pattern);

			}
			return patterns;
		}
		
		public static List<CardPattern> readCardsFromResources()
		{
			List<CardPattern> patterns = new List<CardPattern>();
			
			// get a reference to the current assembly
            Assembly ass = Assembly.GetExecutingAssembly();
        
            // get a list of resource names from the manifest
            string[] names = ass.GetManifestResourceNames();
			
			foreach(string name in names)
			{
				// card?
				if(name.Contains("card"))
				{
					// split
					string cardName = name.Substring(name.IndexOf("_") + 1);
					
					// read from assembly
	                Stream imgStream = ass.GetManifestResourceStream(name);
					Bitmap bitmap = Bitmap.FromStream(imgStream) as Bitmap;
					Image image = toImage(bitmap);
					imgStream.Close();
					
					// crop
					image = cropTransparentLines(image);
					
					// colors
					image = reducer.reduceColors(image);
					
					// new pattern
					Card.SuitEnum suit = toSuit(cardName);
					Card.RankEnum rank = toRank(cardName);
					CardPattern pattern = new CardPattern(image, suit, rank);
					patterns.Add(pattern);
				}
			}
			
			return patterns;
		}
		
		private static IDictionary<char, Card.SuitEnum> suites = newSuites();
		
		private static IDictionary<char, Card.SuitEnum> newSuites()
		{
			IDictionary<char, Card.SuitEnum> dict = new Dictionary<char, Card.SuitEnum>();
			dict.Add('C', Card.SuitEnum.Clubs);
			dict.Add('D', Card.SuitEnum.Diamonds);
			dict.Add('H', Card.SuitEnum.Hearts);
			dict.Add('S', Card.SuitEnum.Spades);
			return dict;
		}

		private static Card.SuitEnum toSuit(String name)
		{
			char key = name.ToCharArray()[0];
			return suites[key];
		}
		
		private static IDictionary<String, Card.RankEnum> ranks = newRanks();

		private static IDictionary<String, Card.RankEnum> newRanks()
		{
			IDictionary<String, Card.RankEnum> dict = new Dictionary<String, Card.RankEnum>();
			dict.Add("2", Card.RankEnum.Two);
			dict.Add("3", Card.RankEnum.Three);
			dict.Add("4", Card.RankEnum.Four);
			dict.Add("5", Card.RankEnum.Five);
			dict.Add("6", Card.RankEnum.Six);
			dict.Add("7", Card.RankEnum.Seven);
			dict.Add("8", Card.RankEnum.Eigth);
			dict.Add("9", Card.RankEnum.Nine);
			dict.Add("10", Card.RankEnum.Ten);
			dict.Add("A", Card.RankEnum.Ace);
			dict.Add("J", Card.RankEnum.Jack);
			dict.Add("K", Card.RankEnum.King);
			dict.Add("Q", Card.RankEnum.Queen);
			return dict;
		}
		
		private static Card.RankEnum toRank(String name)
		{
			String key = name.Split('.')[0].Substring(1);
			return ranks[key];
		}
		
		private static Image readPng(String pattern)
		{
			Bitmap bitmap = new Bitmap(pattern);
			Image image = toImage(bitmap);				
			return image;
		}
		
		private static Image cropTransparentLines(Image image)
		{
			int yStart = 0;
			for(int y = 0; y < image.height; y++)
			{
				int[] line = image.getHorizontalLine(y);
				if(isTransparent(line))
				{
					yStart = y;
				}
				else
				{
					break;
				}
			}
			Image croppedImage = image.crop(0, image.width, yStart+1, image.height);
			return croppedImage;
		}
		
		private static bool isTransparent(int[] line)
		{
			foreach(int pixel in line)
			{
				if(pixel != Image.EmptyPixel)
					return false;
			}
			return true;
		}
		
		private static Image toImage(Bitmap bitmap) {
			int width = bitmap.Size.Width;
			int height = bitmap.Size.Height;
			int[] pixels = new int[width * height];

			BitmapData bData = bitmap.LockBits(
			                                   new Rectangle(new Point(), bitmap.Size),
			                                   ImageLockMode.ReadOnly, 
			                                   PixelFormat.Format32bppArgb);
			Marshal.Copy(bData.Scan0, pixels, 0, pixels.Length);
			bitmap.UnlockBits(bData);

			return new Image(fixTransparent(pixels), width, height);
		}
		
		private static int[] fixTransparent(int[] pixels)
		{
			for(int i = 0; i < pixels.Length; i++)
			{
				if(pixels[i] == 0)
				{
					pixels[i] = Image.EmptyPixel;
				}
			}
			return pixels;
		}
	}
}
