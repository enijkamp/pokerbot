using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokerBot
{	
	public class TestLobbyIdentifier : TestBase
	{
        public static void Main(string[] args)
        {
            Thread.Sleep(10 * 1000);

            LobbyIdentifier identifier = new LobbyIdentifier();
            DeviceControl device = new Win32Control();
            Image screenshot = new ScreenImageIterator(device).next();
            Point offset = PatternLocator.locateLobby(screenshot);
            LobbyLayout layout = new LobbyLayout();          

            Image window = screenshot.crop(offset.X, screenshot.width, offset.Y, screenshot.height);
            Image tableList = window.crop(layout.TableList.X, layout.TableList.X + layout.TableList.Width,
                layout.TableList.Y, layout.TableList.Y + layout.TableList.Height);
            List<LobbyTable> lobbyTables = identifier.identifyLobbyTables(tableList, offset);
            foreach (LobbyTable lobby in lobbyTables)
            {
                Console.WriteLine(lobby.ToString());
            }
        }
	}
}
