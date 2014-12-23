using System;

namespace PokerBot
{
	public class HandAnalysis
	{
		private readonly Hand.HandTypes basic = Hand.HandTypes.None;
		private readonly HandTypes smart = HandTypes.None;
		private readonly ChanceTypes chance = ChanceTypes.None;

        public HandAnalysis(HandTypes smart)
		{
			this.smart = smart;
		}
        
		public HandAnalysis(Hand.HandTypes basic, HandTypes smart, ChanceTypes chance)
		{
			this.basic = basic;
			this.smart = smart;
			this.chance = chance;
		}
		
		public Hand.HandTypes HandBasic
		{
			get { return basic; }
		}
		
		public HandTypes HandSmart
		{
			get { return smart; }
		}
		
		public ChanceTypes Chance
		{
			get { return chance; }
		}
	}
}
