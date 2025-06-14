using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Linq;

namespace TextWidgetApp
{
    public partial class MainWindow : Window
    {
        private string[] texts;
        private int currentIndex = 0;
        private DispatcherTimer timer;
        private FileSystemWatcher watcher;
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "texts.txt");
        // private static readonly string filePath = "E:\\DotNet\\Widgets\\TextWidgetApp\\texts.txt";

        public void LoadTextFromFile()
        {
            System.Diagnostics.Debug.WriteLine("Current Directory: " + Environment.CurrentDirectory);
            System.Diagnostics.Debug.WriteLine("Base Directory: " + AppDomain.CurrentDomain.BaseDirectory);
            System.Diagnostics.Debug.WriteLine("File Path: " + filePath);

            if (File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine("Loading texts from file: " + filePath);
                texts = File.ReadAllLines(filePath);
            }
        }

        public void WatchFileChanges()
        {
            watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "texts.txt");

            // watcher = new FileSystemWatcher("E:\\DotNet\\Widgets\\TextWidgetApp", "texts.txt");
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            
            watcher.Changed += (s, e) =>
            {
                Thread.Sleep(1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    currentIndex = 0;
                    LoadTextFromFile();
                    ShowNextText();
                });
            };

            watcher.EnableRaisingEvents = true;
        }

        public MainWindow()
        {
            InitializeComponent();

            // load lassaved window position and size
            WindowSettingsManager.Load(this);

            // enable dragging the window
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    this.DragMove();
            };

            LoadTextFromFile();
            WatchFileChanges();
            ShowNextText();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1000); // change every 10s
            timer.Tick += (a, b) => ShowNextText();
            timer.Start();

            this.Closing += (s, e) => WindowSettingsManager.Save(this);
        }

        private void ShowNextText()
        {
            if (texts == null || texts.Length == 0)
                return;

            TextPanel.Children.Clear();

            // Only use non-empty lines
            var lines = texts.Skip(currentIndex)
                             .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                             .ToArray();

            if (lines.Length == 0)
            {
                // Skip to next non-empty block
                currentIndex = (currentIndex + 1) % texts.Length;
                ShowNextText();
                return;
            }

            foreach (var line in lines)
            {
                var block = new TextBlock
                {
                    Text = line,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 80,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 4, 0, 4),
                };
                TextPanel.Children.Add(block);
            }

            // Advance to next group of lines (skip current block + next empty)
            currentIndex += lines.Length;
            while (currentIndex < texts.Length && string.IsNullOrWhiteSpace(texts[currentIndex]))
                currentIndex++;

            currentIndex %= texts.Length; // wrap around
        }

    }
}
