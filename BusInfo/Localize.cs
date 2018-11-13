using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusInfo
{
    public static class Localize
    {
        public static Dictionary<string, string> Locale_JP = new Dictionary<string, string>()
        {
            {"BUSINFO_ICON_NAME", "バス時刻表" },
            {"Next bus is at ...:...", "次のバスは???時???分で発射します" },
            {"Settings", "設定" },
            {"Exit", "終了" },
            {"BusInfo Startup", "BusInfoが起動しました" },
            {"BusInfo Shutdown", "BusInfoが終了しました" },
            {"Settings Save OK", "設定が保存しました。" },
            {"BUS_MENU_ITEM", "{0}のバスが{1}で発射します" },
            {"NOTIFY_BODY", "{0}で発射します" },
            {"NOTIFY_HEAD", "{0}からバス" },
            {"Saved successfully", "ファイルを保存しました。"},
            {"Not a BusInfo file", "ファイルを読み込む失敗" },
            {"Schedule", "時刻表" },
            {"Save", "保存" },
            {"Station Name", "のりば名" },
            {"Notify", "通知を" },
            {"min. in advance", "分前に表示。" },
            {"starting at", "" },
            {"JA_START_AT", "から" },
            {"CHG_LOCALE_RESTART", "言語を翻訳するにはBusInfoを再起動してください。" },
            {"Timing", "通知" },
            {"Import", "インポート" },
            {"Export", "エクスポート" }
        };

        public static Dictionary<string, string> Locale_RU = new Dictionary<string, string>()
        {
             {"BUSINFO_ICON_NAME", "Расписание автобусов" },
             {"Next bus is at ...:...", "Следующий автобус отправляется в ???:???" },
             {"Settings", "Настройки" },
             {"Exit", "Завершить" },
             {"BusInfo Startup", "BusInfo запущен" },
             {"BusInfo Shutdown", "BusInfo остановлен" },
             {"Settings Save OK", "Настройки сохранены" },
             {"BUS_MENU_ITEM", "Следующий автобус от {0} отправляется в {1}" },
             {"NOTIFY_BODY", "Время отправления: {0}" },
             {"NOTIFY_HEAD", "Автобус от {0}" },
             {"Saved successfully", "Файл сохранён"},
             {"Not a BusInfo file", "Файл не является файлом BusInfo" },
             {"Schedule", "Расписание" },
             {"Save", "Сохранить" },
             {"Station Name", "Остановка" },
             {"Notify", "За" },
             {"min. in advance", "мин. предупредить" },
             {"starting at", "начиная с" },
             {"CHG_LOCALE_RESTART", "Для полной смены языка интерфейса требуется перезапуск" },
             {"Timing", "Уведомления" },
             {"Import", "Импорт из файла" },
            {"Export", "Выгрузка в файл" }
        };

        public static Dictionary<string, string> Locale_EN = new Dictionary<string, string>()
        {
            {"BUSINFO_ICON_NAME", "Bus Schedule" },
            {"BUS_MENU_ITEM", "Next bus from {0} will depart at {1}" },
            {"NOTIFY_BODY", "Departing at {0}" },
            {"NOTIFY_HEAD", "Bus from {0}" },
            {"JA_START_AT", " " },
            {"CHG_LOCALE_RESTART", "To change GUI language, please restart the application." }
        };

        public static Dictionary<string, Dictionary<string, string>> Locales = new Dictionary<string, Dictionary<string, string>>()
        {
            {"English", Locale_EN },
            {"日本語", Locale_JP },
            {"Русский", Locale_RU }
        };

        public static Dictionary<string, string> ActiveLocale()
        {
            var localeName = BusInfo.Properties.Settings.Default.Locale;
            if(Locales.ContainsKey(localeName))
            {
                return Locales[localeName];
            } else
            {
                return Locale_EN;
            }
        }
        public static string Localized(string what)
        {
            var active = ActiveLocale();
            if(active.ContainsKey(what))
            {
                return active[what];
            } else if (Locale_EN.ContainsKey(what))
            {
                return Locale_EN[what];
            }
            {
                return what;
            }
        }

        public static void LocalizeForm(Control form)
        {
            foreach (Control ctl in form.Controls)
            {
                if (!(ctl is TextBox || ctl is ListBox || ctl is MaskedTextBox))
                {
                    ctl.Text = Localized(ctl.Text);
                    if (ctl is GroupBox)
                    {
                        LocalizeForm(ctl);
                    }
                } 
            }
        }

        public static void LocalizeMenu(ContextMenuStrip menu)
        {
            foreach(ToolStripMenuItem item in menu.Items)
            {
                item.Text = Localized(item.Text);
            }
        }
        public static void LocalizeMenu(MenuStrip menu)
        {
            foreach (MenuItem item in menu.Items)
            {
                item.Text = Localized(item.Text);
            }
        }
    }
}
