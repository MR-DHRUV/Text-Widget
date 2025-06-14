using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace TextWidgetApp
{
    public static class WindowSettingsManager
    {
        private static readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "window.config");
        //private static readonly string configPath = Path.Combine("E:\\DotNet\\Widgets\\TextWidgetApp", "window.config");


        public static void Save(Window window)
        {
            var lines = new[]
            {
                    $"Left={window.Left}",
                    $"Top={window.Top}",
                    $"Width={window.Width}",
                    $"Height={window.Height}"
                };
            File.WriteAllLines(configPath, lines);
        }

        public static void Load(Window window)
        {
            if (!File.Exists(configPath)) return;

            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                if (double.TryParse(parts[1], out double value))
                {
                    switch (key)
                    {
                        case "Left": window.Left = value; break;
                        case "Top": window.Top = value; break;
                        case "Width": window.Width = value; break;
                        case "Height": window.Height = value; break;
                    }
                }
            }
        }
    }

    public partial class App : System.Windows.Application
    {
        private NotifyIcon trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Startupp.Register();

            trayIcon = new NotifyIcon();
            // trayIcon.Icon = new System.Drawing.Icon("E:\\DotNet\\Widgets\\TextWidgetApp\\favicon.ico");
            trayIcon.Icon = new System.Drawing.Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favicon.ico"));

            trayIcon.Visible = true;
            trayIcon.Text = "Text Widget";

            // Right-click menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, args) =>
            {
                new MainWindow().Show();
            });
            contextMenu.Items.Add("Exit", null, (s, args) =>
            {
                trayIcon.Visible = false;
                Current.Shutdown();
            });
            trayIcon.ContextMenuStrip = contextMenu;

            // Double-click to show window
            trayIcon.DoubleClick += (s, args) =>
            {
                new MainWindow().Show();
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIcon.Dispose();
            base.OnExit(e);
        }
    }
}
