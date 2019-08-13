using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using BuddyCompiler = Blapaz.Buddy.Compiler.Program;
using BuddyRuntime = Blapaz.Buddy.Runtime.Program;

namespace App
{
    public partial class FormMain : Form
    {
        private NotifyIcon notifyIcon;

        public FormMain()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Text = "Buddy";
            notifyIcon.BalloonTipTitle = "Buddy";
            notifyIcon.BalloonTipText = "Buddy is running in the background";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, this.NotifyIconExit_Click); 
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
                        string compiledCode = BuddyCompiler.Compile(scriptFile);

                        if (Config.ShouldOutputCompiled)
                        {
                            using (FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(scriptFile) + ".bud", FileMode.Create))
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                bw.Write(compiledCode);
                            }
                        }

                        if (!Config.ShouldCompileOnly)
                        {
                            BuddyRuntime.Run(compiledCode);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Only files with the *.buddy extension are executable by this app", "Invalid Executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Script file path specified is invalid", "Invalid File Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void NotifyIconExit_Click(object sender, EventArgs e) => Application.Exit();

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000);
        }
    }
}
