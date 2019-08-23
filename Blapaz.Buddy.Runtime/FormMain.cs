using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using BuddyRuntimeLibrary = Blapaz.Buddy.Runtime.Library.Program;

namespace Blapaz.Buddy.Runtime
{
    public partial class FormMain : Form
    {
        private NotifyIcon _notifyIcon;

        public FormMain()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _notifyIcon.Text = "Buddy";
            _notifyIcon.BalloonTipTitle = "Buddy";
            _notifyIcon.BalloonTipText = "Buddy script is running in the background";
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, NotifyIconExit_Click);

            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                MessageBox.Show("Script file not found", "No Script File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string exePath = args[0];
                string scriptFile = args[1];

                if (File.Exists(scriptFile))
                {
                    if (Path.GetExtension(scriptFile).Equals(".buddy"))
                    {
                        BuddyRuntimeLibrary.Run(File.ReadAllText(scriptFile));
                    }
                    else
                    {
                        MessageBox.Show("Only files with the *.buddy extensions are executable by this app", "Invalid Executable", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                else
                {
                    MessageBox.Show("Script file path specified is invalid", "Invalid File Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NotifyIconExit_Click(object sender, EventArgs e) => Application.Exit();

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(2000);
        }
    }
}
