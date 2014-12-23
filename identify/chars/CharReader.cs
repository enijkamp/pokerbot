using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Configuration;

namespace PokerBot
{	
	public class CharReader
	{
		public static List<CharPattern> readCharsFromResourcesPlayer()
		{
			return readCharsFromResources("player");
		}
		
		public static List<CharPattern> readCharsFromResourcesBets()
		{
			return readCharsFromResources("bets");
		}

        public static List<CharPattern> readCharsFromResourcesControls()
        {
            return readCharsFromResources("controls");
        }

        public static List<CharPattern> readCharsFromResourcesPot()
        {
            return readCharsFromResources("pot");
        }

        public static List<CharPattern> readCharsFromResourcesLobby()
        {
            return readCharsFromResources("lobby");
        }		
		
		public static List<CharPattern> readCharsFromResources(string section)
		{
			// result
			List<CharPattern> patterns = new List<CharPattern>();

			// get a reference to the current assembly
            Assembly ass = Assembly.GetExecutingAssembly();

			// read from assembly
            Stream stream = getAssemblyStream(ass, "mapping.config");
			
			// config
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);

			// parse
			XmlNode parent = doc.GetElementsByTagName(section).Item(0); 
			foreach (XmlNode node in parent.ChildNodes)
			{
				// attributes
				XmlElement charElement = (XmlElement) node;
				char name = charElement.Attributes["key"].Value[0];
				string file = charElement.Attributes["image"].Value;
				
				// read from assembly
                Stream imgStream = getAssemblyStream(ass, file);
				Bitmap bitmap = Bitmap.FromStream(imgStream) as Bitmap;
				Image image = toImage(bitmap);
				imgStream.Close();
				
				// new pattern
				CharPattern pattern = new CharPattern(name, image);
				patterns.Add(pattern);
			}
			
			return patterns;
		}

        private static Stream getAssemblyStream(Assembly ass, string file)
        {
            string[] names = ass.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name.Contains(file))
                {
                    return ass.GetManifestResourceStream(name);
                }
            }
            throw new Exception("cannot find resource '"+file+"'");
        }

		private static Image toImage(Bitmap bitmap) {
			int width = bitmap.Size.Width;
			int height = bitmap.Size.Height;
			int[] pixels = new int[width * height];

			BitmapData bData = bitmap.LockBits(
			                                   new Rectangle (new Point(), bitmap.Size),
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
