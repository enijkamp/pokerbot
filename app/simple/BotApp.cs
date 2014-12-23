using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace PokerBot.App.Simple
{
	public class BotApp
	{
        enum InputMode { MOCK, WIN32, VM };
                		
		// members
        private static TableRenderer renderer = TableRendererForm.newTableRendererForm();
        private static Random random = new Random();
        private static Settings settings = new Settings();
        private static InputMode INPUT_MODE = InputMode.MOCK;

		public static void Main(string[] args)
		{
            Log.SetLevel(Log.Level.FINE);
            BotAppLogic.Beep(settings);

            Point offset;
            Iterator<Image> screen;
            DeviceControl deviceControl;

            if (INPUT_MODE == InputMode.WIN32)
            {
                Console.WriteLine("## win32 mode ##");
                deviceControl = new Win32Control();

			    // wait
			    Log.Info("waiting ...");
    			Thread.Sleep(10000);
    			
    			// full screen
    			Console.WriteLine("## scanning for table ##");
                Image fullScreen = new ScreenImageIterator(deviceControl).next();
    			offset = PatternLocator.locateTable(fullScreen);
                Console.WriteLine("table found at x=" + offset.X + " y=" + offset.Y);

    			// desk
                screen = new ScreenImageIterator(deviceControl, new Rectangle(offset.X, offset.Y, new TableLayout9().Size.Width, new TableLayout9().Size.Height));
    			screen = new WaitDeltaImageIterator(screen);
            }
            else if (INPUT_MODE == InputMode.VM)
            {
                Console.WriteLine("## vm mode ##");

                // boot
                deviceControl = new VirtualBoxControl(BotAppLogic.ReadConfig()["vm"]);

                // poker
                Console.WriteLine("waiting for pokerstars ...");
                Console.ReadKey();

                // mouse
                deviceControl.ResetMouse();

                // full screen
                Console.WriteLine("## scanning for table ##");
                Image fullScreen = new ScreenImageIterator(deviceControl).next();
                offset = PatternLocator.locateTable(fullScreen);
                Console.WriteLine("table found at x=" + offset.X + " y=" + offset.Y);

                // desk
                screen = new ScreenImageIterator(deviceControl, new Rectangle(offset.X, offset.Y, new TableLayout9().Size.Width, new TableLayout9().Size.Height));
                screen = new WaitDeltaImageIterator(screen);
            }
            else
            {
                Console.WriteLine("## mock mode ##");
                screen = new MockWaitImageIterator(1000, ImageTools.toImage(new Bitmap("test/table_check_free.png")));
                offset = new Point(renderer.Control.Location.X + 5, renderer.Control.Location.Y + 15);
                deviceControl = new Win32Control();
            }

			// identifier
            TableLayout layout = new TableLayout9(offset);
            TableIdentifier tableIdentifier = new TableIdentifier(layout);
            tableIdentifier.Renderer = renderer;


            // evaluator
            List<Rule> rules = RulesReader.readRules();
            RuleEvaluator evaluator = new RuleEvaluator(rules);
            RuleInterpreter interpreter = new RuleInterpreter(settings.SmallBlind, settings.BigBlind);

            // controller
            double betSlideTextLimit = settings.PlayMoney ? 100 : 0.2;
            Mouse mouse = new HumanMouse(deviceControl);
            Keyboard keyboard = new Keyboard(deviceControl);
            Controller controller = new Controller(keyboard, mouse, betSlideTextLimit, tableIdentifier, new ScreenImageIterator(deviceControl));

            // replayer
            Replayer replayer = new Replayer(deviceControl);

            // auto-click
            RandomClicker clicker = new RandomClicker(new Point(deviceControl.DisplayWidth, 0), mouse);

			// initial table
			Console.WriteLine("## initial table scan ##");
            string player = BotAppLogic.ReadConfig()["name"];
            Console.WriteLine("looking for '"+player+"'");
            int seat = -1;
            /*while (seat == -1)
            {
                Table previousTable = tableIdentifier.identifyTable(screen.next(), TableIdentifier.PlayerInfoEnum.BOTH);
                seat = BotAppLogic.IdentifySeat(previousTable, player);
                Thread.Sleep(1000);
            }
            Console.WriteLine("my seat = " + (seat+1)); */
            seat = 4;
			
			// loop
			while(screen.hasNext())
			{
				// start
				Console.WriteLine("## iteration -> start ##");
				DateTime start = DateTime.Now;
				
				// table				
				Console.WriteLine("# next table image");
				Image tableImage = screen.next();

                // render table
                renderer.clearImages();
				renderer.renderImage(tableImage, 0, 0);				
				
				// identify table
                //try 
                //{
                    if (tableIdentifier.identifyMove(tableImage))
                    {
                        Table table = tableIdentifier.identifyTable(tableImage, seat);
                        List<TableControl> controls = tableIdentifier.identifyControls(tableImage);
                        table.MaxBet = BotAppLogic.GetMaxBet(controls);
                        TableContainer container = new TableContainer(0, null, null, layout, seat, false, null);
                        new BotAppLogic(deviceControl).ProcessTable(settings, tableImage, renderer, table, container, evaluator, interpreter, controller, replayer, clicker, controls);
                    }
                    else
                    {
                        BotAppLogic.Sleep(settings, 1000);
                    }
                /*}
                catch(Exception ex)
                {
                    Console.WriteLine("Unable to identify table");
                    Console.WriteLine(ex.ToString());
                    Thread.Sleep(5000);
                    continue;
                }*/

                // end
                double time = DateTime.Now.Subtract(start).TotalMilliseconds;
                Console.WriteLine("## iteration -> end -> " + time + " ms ##");
			}
		}
	}
}
