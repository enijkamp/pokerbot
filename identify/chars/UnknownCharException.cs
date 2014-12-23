using System;

namespace PokerBot
{
	public class UnknownCharException : Exception
	{	
		public readonly Image image;
		
		public UnknownCharException(Image image) {
			this.image = image;
		}
	}
}
