using CloudyApi;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Blacklow_Executor_by_Blacklow_Technologies
{
    public partial class Form1 : Form
    {
        private bool autoInjectEnabled = false;
        private Timer autoInjectTimer;

        public Form1()
        {
            InitializeComponent();

            autoInjectTimer = new Timer();
            autoInjectTimer.Interval = 2000;
            autoInjectTimer.Tick += AutoInjectTimer_Tick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length == 0)
            {
                MessageBox.Show("Roblox is not running. Please open Roblox before injecting.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CloudyApi.Api.Internal.isInjected)
            {
                MessageBox.Show("Cloudy API is already injected!");
            }
            else
            {
                CloudyApi.Api.Internal.inject();
                MessageBox.Show("Injection successful!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CloudyApi.Api.Internal.execute(richTextBox1.Text);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

                if (robloxProcesses.Length == 0)
                {
                    MessageBox.Show("Roblox is not running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    foreach (var process in robloxProcesses)
                    {
                        process.Kill();
                    }
                    MessageBox.Show("Roblox has been successfully closed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while closing Roblox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            autoInjectEnabled = !autoInjectEnabled;

            if (autoInjectEnabled)
            {
                autoInjectTimer.Start();
                MessageBox.Show("Auto Inject is ON");
            }
            else
            {
                autoInjectTimer.Stop();
                MessageBox.Show("Auto Inject is OFF");
            }
        }

        private void AutoInjectTimer_Tick(object sender, EventArgs e)
        {
            if (autoInjectEnabled && !CloudyApi.Api.Internal.isInjected)
            {
                var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

                if (robloxProcesses.Length > 0)
                {
                    CloudyApi.Api.Internal.inject();
                    MessageBox.Show("Auto Inject: Injection successful!");
                    autoInjectEnabled = false;
                    autoInjectTimer.Stop();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
