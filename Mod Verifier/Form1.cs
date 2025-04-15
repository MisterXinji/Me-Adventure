using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32; // Needed for registry access
using System.Net;

namespace Mod_Verifier
{
    public partial class Form1 : Form
    {
        //Mouse move around the Form
        private bool _mouseDown;
        private Point _lastMousePos;

        //Fixed
        public Form1()
        {
            InitializeComponent();
            // Attach the same mouse events to panel1
            panel1.MouseDown += Form1_MouseDown;
            panel1.MouseMove += Form1_MouseMove;
            panel1.MouseUp += Form1_MouseUp;
            // Attach the same mouse events to panel1
            panel2.MouseDown += Form1_MouseDown;
            panel2.MouseMove += Form1_MouseMove;
            panel2.MouseUp += Form1_MouseUp;
            // Attach the SAME events to LABEL (optional)
            label1.MouseDown += Form1_MouseDown;
            label1.MouseMove += Form1_MouseMove;
            label1.MouseUp += Form1_MouseUp;
            // Attach the SAME events to LABEL (optional)
            label4.MouseDown += Form1_MouseDown;
            label4.MouseMove += Form1_MouseMove;
            label4.MouseUp += Form1_MouseUp;
            // Attach the SAME events to LABEL (optional)
            pictureBox1.MouseDown += Form1_MouseDown;
            pictureBox1.MouseMove += Form1_MouseMove;
            pictureBox1.MouseUp += Form1_MouseUp;
            // Attach the SAME events to LABEL (optional)
            pictureBox4.MouseDown += Form1_MouseDown;
            pictureBox4.MouseMove += Form1_MouseMove;
            pictureBox4.MouseUp += Form1_MouseUp;
            // Attach the SAME events to LABEL (optional)
            pictureBox3.MouseDown += Form1_MouseDown;
            pictureBox3.MouseMove += Form1_MouseMove;
            pictureBox3.MouseUp += Form1_MouseUp;

        }
        //Fix
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _lastMousePos = Cursor.Position; // Store cursor position (screen coords)
        }
        //Fix
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                // Calculate movement delta (cursor change since MouseDown)
                int deltaX = Cursor.Position.X - _lastMousePos.X;
                int deltaY = Cursor.Position.Y - _lastMousePos.Y;

                // Move form by the delta
                this.Location = new Point(
                    this.Location.X + deltaX,
                    this.Location.Y + deltaY
                );

                // Update stored cursor position
                _lastMousePos = Cursor.Position;
            }
        }
        //Fix
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }
        //Fix
        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //Fix
        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        //Fix
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //Fixed opening website
        private void button5_Click(object sender, EventArgs e)
        {
            // Open URL in default browser
            System.Diagnostics.Process.Start("https://www.nexusmods.com/monsterhunterwilds/mods/93");
        }
        //Fixed opening website
        private void button6_Click(object sender, EventArgs e)
        {
            // Open URL in default browser
            System.Diagnostics.Process.Start("https://www.nexusmods.com/site/mods/818");
        }
        //Fixed for Running ModPakfilesVerifier.ahk
        private void button3_Click(object sender, EventArgs e)
        {
            //Process.Start("C:\\Users\\rhyan\\Downloads\\Builk Pak File\\mod verifier\\App Mod Verifier\\Mod Verifier\\Mod Verifier\\ModPakfilesVerifier.ahk");
            RunAhkFile("ModPakfilesVerifier.ahk");
        }
        //Fixed for Running ModNativesVerifier.ahk
        private void button4_Click(object sender, EventArgs e)
        {
            //Process.Start("C:\\Users\\rhyan\\Downloads\\Builk Pak File\\mod verifier\\App Mod Verifier\\Mod Verifier\\Mod Verifier\\ModNativesVerifier.ahk");
            RunAhkFile("ModNativesVerifier.ahk");
        }
        // Shared method to handle both cases
        private void RunAhkFile(string fileName)
        {
            try
            {
                string steamPath = GetSteamPath();
                if (steamPath == null)
                {
                    MessageBox.Show("Steam installation not found!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string mhwPath = FindMHWThroughSteam(steamPath);
                if (mhwPath == null)
                {
                    MessageBox.Show("MonsterHunterWilds installation not found in Steam!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string ahkPath = FindAhkFile(mhwPath, fileName);
                if (ahkPath == null)
                {
                    MessageBox.Show($"{fileName} not found in MonsterHunterWilds folder!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Process.Start(ahkPath);
                MessageBox.Show($"{fileName} launched successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Fixed for Button Run Mods
        private string GetSteamPath()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key != null)
                {
                    object path = key.GetValue("SteamPath");
                    if (path != null)
                    {
                        return path.ToString().Replace('/', '\\');
                    }
                }
            }
            return null;
        }
        //Fixed for Button Run Mods
        private string FindMHWThroughSteam(string steamPath)
        {
            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

            if (!File.Exists(libraryFoldersPath))
            {
                return CheckSteamLibrary(Path.Combine(steamPath, "steamapps", "common"));
            }

            try
            {
                string[] lines = File.ReadAllLines(libraryFoldersPath);
                foreach (string line in lines)
                {
                    if (line.Contains("\"path\""))
                    {
                        int start = line.IndexOf("\"", line.IndexOf("\"path\"") + 6) + 1;
                        int end = line.IndexOf("\"", start);
                        string libraryPath = line.Substring(start, end - start).Replace("\\\\", "\\");

                        string mhwPath = CheckSteamLibrary(Path.Combine(libraryPath, "steamapps", "common"));
                        if (mhwPath != null)
                        {
                            return mhwPath;
                        }
                    }
                }
            }
            catch { }

            return null;
        }
        //Fixed for Button Run Mods
        private string CheckSteamLibrary(string commonPath)
        {
            string mhwPath = Path.Combine(commonPath, "MonsterHunterWilds");
            return Directory.Exists(mhwPath) ? mhwPath : null;
        }
        //Fixed for Button Run Mods
        private string FindAhkFile(string gamePath, string fileName)
        {
            string[] searchLocations = {
            Path.Combine(gamePath, fileName), // Root folder
            Path.Combine(gamePath, "tools", fileName),
            Path.Combine(gamePath, "scripts", fileName),
            Path.Combine(gamePath, "mods", fileName),
            Path.Combine(gamePath, "nativePC", fileName),
            Path.Combine(gamePath, "verifier", fileName)
        };

            foreach (string path in searchLocations)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return SearchFileRecursive(gamePath, fileName);
        }
        //Fixed for Button Run Mods
        private string SearchFileRecursive(string directory, string fileName)
        {
            try
            {
                string filePath = Path.Combine(directory, fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }

                foreach (string subdir in Directory.GetDirectories(directory))
                {
                    string foundPath = SearchFileRecursive(subdir, fileName);
                    if (foundPath != null)
                    {
                        return foundPath;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip inaccessible directories
            }

            return null;
        }
       
        //fixed paypal
        private void button8_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/paypalme/MrXinji");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.autohotkey.com/");
        }
    }


}  
  