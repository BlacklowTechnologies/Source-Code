using CloudyApi;
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Blacklow_Executor_by_Blacklow_Technologies
{
    public partial class Form1 : Form
    {
        private bool autoInjectEnabled = false;
        private Timer autoInjectTimer;
        private string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.txt");

        public Form1()
        {
            InitializeComponent();

            autoInjectTimer = new Timer();
            autoInjectTimer.Interval = 2000;
            autoInjectTimer.Tick += AutoInjectTimer_Tick;

            CheckIfRunAsAdmin();
            CheckAndUpdateSettings();
        }

        private void CheckIfRunAsAdmin()
        {
            bool isAdmin;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!isAdmin)
            {
                MessageBox.Show("Please restart the executor as administrator for proper functionality.", "Administrator Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CheckAndUpdateSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var settings = File.ReadAllLines(settingsFilePath);
                bool autoInjectFound = false;
                bool discordLinkFound = false;

                for (int i = 0; i < settings.Length; i++)
                {
                    if (settings[i].StartsWith("AutoInject"))
                    {
                        settings[i] = $"AutoInject = {autoInjectEnabled}";
                        autoInjectFound = true;
                    }

                    if (settings[i].StartsWith("DiscordLink"))
                    {
                        settings[i] = "DiscordLink = true";
                        discordLinkFound = true;
                    }
                }

                if (!autoInjectFound)
                {
                    Array.Resize(ref settings, settings.Length + 1);
                    settings[settings.Length - 1] = $"AutoInject = {autoInjectEnabled}";
                }

                if (!discordLinkFound)
                {
                    Array.Resize(ref settings, settings.Length + 1);
                    settings[settings.Length - 1] = "DiscordLink = true"; 
                }

                File.WriteAllLines(settingsFilePath, settings);
            }
            else
            {
                string[] settings = { "AutoInject = false", "DiscordLink = true" };
                File.WriteAllLines(settingsFilePath, settings);
            }
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
                Api.External.RegisterExecutor("Blacklow Executor");
                CloudyApi.Api.External.inject();
                MessageBox.Show("Injection successful!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length == 0)
            {
                MessageBox.Show("Roblox is not running. Please open Roblox before executing the script.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CloudyApi.Api.External.execute(richTextBox1.Text);
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
                button5.Text = "AutoInject: ON";
                button5.BackColor = Color.LightGreen;
                MessageBox.Show("Auto Inject is ON");
            }
            else
            {
                autoInjectTimer.Stop();
                button5.Text = "AutoInject: OFF";
                button5.BackColor = Color.LightCoral;
                MessageBox.Show("Auto Inject is OFF");
            }

            // Mise à jour du fichier settings après changement
            CheckAndUpdateSettings();
        }

        private void AutoInjectTimer_Tick(object sender, EventArgs e)
        {
            if (!autoInjectEnabled)
                return;
            if (CloudyApi.Api.Internal.isInjected)
            {
                autoInjectTimer.Stop();
                autoInjectEnabled = false;
                MessageBox.Show("Auto Inject: Already injected.");
                return;
            }

            var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length > 0)
            {
                CloudyApi.Api.Internal.inject();
                MessageBox.Show("Auto Inject: Injection successful!");
                autoInjectEnabled = false;
                autoInjectTimer.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OuvrirInvitationDiscord();
        }

        private void OuvrirInvitationDiscord()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/DktHZxkN", // Remplacer par le vrai lien d'invitation Discord
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:Bruh");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to disable security?\n\nDisabling security allows the executor to work with all Roblox versions, but it may cause instability or crash your game.",
                "Disable Security - Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Api.misc.disableSecurity();
                MessageBox.Show("Security has been disabled.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Security was not disabled.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
