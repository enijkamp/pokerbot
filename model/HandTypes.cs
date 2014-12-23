using System;

namespace PokerBot
{
	public enum HandTypes
	{
        // preflop
        AK,
        AQ,
        QQ,
        Low_Pair,

		// postlfop
		AK_high_flush_draw,
		AK_overcards_ace_high,
		AA_KK_plus_draw,
		AK_high_flush, 

		KK_with_A_board,
		Low_flush,
		Full_house,		
		Mid_pair,
		
        Top_pair,
        Weak_top_pair,
		Two_pair,
		Just_board_paired,
		
        Set,
        Trips,
		Straight,	
        Open_ended,

        // both
        AA_KK,
		None 
	}

    public class HandTypesText
    {
        public static string[] Texts =
        {
            "Ace King",
            "Ace Queen",
            "Queens",
            "Low pair",

            "Ace King with Flush Draw",
            "Ace King did not hit",
            "Pocket rockets or big daddies with Flush Draw",
            "High flush",

            "Kings with Ace on board",
            "Low flush",
            "Titanic",
            "Mid or low pair",

            "Top pair with good kicker",
            "Weak top pair",
            "Two dizzle",
            "Only board paired",
            
            "Set",
            "Trips",
            "Straight",
            "Open ended draw",
            
            "Pocket rockets or big daddies",
            "Nothing"            
        };
    }
}
