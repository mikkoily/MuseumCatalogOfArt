﻿using System;
using System.Windows.Forms;

namespace OOP_Project { static class Program {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainMenu()); 
        } 
    } 
}