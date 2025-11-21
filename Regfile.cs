using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace vivaldi_Updater
{
    public partial class Regfile
    {
        public static void RegCreate(string applicationPath, string instDir, int icon)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\vivaldiHTML.PORTABLE");
            key.SetValue(default, "Vivaldi HTML Document");
            key.SetValue("AppUserModelId", "Vivaldi.PORTABLE");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\vivaldiHTML.PORTABLE\\Application");
            key.SetValue("AppUserModelId", "Vivaldi.PORTABLE");
            key.SetValue("ApplicationIcon", applicationPath + @"\" + instDir + @"\Vivaldi.exe," + icon);
            key.SetValue("ApplicationName", "Google " + instDir + @" Portable");
            key.SetValue("ApplicationDescription", Langfile.Texts("AppDescriptShort"));
            key.SetValue("ApplicationCompany", "Google LLC");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\vivaldiHTML.PORTABLE\\DefaultIcon");
            key.SetValue(default, applicationPath + @"\" + instDir + @"\Vivaldi.exe," + icon);
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\vivaldiHTML.PORTABLE\\shell\\open\\command");
            key.SetValue(default, "\"" + applicationPath + @"\" + instDir + @" Launcher.exe"" ""%1""");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\RegisteredApplications");
            key.SetValue("Google Vivaldi.PORTABLE", @"Software\Clients\StartMenuInternet\Google Vivaldi.PORTABLE\Capabilities");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE");
            key.SetValue(default, "Google " + instDir + @" Portable");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities");
            key.SetValue("ApplicationDescription", Langfile.Texts("AppDescriptFull"));
            key.SetValue("ApplicationIcon", applicationPath + @"\" + instDir + @"\Vivaldi.exe," + icon);
            key.SetValue("ApplicationName", "Google " + instDir + @" Portable");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\FileAssociations");
            key.SetValue(".htm", "vivaldiHTML.PORTABLE");
            key.SetValue(".html", "vivaldiHTML.PORTABLE");
            key.SetValue(".shtml", "vivaldiHTML.PORTABLE");
            key.SetValue(".svg", "vivaldiHTML.PORTABLE");
            key.SetValue(".xht", "vivaldiHTML.PORTABLE");
            key.SetValue(".xhtml", "vivaldiHTML.PORTABLE");
            key.SetValue(".webp", "vivaldiHTML.PORTABLE");
            key.SetValue(".pdf", "vivaldiHTML.PORTABLE");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\Startmenu");
            key.SetValue("StartMenuInternet", "Google Vivaldi.PORTABLE");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\URLAssociations");
            key.SetValue("ftp", "vivaldiHTML.PORTABLE");
            key.SetValue("http", "vivaldiHTML.PORTABLE");
            key.SetValue("https", "vivaldiHTML.PORTABLE");
            key.SetValue("irc", "vivaldiHTML.PORTABLE");
            key.SetValue("mailto", "vivaldiHTML.PORTABLE");
            key.SetValue("mms", "vivaldiHTML.PORTABLE");
            key.SetValue("news", "vivaldiHTML.PORTABLE");
            key.SetValue("nntp", "vivaldiHTML.PORTABLE");
            key.SetValue("read", "vivaldiHTML.PORTABLE");
            key.SetValue("sms", "vivaldiHTML.PORTABLE");
            key.SetValue("smsto", "vivaldiHTML.PORTABLE");
            key.SetValue("tel", "vivaldiHTML.PORTABLE");
            key.SetValue("urn", "vivaldiHTML.PORTABLE");
            key.SetValue("webcal", "vivaldiHTML.PORTABLE");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\DefaultIcon");
            key.SetValue("ApplicationIcon", applicationPath + @"\" + instDir + @"\Vivaldi.exe," + icon);
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\InstallInfo");
            key.SetValue("ReinstallCommand", "\"" + applicationPath + @"\\" + instDir + @" Launcher.exe"" --make-default-browser");
            key.SetValue("HideIconsCommand", "\"" + applicationPath + @"\\" + instDir + @" Launcher.exe"" --hide-icons");
            key.SetValue("ShowIconsCommand", "\"" + applicationPath + @"\\" + instDir + @" Launcher.exe"" --show-icons");
            key.SetValue("IconsVisible", 1, Microsoft.Win32.RegistryValueKind.DWord);
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\shell\\open\\command");
            key.SetValue(default, "\"" + applicationPath + @"\" + instDir + @" Launcher.exe"" ""%1""");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.xhtml\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.xht\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.webp\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.svg\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.shtml\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.pdf\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.html\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\.htm\\OpenWithProgids");
            key.SetValue("vivaldiHTML.PORTABLE", "");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\Vivaldi.exe");
            key.SetValue(default, "\"" + applicationPath + @"\" + instDir + @" Launcher.exe"" ""%1""");
            key.SetValue("Path", applicationPath);
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ApplicationAssociationToasts");
            key.SetValue("vivaldiHTML.PORTABLE_http", 0, Microsoft.Win32.RegistryValueKind.DWord);
			key.SetValue("vivaldiHTML.PORTABLE_https", 0, Microsoft.Win32.RegistryValueKind.DWord);
			key.SetValue("vivaldiHTML.PORTABLE_.htm", 0, Microsoft.Win32.RegistryValueKind.DWord);
			key.SetValue("vivaldiHTML.PORTABLE_.html", 0, Microsoft.Win32.RegistryValueKind.DWord);
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\https\\UserChoice");
            key.SetValue("ProgId", "vivaldiHTML.PORTABLE");
            key.Close();
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice");
            key.SetValue("ProgId", "vivaldiHTML.PORTABLE");
            key.Close();
            try
            {
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", false);
                if (key.GetValue("ProductName").ToString().Contains("Windows 10"))
                {
                    key.Close();
                    Process process = new Process();
                    process.StartInfo.FileName = "ms-settings:defaultapps";
                    process.Start();
                }
                else
                {
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void RegDel()
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.pdf\\UserChoice", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\FileAssociations", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\shell\\open\\command", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\shell\\open", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\shell", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\DefaultIcon", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\InstallInfo", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\Startmenu", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE\\Capabilities\\URLAssociations", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Clients\\StartMenuInternet\\Google Vivaldi.PORTABLE", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable\\shell\\open\\command", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable\\shell\\open", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable\\shell", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable\\DefaultIcon", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable\\Application", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Classes\\vivaldiHTML.Portable", false);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\Vivaldi.exe", false);
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\RegisteredApplications", true);
                key.DeleteValue("Google Vivaldi.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.xhtml\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.xht\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.webp\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.svg\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.shtml\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.pdf\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.html\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\.htm\\OpenWithProgids", true);
                key.DeleteValue("vivaldiHTML.PORTABLE", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ApplicationAssociationToasts", true);
                key.DeleteValue("vivaldiHTML.PORTABLE_http", false);
				key.DeleteValue("vivaldiHTML.PORTABLE_https", false);
				key.DeleteValue("vivaldiHTML.PORTABLE_.htm", false);
				key.DeleteValue("vivaldiHTML.PORTABLE_.html", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\https\\UserChoice", true);
                key.DeleteValue("Hash", false);
                key.DeleteValue("ProgId", false);
                key.Close();
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", true);                
                key.DeleteValue("Hash", false);
                key.DeleteValue("ProgId", false);
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
