﻿using RBXMSEAPI.Classes;
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

namespace RobloxLuaGitHub
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fluxteam_net_api.create_files(Path.GetFullPath("Module.dll"));
            if (!fluxteam_net_api.is_injected(fluxteam_net_api.pid))
            {
                fluxteam_net_api.inject();
                MessageBox.Show("Injected", "Injected");
            }
            else
            {
                MessageBox.Show("Already injected", "Already injected");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fluxteam_net_api.is_injected(fluxteam_net_api.pid))
            {
                fluxteam_net_api.run_script(fluxteam_net_api.pid, richTextBox1.Text);
            }
            else
            {
                MessageBox.Show("Inject before running script", "Please inject");
            }
        }
        string scriptBtn = "Script here!";
        private void button4_Click(object sender, EventArgs e)
        {
            if (fluxteam_net_api.is_injected(fluxteam_net_api.pid))
            {
                fluxteam_net_api.run_script(fluxteam_net_api.pid, scriptBtn);
            }
            else
            {
                MessageBox.Show("Inject before running script", "Please inject");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}
