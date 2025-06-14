using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace TextWidgetApp
{
    public static class Startupp
    {
        public static void Register()
        {
            string appName = "TextWidgetApp";
            string exePath = Assembly.GetExecutingAssembly().Location;

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key.GetValue(appName) == null)
                    {
                        key.SetValue(appName, $"\"{exePath}\"");
                        Console.WriteLine("Registered for startup.");
                    } else {
                        Console.WriteLine("Already registered for startup.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to register for startup: " + ex.Message);
            }
        }

        public static void Unregister()
        {
            string appName = "TextWidgetApp";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key.GetValue(appName) != null)
                    {
                        key.DeleteValue(appName);
                        Console.WriteLine("Unregistered from startup.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to unregister: " + ex.Message);
            }
        }
    }
}
