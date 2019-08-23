using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using BuddyCompilerLibrary = Blapaz.Buddy.Compiler.Library.Program;
using BuddyRuntimeLibrary = Blapaz.Buddy.Runtime.Library.Program;
using System.Diagnostics;

namespace Blapaz.Buddy.Bundle
{
    public partial class FormMain : Form
    {
        private NotifyIcon _notifyIcon;

        private bool _shouldSilentStart = false;
        private bool _shouldOutputCompiled = false;
        

        public FormMain()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _notifyIcon.Text = "Buddy";
            _notifyIcon.BalloonTipTitle = "Buddy";
            _notifyIcon.BalloonTipText = "Buddy is running in the background";
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

                foreach (string arg in args)
                {
                    if (arg.Equals("-s"))
                    {
                        _shouldSilentStart = true;
                    }
                    else if (arg.Equals("-o"))
                    {
                        _shouldOutputCompiled = true;
                    }
                }

                if (File.Exists(scriptFile))
                {
                    if (Path.GetExtension(scriptFile).Equals(".bud"))
                    {
                        string compiledCode = BuddyCompilerLibrary.Compile(scriptFile);

                        if (_shouldOutputCompiled)
                        {
                            using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(scriptFile), Path.GetFileNameWithoutExtension(scriptFile) + ".buddy"), FileMode.Create))
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                bw.Write(compiledCode);
                            }
                        }

                        BuddyRuntimeLibrary.Run(compiledCode);
                    }
                    else if (Path.GetExtension(scriptFile).Equals(".buddy"))
                    {
                        BuddyRuntimeLibrary.Run(File.ReadAllText(scriptFile));
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
            _notifyIcon.Visible = true;

            if (!_shouldSilentStart)
            {
                _notifyIcon.ShowBalloonTip(2000);
            }
        }
    }
}
