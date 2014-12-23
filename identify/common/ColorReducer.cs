using System;
using System.Drawing;

namespace PokerBot
{
	public interface ColorReducer
	{
		Image reduceColors(Image image);
	}
}
