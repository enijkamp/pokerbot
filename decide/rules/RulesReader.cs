using System;
using System.Collections.Generic;
using ExcelLibrary.SpreadSheet;

namespace PokerBot
{	
	public class RulesReader
	{
		private const string RULES_XLS = "rules.xls";
		
		public static List<Rule> readRules()
		{
            Log.Info("Reading spreadsheet '" + RULES_XLS + "'");
			Workbook workbook = Workbook.Load(RULES_XLS);
			Worksheet worksheet = workbook.Worksheets[0];
			List<Rule> rules = new List<Rule>();
			int row = 1;
			while(true)
			{
				// check
				if(isEmptyRow(worksheet, row)) break;
                Log.DebugIf(row % 100 == 0, "Reading row '" + (row + 1) + "'");
				
				// columns
				string hand     = worksheet.Cells[row,  2].StringValue;
				string street   = worksheet.Cells[row,  3].StringValue;
				string pot      = worksheet.Cells[row,  4].StringValue;
				string board    = worksheet.Cells[row,  5].StringValue;
				string opps     = worksheet.Cells[row,  6].StringValue;
				string action   = worksheet.Cells[row,  7].StringValue;
                string position = worksheet.Cells[row,  8].StringValue;
                string maxbet   = worksheet.Cells[row,  9].StringValue;
                string potsize  = worksheet.Cells[row, 10].StringValue;
				string decision = worksheet.Cells[row, 11].StringValue;
				
				// map				
				StreetTypes streetType = mapStreetType(street);
                HandTypes handType = mapHandType(hand);
				ChanceTypes chanceType = mapChanceType(board);
				int minOpponents = mapMinOpponents(opps);
				int maxOpponents = mapMaxOpponents(opps);
				OpponentActionTypes actionType = mapActionType(action);
                PositionTypes positionType = mapPositionType(position);
                double minMaxBet = mapMinInterval(maxbet);
                double maxMaxBet = mapMaxInterval(maxbet);
                double minPotSize = mapMinInterval(potsize);
                double maxPotSize = mapMaxInterval(potsize);

				// rule
				Rule rule = new Rule(streetType, handType, chanceType, minOpponents,
				                     maxOpponents, actionType, positionType, minMaxBet, maxMaxBet,
                                     minPotSize, maxPotSize, decision);
				rules.Add(rule);
				
				// next
				row++;
			}
			Log.Info("Done reading spreadsheet");
			return rules;
		}

        private static double mapMinInterval(string value)
        {
            string values = value.Replace("[", "").Replace("]", "");
            string[] numbers = values.Split(',');
            return TextTools.ParseDouble(numbers[0]);
        }

        private static double mapMaxInterval(string value)
        {
            string values = value.Replace("[", "").Replace("]", "");
            string[] numbers = values.Split(',');
            return TextTools.ParseDouble(numbers[1]);
        }
		
		private static bool isEmptyRow(Worksheet sheet, int row)
		{
			for(int i = 0; i < 5; i++)
			{
				if(!sheet.Cells[row, i].IsEmpty) return false;
			}
			return true;
		}

        private static PositionTypes mapPositionType(string position)
        {
            position = position.Trim().ToLower();
            if (position == "all") return PositionTypes.All;
            if (position == "early") return PositionTypes.Early;
            if (position == "late") return PositionTypes.Late;

            throw new ArgumentException("Unknown position pattern '" + position + "'");
        }
		
		private static OpponentActionTypes mapActionType(string action)
		{
			action = action.Trim().ToLower();
			if(action == "raise") return OpponentActionTypes.Raise;
			if(action == "bet") return OpponentActionTypes.Bet;
			if(action == "check") return OpponentActionTypes.Check;
			if(action == "first to act") return OpponentActionTypes.First_to_act;
			if(action == "limp") return OpponentActionTypes.Limp;
			if(action == "reraise") return OpponentActionTypes.Reraise;
			
			throw new ArgumentException("Unknown action pattern '"+action+"'");
		}

		private static int mapMinOpponents(string opps)
		{
			return int.Parse(opps.ToCharArray()[1].ToString());
		}
		
		private static int mapMaxOpponents(string opps)
		{
			return int.Parse(opps.ToCharArray()[3].ToString());
		}
		
		private static ChanceTypes mapChanceType(string chance)
		{
			chance = chance.Trim().ToLower();
			if(chance == "gothit") return ChanceTypes.GotHit;
			if(chance == "safe") return ChanceTypes.Safe;
			if(chance == "draws") return ChanceTypes.Draws;
			if(chance == "extreme") return ChanceTypes.Extreme;
            if(chance == "none") return ChanceTypes.None;
			
			throw new ArgumentException("Unknown board pattern '"+chance+"'");
		}
		
		private static StreetTypes mapStreetType(string street)
		{
			street = street.Trim().ToLower();
			if(street == "preflop") return StreetTypes.Preflop;
			if(street == "flop") return StreetTypes.Flop;
			if(street == "river") return StreetTypes.River;
			if(street == "turn") return StreetTypes.Turn;
			
			throw new ArgumentException("Unknown street pattern '"+street+"'");
		}
		
		private static HandTypes mapHandType(string hand)
		{
			hand = hand.Trim();
            if(hand == "AK") return HandTypes.AK;
            if(hand == "AQ") return HandTypes.AQ;
            if (hand == "QQ") return HandTypes.QQ;
            if(hand == "Low pair") return HandTypes.Low_Pair;
			if(hand == "AA/KK+Draw") return HandTypes.AA_KK_plus_draw;
			if(hand == "AA/KK (Overpair)") return HandTypes.AA_KK;
			if(hand == "AK overcards") return HandTypes.AK_overcards_ace_high;
			if(hand == "HFlush+") return HandTypes.AK_high_flush;
			if(hand == "High Flushdraw") return HandTypes.AK_high_flush_draw;			
			if(hand == "KKwA") return HandTypes.KK_with_A_board;
			if(hand == "LFlush") return HandTypes.Low_flush;			
			if(hand == "Midpair") return HandTypes.Mid_pair;
			if(hand == "Open-ended") return HandTypes.Open_ended;
			if(hand == "Strong-Top Pair") return HandTypes.Top_pair;			
			if(hand == "Set") return HandTypes.Set;
			if(hand == "Straight") return HandTypes.Straight;
			if(hand == "Two pair") return HandTypes.Two_pair;
            if(hand == "None") return HandTypes.None;
            if(hand == "Weak-Top Pair") return HandTypes.Weak_top_pair;
            if(hand == "Trips") return HandTypes.Trips;
			
			throw new ArgumentException("Unknown hand pattern '"+hand+"'");
		}
	}
}
