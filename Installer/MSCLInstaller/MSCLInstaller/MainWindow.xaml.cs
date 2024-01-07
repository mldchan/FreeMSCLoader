﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectGameFolder sgf;
        public MainWindow()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists("Log.txt")) File.Delete("Log.txt");
            Dbg.Init();
            Dbg.Log($"Current folder: {Path.GetFullPath(".")}");
            sgf = new SelectGameFolder();
            if(Directory.GetFiles(".", "*.pack").Length == 0)
            {
                MessageBox.Show($"Couldn't find any required core files.{Environment.NewLine}Please unpack all files before launching this installer!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dbg.Log($"!!! CRASH !!!", true, true);
                Dbg.Log($"Core files not found, exiting.");
                Environment.Exit(0);
            }
            SelectGameFolderPage(Game.MSC);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectGameFolderPage(Game.MSC);
        }

        public void SelectGameFolderPage(Game game)
        {
            Dbg.Log("Select Game Folder", true, true);
            Storage.selectedGame = game;
            MainFrame.Content = sgf;
            sgf.Init(this);
        }
    }
}
