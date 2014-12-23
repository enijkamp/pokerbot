using System;

namespace PokerBot
{	
	public class CharPattern : Image
	{
		private char character;
			
		public CharPattern(char character, Image image) : base(image)
		{
			this.character = character;
		}
		
		public char Character 
		{
			get { return character; }
		}
	}
}
