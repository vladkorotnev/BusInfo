using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusInfo
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            var Settings = BusInfo.Properties.Settings.Default;
            Localize.LocalizeForm(this);
            Localize.LocalizeMenu(mnuIO);

            cbLanguage.Items.Clear();
            foreach (string localeName in Localize.Locales.Keys)
            {
                cbLanguage.Items.Add(localeName);
            }
            cbLanguage.SelectedItem = Settings.Locale;

            tbStation.Text = Settings.StationName;
            tbStartNotify.Text = Settings.StartNotifyAt.Replace(":","");
            nmNotifyBefore.Value = Settings.NotifyBefore;

            foreach(string time in Settings.Schedule)
            {
                lstSchedule.Items.Add(time);
            }
        }

        private void WriteSettings()
        {
            var Settings = BusInfo.Properties.Settings.Default;

            Settings.Schedule.Clear();
            foreach (string time in lstSchedule.Items)
            {
                Settings.Schedule.Add(time);
            }

            Settings.StationName = tbStation.Text;

            Time t = Time.FromString(tbStartNotify.Text);
            Settings.StartNotifyAt = t.ToString();

            Settings.NotifyBefore = Convert.ToInt32(nmNotifyBefore.Value);

            Settings.Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteSettings();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lstSchedule.SelectedIndex < 0 ||
                lstSchedule.SelectedIndex >= lstSchedule.Items.Count)
                return;

            lstSchedule.Items.RemoveAt(lstSchedule.SelectedIndex);
        }


        private void lstSchedule_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnDel.PerformClick();
            } 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Time t = Time.FromString(tbAdd.Text);
            lstSchedule.Items.Add(t.ToString());
            tbAdd.Text = "";
            tbAdd.Focus();
            tbAdd.SelectionStart = 0;
        }

        private void tbAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.PerformClick();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            {
                sfd.Filter = "BusInfo|*.bnf";
                sfd.FileName = tbStation.Text;
            }

            if (sfd.ShowDialog() != DialogResult.Cancel)
            {
                var outfile = File.CreateText(sfd.FileName);

                outfile.WriteLine("[BusInfo]");
                outfile.WriteLine("VER1");
                outfile.WriteLine("; Exporting station " + tbStation.Text);
                outfile.WriteLine("STA" + tbStation.Text);
                foreach (string time in lstSchedule.Items)
                {
                    outfile.WriteLine("BUS" + time);
                }
                outfile.WriteLine("EST" + tbStation.Text);
                outfile.WriteLine("; End of station " + tbStation.Text);
                outfile.WriteLine("ECF");
                outfile.WriteLine("; End of Config File");
                outfile.Close();

                MessageBox.Show(Localize.Localized("Saved successfully"),
                                "BusInfo Export",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void btnIO_Click(object sender, EventArgs e)
        {
            mnuIO.Show(btnIO, 0, 0);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            {
                ofd.Filter = "BusInfo|*.bnf";
            }

            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                var infile = File.OpenText(ofd.FileName);

                string input = infile.ReadLine();
                if (input != "[BusInfo]")
                {
                    MessageBox.Show(Localize.Localized("Not a BusInfo file"),
                                    "BusInfo Import",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                } else
                {
                    input = infile.ReadLine();
                    if (input.Substring(0,3) != "VER")
                    {
                        MessageBox.Show("Expected VER, found "+input,
                                    "BusInfo Import",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    } else
                    {
                        int version = Convert.ToInt32(input.Substring(3));
                        if (1 == version)
                        {
                            while(input.Substring(0,3) != "ECF")
                            {
                                if(infile.EndOfStream)
                                {
                                    MessageBox.Show("Unexpected EOF",
                                                     "BusInfo Import",
                                                      MessageBoxButtons.OK,
                                                         MessageBoxIcon.Error);
                                }
                                else
                                {
                                    input = infile.ReadLine();
                                    if (input.Length < 3 || (input.Length > 0 && input[0] == ';'))
                                        continue;
                                    string cmd = input.Substring(0, 3);
                                    string val = input.Substring(3);
                                    switch (cmd)
                                    {
                                        case "STA":
                                            tbStation.Text = val;
                                            lstSchedule.Items.Clear();
                                            break;

                                        case "BUS":
                                            Time t = Time.FromString(val);
                                            if (t == null)
                                            {
                                                MessageBox.Show("Bad time " + val,
                                                    "BusInfo Import",
                                                     MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                                            } else
                                            {
                                                lstSchedule.Items.Add(t.ToString());
                                            }
                                            break;

                                        case "EST":
                                            // ver 1 doesn't have multiple station support.
                                            break;

                                        case "ECF":
                                            // end config file
                                            WriteSettings();
                                            MessageBox.Show("Imported 1 station successfully",
                                                           "BusInfo Export",
                                                           MessageBoxButtons.OK,
                                                           MessageBoxIcon.Information);
                                            break;

                                        default:
                                            MessageBox.Show("Command unknown "+cmd,
                                                    "BusInfo Import",
                                                     MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                                            break;
                                    }
                                }
                            }
                        } else
                        {
                            MessageBox.Show("File version unknown",
                                    "BusInfo Import",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                        }
                    }
                }

                infile.Close();
            }
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLanguage.SelectedItem.Equals(BusInfo.Properties.Settings.Default.Locale))
                return;

            BusInfo.Properties.Settings.Default.Locale = cbLanguage.SelectedItem.ToString();

           MessageBox.Show(Localize.Localized("CHG_LOCALE_RESTART"),
                                "BusInfo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }
    }
}
