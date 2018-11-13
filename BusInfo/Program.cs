using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BusInfo
{
    
    static class Program
    {
        // GUI
        static NotifyIcon icon;
        static Timer timer;
        static ContextMenu menu;
        static MenuItem mNextBusItem;

        // Data
        static string stationName = "Technopark Dai-2";
        static Time startNotify = new Time(17, 30);
        static List<Time> busTT = new List<Time>()
        {
            new Time(9, 40),
            new Time(10, 40),
            new Time(11, 40),
            new Time(12, 40),
            new Time(13, 40),
            new Time(14, 40),
            new Time(15, 40),
            new Time(16, 40),
            new Time(17, 15),
            new Time(17, 40),
            new Time(17, 52),
            new Time(18, 0),
            new Time(18, 7),
            new Time(18, 15),
            new Time(18, 22),
            new Time(18, 30),
            new Time(18, 40),
            new Time(18, 52),
            new Time(19, 7),
            new Time(19, 27),
            new Time(19, 47),
            new Time(20, 10),
            new Time(20, 30),
            new Time(20, 52),
            new Time(21, 15),
            new Time(21, 40),
            new Time(22, 10)
        };
        static Time lastNotified = Time.Now();

        static Time GetNextBus()
        {
            DateTime now = DateTime.Now;
            Time nextBus = busTT.Where((t) => !t.Passed() && 
                                        (t.MinutesUntil() > BusInfo.Properties.Settings.Default.NotifyBefore))
                                 .FirstOrDefault();
            if (nextBus == null)
                nextBus = busTT[0];
            return nextBus;
        }

        static void ReloadSetting()
        {
            var Settings = BusInfo.Properties.Settings.Default;

            if (Settings.NotifyBefore <= 0)
            {
                Settings.NotifyBefore = 15;
            }

            if (Settings.Schedule == null ||
                Settings.Schedule.Count == 0)
            {
                Settings.Schedule = new System.Collections.Specialized.StringCollection();
                foreach (var time in busTT)
                {
                    Settings.Schedule.Add(time.ToString());
                }
            }
            else
            {
                busTT.Clear();
                foreach (var tString in Settings.Schedule)
                {
                    busTT.Add(Time.FromString(tString));
                }
            }

            if (Settings.StationName == null)
            {
                Settings.StationName = stationName;
            } else
            {
                stationName = Settings.StationName;
            }

            if (Settings.StartNotifyAt == null)
            {
                Settings.StartNotifyAt = startNotify.ToString();
            } else
            {
                startNotify = Time.FromString(Settings.StartNotifyAt);
            }
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ReloadSetting();

            mNextBusItem = new MenuItem();
            {
                mNextBusItem.Enabled = false;
                mNextBusItem.Text = Localize.Localized("Next bus is at ...:...");
            }

            menu = new ContextMenu();
            {
                menu.MenuItems.Add(mNextBusItem);
                menu.Popup += Menu_Popup;

                MenuItem separator = new MenuItem("-");
                menu.MenuItems.Add(separator);

                MenuItem setting = new MenuItem(Localize.Localized("Settings"));
                setting.Click += Setting_Click;
                menu.MenuItems.Add(setting);

                MenuItem exit = new MenuItem(Localize.Localized("Exit"));
                exit.Click += Exit_Click;
                menu.MenuItems.Add(exit);
            }
     
            icon = new NotifyIcon();
            {
                icon.Text = Localize.Localized("BUSINFO_ICON_NAME");
                icon.Icon = BusInfo.Properties.Resources.Icon1;
                icon.Visible = true;
                icon.ContextMenu = menu;
                icon.ShowBalloonTip(10, Localize.Localized("BusInfo"), Localize.Localized("BusInfo Startup"), ToolTipIcon.Info);
            }

            timer = new Timer();
            {
                timer.Interval = 1000;
                timer.Tick += Timer_Tick;
                timer.Enabled = true;
            }

          //   Application.EnableVisualStyles();
           //  Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            icon.ShowBalloonTip(10, Localize.Localized("BusInfo"), Localize.Localized("BusInfo Shutdown"), ToolTipIcon.Info);
            Application.Exit();
        }

        private static void Setting_Click(object sender, EventArgs e)
        {
            icon.Visible = false;
            frmSettings settings = new frmSettings();
            settings.ShowDialog();
            ReloadSetting();
            icon.Visible = true;
            MessageBox.Show(Localize.Localized("Settings Save OK"),
                            Localize.Localized("BusInfo"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);
        }

        private static void Menu_Popup(object sender, EventArgs e)
        {
            mNextBusItem.Text = String.Format(Localize.Localized("BUS_MENU_ITEM"), stationName, GetNextBus());
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            if (!startNotify.Passed())
                return;
            if(lastNotified.MinutesAfter() < 1)
                return;
            DateTime now = DateTime.Now;
            Time bus = busTT.Where(t => !t.Passed())
                             .Where(t => t.MinutesUntil() == BusInfo.Properties.Settings.Default.NotifyBefore)
                             .FirstOrDefault();
            if (bus != null) {
                icon.ShowBalloonTip(60, String.Format(Localize.Localized("NOTIFY_HEAD"), stationName),
                                    String.Format(Localize.Localized("NOTIFY_BODY"), bus), ToolTipIcon.Info);
                lastNotified = Time.Now();
            }
        }


    }
}
