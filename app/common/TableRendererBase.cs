using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokerBot
{
    public class TableRendererBase
    {
        private static string Format(double dbl)
        {
            if (dbl == -1)
            {
                return "        ";
            }
            else
            {
                return string.Format("{0,5:0.00}", dbl);
            }
        }

        public static void DrawTable(Graphics g, List<RenderImage> images, Table table, TableLayout layout, Situation situation, Rule rule, Decision decision, List<TableControl> controls)
        {
            // images
            foreach (RenderImage image in images)
            {
                g.DrawImage(image.Image, image.Position.X, image.Position.Y);
            }

            // table
            int Y_OFFSET = 541;

            // players
            int X_PLAYERS = 0, Y_PLAYERS = Y_OFFSET + 5;
            g.DrawString("Players", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_PLAYERS, Y_PLAYERS);
            foreach (Player player in table.Players)
            {
                String playerText = "";
                if (!player.IsExistent)
                {
                    playerText = player.Position + 1 + ": " + Format(player.Money) + " " + Format(player.Bet) + " Empty";
                }
                else
                {
                    playerText = player.Position + 1 + ": " + Format(player.Money) + " " + Format(player.Bet) + " Seated";
                }
                g.DrawString(playerText, new Font("Verdana", 8), new SolidBrush(Color.Black), X_PLAYERS, Y_PLAYERS + 15 + (player.Position * 10));
            }

            // actions
            int X_ACTION = X_PLAYERS + 160, Y_ACTION = Y_OFFSET + 5;
            g.DrawString("Actions", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_ACTION, Y_ACTION);
            int actionNr = 0;
            foreach (Player player in table.Players)
            {
                String actionText = actionNr + 1 + ": " + player.Action;
                g.DrawString(actionText, new Font("Verdana", 8), new SolidBrush(Color.Black), X_ACTION, Y_ACTION + 15 + (actionNr * 10));
                actionNr++;
            }

            // cards
            int X_CARDS = X_ACTION + 95, Y_CARDS = Y_OFFSET + 5;
            g.DrawString("Cards", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_CARDS, Y_CARDS);
            int cardNr = 0;
            foreach (Card card in table.Community)
            {
                String cardText = cardNr + 1 + ": " + card;
                g.DrawString(cardText, new Font("Verdana", 8), new SolidBrush(Color.Black), X_CARDS, Y_CARDS + 15 + (cardNr * 10));
                cardNr++;
            }

            // players with money
            int X_MONEY = X_CARDS, Y_MONEY = Y_OFFSET + 75;
            g.DrawString("Players #", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_MONEY, Y_MONEY);
            g.DrawString(table.PlayersWithMoney.ToString(), new Font("Verdana", 8), new SolidBrush(Color.Black), X_MONEY, Y_MONEY + 15);

            // my money
            int X_MYMONEY = X_CARDS, Y_MYMONEY = Y_MONEY + 30;
            g.DrawString("My Money", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_MYMONEY, Y_MYMONEY);
            g.DrawString(table.HasSeat ? table.MyPlayer.Money.ToString() + "$" : "", new Font("Verdana", 8), new SolidBrush(Color.Black), X_MYMONEY, Y_MYMONEY + 15);
            
            // hand
            int X_HAND = X_CARDS + 90, Y_HAND = Y_OFFSET + 5;
            g.DrawString("Hand", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_HAND, Y_HAND);
            int handNr = 0;
            foreach (Card card in table.Hand)
            {
                String cardText = handNr + 1 + ": " + card;
                g.DrawString(cardText, new Font("Verdana", 8), new SolidBrush(Color.Black), X_HAND, Y_HAND + 15 + (handNr * 10));
                handNr++;
            }

            // seat
            int X_SEAT = X_HAND, Y_SEAT = Y_OFFSET + 45;
            g.DrawString("Seat", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_SEAT, Y_SEAT);
            g.DrawString((table.Seat+1).ToString(), new Font("Verdana", 8), new SolidBrush(Color.Black), X_SEAT, Y_SEAT + 15);
           
            // blinds
            int X_BLINDS = X_HAND, Y_BLINDS = Y_OFFSET + 75;
            g.DrawString("Blind", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_BLINDS, Y_BLINDS);
            g.DrawString("SB = " + (table.SmallBlindSeat+1), new Font("Verdana", 8), new SolidBrush(Color.Black), X_BLINDS, Y_BLINDS + 15);
            g.DrawString("BB = " + (table.BigBlindSeat+1), new Font("Verdana", 8), new SolidBrush(Color.Black), X_BLINDS, Y_BLINDS + 30);
            g.DrawString("My BB = " + (table.IsMyBigBlind ? "Yes" : "No"), new Font("Verdana", 8), new SolidBrush(Color.Black), X_BLINDS, Y_BLINDS + 45);

            // dealer
            int X_DEALER = X_BLINDS + 100, Y_DEALER = Y_OFFSET + 5;
            g.DrawString("Dealer", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_DEALER, Y_DEALER);
            if (table.Dealer != Table.NO_DEALER)
            {
                g.DrawString((table.Dealer + 1).ToString(), new Font("Verdana", 8), new SolidBrush(Color.Black), X_DEALER, Y_DEALER + 15);
            }

            // round
            int X_ROUND = X_DEALER, Y_ROUND = Y_DEALER + 30;
            g.DrawString("Round", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_ROUND, Y_ROUND);
            g.DrawString((table.IsFirstRound ? "1-st" : "n-th"), new Font("Verdana", 8), new SolidBrush(Color.Black), X_ROUND, Y_ROUND + 15);

            // pot
            int X_POT = X_DEALER, Y_POT = Y_ROUND + 30;
            g.DrawString("Pot", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_POT, Y_POT);
            g.DrawString(table.Pot.ToString() + "$", new Font("Verdana", 8), new SolidBrush(Color.Black), X_POT, Y_POT + 15);

            // max bet
            int X_BET = X_DEALER, Y_BET = Y_POT + 30;
            g.DrawString("Max Bet", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_BET, Y_BET);
            g.DrawString(table.MaxBet.ToString() + "$", new Font("Verdana", 8), new SolidBrush(Color.Black), X_BET, Y_BET + 15);

            // situation
            int X_SIT = X_BET + 100, Y_SIT = Y_OFFSET + 5;
            g.DrawString("Situation", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_SIT, Y_SIT);
            if (situation != null)
            {
                g.DrawString("street: " + situation.Street, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 15);
                g.DrawString("hand: " + situation.Hand, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 30);
                g.DrawString("opps: " + situation.Opponents, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 45);
                g.DrawString("action: " + situation.OpponentAction, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 60);
                g.DrawString("chance: " + situation.Chance, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 75);
                g.DrawString("position: " + situation.Position, new Font("Verdana", 8), new SolidBrush(Color.Black), X_SIT, Y_SIT + 90);
            }

            // rule
            int X_RULE = X_SIT + 130, Y_RULE = Y_OFFSET + 5;
            g.DrawString("Rule", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_RULE, Y_RULE);
            if (rule != null)
            {
                g.DrawString(rule.Decision, new Font("Verdana", 8), new SolidBrush(Color.Black), X_RULE, Y_RULE + 15);
            }

            // decision
            int X_DECISION = X_RULE, Y_DECISION = Y_RULE + 30;
            g.DrawString("Decision", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_DECISION, Y_DECISION);
            if (decision != null)
            {
                g.DrawString(decision.ToString(), new Font("Verdana", 8), new SolidBrush(Color.Black), X_DECISION, Y_DECISION + 15);
            }

            // controls
            int X_CONTROLS = X_DECISION, Y_CONTROLS = Y_DECISION + 30;
            g.DrawString("Controls", new Font("Verdana", 10), new SolidBrush(Color.Blue), X_CONTROLS, Y_CONTROLS);
            int yControls = Y_CONTROLS + 15;
            foreach(TableControl control in controls)
            {
                g.DrawString(control.ToString(), new Font("Verdana", 8), new SolidBrush(Color.Black), X_CONTROLS, yControls);
                yControls += 15;
            }

            // markers
            if (layout != null)
            {
                g.DrawEllipse(Pens.Blue, layout.SliderText.X, layout.SliderText.Y, 4, 4);
                g.DrawEllipse(Pens.Blue, layout.SliderClick.X, layout.SliderClick.Y, 4, 4);
            }
        }
    }
}
