using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using BuddyCompiler = Blapaz.Buddy.Compiler.Program;
using BuddyRuntime = Blapaz.Buddy.Runtime.Program;
using System.Diagnostics;

namespace App
{
    public partial class FormMain : Form
    {
        private NotifyIcon _notifyIcon;
        private Config _config;

        public FormMain()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _notifyIcon.Text = "Buddy";
            _notifyIcon.BalloonTipTitle = "Buddy";
            _notifyIcon.BalloonTipText = "Buddy is running in the background";
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Config", null, NotifyIconConfig_Click);
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
                _config = new Config(Path.Combine(Path.GetDirectoryName(scriptFile), "buddy.cfg"));

                if (File.Exists(scriptFile))
                {
                    if (Path.GetExtension(scriptFile).Equals(".bud"))
                    {
                        string compiledCode = BuddyCompiler.Compile(scriptFile);

                        if (_config.ShouldOutputCompiled)
                        {
                            using (FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(scriptFile) + ".buddy", FileMode.Create))
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                bw.Write(compiledCode);
                            }
                        }

                        if (!_config.ShouldCompileOnly)
                        {
                            BuddyRuntime.Run(compiledCode);
                        }
                    }
                    else if (Path.GetExtension(scriptFile).Equals(".buddy"))
                    {
                        BuddyRuntime.Run(File.ReadAllText(scriptFile));
                    }
                    else
                    {
                        MessageBox.Show("Only files with the *.bud or *.buddy extensions are executable by this app", "Invalid Executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Script file path specified is invalid", "Invalid File Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NotifyIconExit_Click(object sender, EventArgs e) => Application.Exit();
        private void NotifyIconConfig_Click(object sender, EventArgs e) => ModifyConfigFile();

        private void ModifyConfigFile()
        {
            if (!File.Exists(_config.ConfigFile))
            {
                File.Create(_config.ConfigFile);
            }

            MessageBox.Show("Be sure to restart application after making changes to config file", "Restart After Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Process.Start(_config.ConfigFile);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
            _notifyIcon.Visible = true;

            if (!_config.ShouldSilentStart)
            {
                _notifyIcon.ShowBalloonTip(2000);
            }
        }
    }
}
