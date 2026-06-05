
Claude finished the response

using System.Diagnostics; using System.Net; using System.Net.Http; using System.Runtime.InteropServices; using System.Xml.Linq; namespace PorVivaldiUpd; public partial class Form1 : Form { private readonly HttpClient _http; private CancellationTokenSource? _cts; private str

pasted

2:17 PM

Awaited clarification on code review requirements
Awaited clarification on code review requirements
Ciao! Hai incollato il codice di PorVivaldiUpd ma non hai indicato cosa ti serve. Come posso aiutarti?

Ecco alcune cose che posso fare:

Revisione del codice – bug, miglioramenti, robustezza
Nuova funzionalità – es. aggiornamento automatico all'avvio, notifiche, log dettagliato
Refactoring – separare la logica UI dalla business logic, async più pulito, ecc.
Bug specifico – se hai un problema da risolvere dimmi pure
Altro – documentazione, traduzione UI, packaging...
Cosa ti serve?


using System; using System.Diagnostics; using System.Drawing; using System.IO; using System.Net; using System.Windows.Forms; using System.Threading; using System.Threading.Tasks; using System.Globalization; using System.Collections.Generic; namespace Firefox_Updater { public partial class Form1

pasted

mimic this code, add Download 7zip if missing and desktop links

2:20 PM

Architected updater refactor incorporating 7zip download and desktop shortcut functionality
Architected updater refactor incorporating 7zip download and desktop shortcut functionality
Here's what was added and changed, modelled on the Firefox updater style:

Download7zrIfMissing() — called in Load before RefreshAll(). Silently creates the Bin\ folder if needed and downloads 7zr.exe from 7-zip.org if the file is absent. Updates lblStatus while it works, so the user sees feedback.

CreateDesktopShortcut(ArchData a) — called in StartUpdate right after a successful install, only when chkDesktopLink is checked. Uses the same IWshRuntimeLibrary.WshShell COM approach as the Firefox updater. It skips creation if the .lnk already exists (so re-runs don't accidentally overwrite a moved shortcut). Points to Application\vivaldi.exe inside the install folder.

chkDesktopLink checkbox — added below chkDeleteTemp at the bottom of the form, unchecked by default (matching Firefox updater's checkBox5 pattern).

One project setup note: to use IWshRuntimeLibrary you need to add a COM reference — in Visual Studio: Project → Add COM Reference → Windows Script Host Object Model. If you prefer to avoid COM, let me know and I can swap it for a pure-managed shortcut writer using System.Runtime.InteropServices.


Form1
CS 
//-------------------------------------------------------
        // CREATE DESKTOP SHORTCUTS USING DYNAMIC COM
        //-------------------------------------------------------
        private void CreateDesktopShortcuts()
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string chromePath = Path.Combine(destFolder, "chrome.exe");
                string profileCommonDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "profile");
                string profileDir = Path.Combine(destFolder, "profile");
                //Directory.CreateDirectory(profileDir);
                // 1) Simple
                CreateShortcut(desktop, "Chromium.lnk", chromePath, "");
                // 2) With --user-data-dir=
                CreateShortcut(desktop, "Chromium - Common Profile.lnk", chromePath, $"--user-data-dir=\"{profileCommonDir}\"");
                // 3) With --user-data-dir=Chromium Stable x64\profile
                CreateShortcut(desktop, "Chromium - Single Profile.lnk", chromePath, $"--user-data-dir=\"{profileDir}\"");
            }
            catch (Exception ex)
            {
                LogError("CreateDesktopShortcuts failed: " + ex);
            }
        }
        private void CreateShortcut(string folder, string shortcutName, string targetPath, string arguments)
        {
            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            dynamic shortcut = shell.CreateShortcut(Path.Combine(folder, shortcutName));
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.Save();
        }

Show more
2:22 PM

Refactored shortcut creation logic using dynamic COM approach
Refactored shortcut creation logic using dynamic COM approach
Clean approach — no COM reference needed. Let me update the file:


Edited a file, read a file
Edited a file, read a file
Updated. The key differences from the old version:

No COM reference required — Type.GetTypeFromProgID("WScript.Shell") + dynamic resolves WScript.Shell at runtime, so no need to add anything to the project references.

Three shortcuts created per arch (mirroring your Chromium pattern):

Shortcut	--user-data-dir
Vivaldi Stable x64.lnk	(none — default profile)
Vivaldi Stable x64 - Common Profile.lnk	ExeDir\profile shared across all arches
Vivaldi Stable x64 - Single Profile.lnk	ExeDir\Vivaldi Stable x64\profile isolated
The only project requirement is <LangVersion> set to at least 9.0 (for dynamic) — which any modern .NET WinForms project already has.


Form1
CS 





Claude is AI and can make mistakes. Please double-check responses.
Form1 · CS
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml.Linq;
 
namespace PorVivaldiUpd;
 
public partial class Form1 : Form
{
    private readonly HttpClient _http;
    private CancellationTokenSource? _cts;
    private string? _downloadedFile;
 
    private static readonly string ExeDir   = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string SevenZip = Path.Combine(ExeDir, "Bin", "7zr.exe");
    private static readonly string DeskDir  = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
 
    private Label    lblStatus    = null!;
    private Label    lblProgress  = null!;
    private Label    lblSpeed     = null!;
    private Label    lblEta       = null!;
    private Label    lblSysArch   = null!;
    private ProgressBar progressBar = null!;
    private CheckBox chkDeleteTemp  = null!;
    private CheckBox chkDesktopLink = null!;
 
    private record ArchData(
        string Name,
        string Folder,
        string FeedUrl,
        string ArchCode,
        Label  lblInstalled,
        Label  lblAvailable,
        Button btnAction);
 
    private ArchData[] arches = null!;
 
    // -------------------------------------------------------------------------
    public Form1()
    {
        Text             = "PorVivaldiUpd – Portable Vivaldi Updater";
        Size             = new Size(600, 420);
        FormBorderStyle  = FormBorderStyle.FixedSingle;
        MaximizeBox      = false;
        StartPosition    = FormStartPosition.CenterScreen;
        Font             = new Font("Segoe UI", 9.5F);
        BackColor        = Color.FromArgb(248, 249, 250);
        ForeColor        = Color.Black;
 
        var handler = new HttpClientHandler
        {
            UseProxy               = true,
            Proxy                  = WebRequest.DefaultWebProxy,
            DefaultProxyCredentials = CredentialCache.DefaultCredentials
        };
        _http = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("PorVivaldiUpd/10.2");
 
        // ── top arch label ──────────────────────────────────────────────────
        lblSysArch = new Label
        {
            Text      = $"Sistema: {RuntimeInformation.OSArchitecture} • {RuntimeInformation.ProcessArchitecture}",
            Dock      = DockStyle.Top,
            Height    = 36,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(16, 10, 0, 0),
            Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 120, 215)
        };
 
        // ── arch rows ───────────────────────────────────────────────────────
        arches = new[]
        {
            new ArchData("x86",   "Vivaldi Stable x86",
                "https://update.vivaldi.com/update/1.0/public/appcast.xml",       "x86",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton()),
            new ArchData("x64",   "Vivaldi Stable x64",
                "https://update.vivaldi.com/update/1.0/public/appcast.x64.xml",   "x64",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton()),
            new ArchData("ARM64", "Vivaldi Stable arm64",
                "https://update.vivaldi.com/update/1.0/public/appcast.arm64.xml", "arm64",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton())
        };
 
        foreach (var a in arches)
            a.btnAction.Click += (_, _) => StartUpdate(a);
 
        // ── option checkboxes ───────────────────────────────────────────────
        chkDeleteTemp = new CheckBox
        {
            Text     = "Elimina file scaricato dopo l'aggiornamento",
            Checked  = true,
            AutoSize = true,
            Location = new Point(16, 345),
            Font     = new Font("Segoe UI", 9.5F)
        };
 
        chkDesktopLink = new CheckBox
        {
            Text     = "Crea collegamento sul Desktop",
            Checked  = false,
            AutoSize = true,
            Location = new Point(16, 368),
            Font     = new Font("Segoe UI", 9.5F)
        };
 
        // ── status panel ────────────────────────────────────────────────────
        var statusPanel = new TableLayoutPanel
        {
            Dock        = DockStyle.Bottom,
            Height      = 110,
            RowCount    = 4,
            ColumnCount = 1
        };
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
 
        lblStatus   = new Label     { Dock = DockStyle.Fill, Text = "Pronto", Padding = new Padding(12, 6, 0, 0) };
        progressBar = new ProgressBar { Dock = DockStyle.Fill, Margin = new Padding(12, 4, 12, 4), Visible = false };
        lblProgress = new Label     { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
        lblSpeed    = new Label     { Dock = DockStyle.Fill, Padding = new Padding(12, 0, 0, 0) };
        lblEta      = new Label     { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 12, 0) };
 
        statusPanel.Controls.Add(lblStatus,   0, 0);
        statusPanel.Controls.Add(progressBar, 0, 1);
        statusPanel.Controls.Add(lblProgress, 0, 1);
        statusPanel.Controls.Add(lblSpeed,    0, 2);
        statusPanel.Controls.Add(lblEta,      0, 3);
 
        // ── arch grid ───────────────────────────────────────────────────────
        var table = new TableLayoutPanel
        {
            Dock    = DockStyle.Top,
            Height  = 180,
            Padding = new Padding(16, 8, 16, 8)
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
 
        table.Controls.Add(Bold("Architettura"), 0, 0);
        table.Controls.Add(Bold("Installata"),   1, 0);
        table.Controls.Add(Bold("Disponibile"),  2, 0);
        table.Controls.Add(Bold("Stato"),        3, 0);
 
        for (int i = 0; i < arches.Length; i++)
        {
            var a = arches[i];
            table.Controls.Add(new Label { Text = a.Name, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, i + 1);
            table.Controls.Add(a.lblInstalled, 1, i + 1);
            table.Controls.Add(a.lblAvailable, 2, i + 1);
            table.Controls.Add(a.btnAction,    3, i + 1);
        }
 
        Controls.Add(table);
        Controls.Add(chkDeleteTemp);
        Controls.Add(chkDesktopLink);
        Controls.Add(statusPanel);
        Controls.Add(lblSysArch);
 
        FormClosing += (_, _) => CleanupDownloadedFile();
        Load        += async (_, _) =>
        {
            Download7zrIfMissing();
            await RefreshAll();
        };
    }
 
    // ── helpers ───────────────────────────────────────────────────────────────
    private Label Bold(string t) => new() { Text = t, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
 
    private Button CreateButton() => new()
    {
        Width      = 120,
        Height     = 40,
        FlatStyle  = FlatStyle.Flat,
        Font       = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor  = Color.White,
        FlatAppearance = { BorderSize = 0 }
    };
 
    // ── 7zr.exe auto-download ─────────────────────────────────────────────────
    private void Download7zrIfMissing()
    {
        string binDir = Path.Combine(ExeDir, "Bin");
 
        try
        {
            if (!Directory.Exists(binDir))
                Directory.CreateDirectory(binDir);
 
            if (File.Exists(SevenZip))
                return;
 
            lblStatus.Text = "Download 7zr.exe...";
 
            using var client = new WebClient
            {
                Proxy = WebRequest.DefaultWebProxy
            };
            client.DownloadFile("https://www.7-zip.org/a/7zr.exe", SevenZip);
 
            lblStatus.Text = "Pronto";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Impossibile scaricare 7zr.exe:\n{ex.Message}",
                "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
 
    // ── refresh ───────────────────────────────────────────────────────────────
    private async Task RefreshAll()
    {
        foreach (var a in arches) await Refresh(a);
    }
 
    private async Task Refresh(ArchData a)
    {
        string logPath   = Path.Combine(ExeDir, a.Folder, "updates", "Version.log");
        string installed = "Non installata";
 
        if (File.Exists(logPath))
        {
            try
            {
                string line = await File.ReadAllTextAsync(logPath);
                if (line.Contains('|')) installed = line.Split('|')[0].Trim();
            }
            catch { }
        }
 
        a.lblInstalled.Text = installed;
 
        try
        {
            string xml = await _http.GetStringAsync(a.FeedUrl);
            var doc    = XDocument.Parse(xml);
            var ns     = XNamespace.Get("http://www.andymatuschak.org/xml-namespaces/sparkle");
            var enc    = doc.Descendants("enclosure")
                           .FirstOrDefault(e => e.Attribute("url")?.Value
                               .EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true);
 
            string latest = enc?.Attribute(ns + "version")?.Value ?? "?";
            a.lblAvailable.Text = latest;
 
            if (installed == "Non installata")
            {
                a.btnAction.Text      = "Installa";
                a.btnAction.BackColor = Color.FromArgb(70, 70, 70);
                a.btnAction.Enabled   = true;
            }
            else if (installed == latest)
            {
                a.btnAction.Text      = "Aggiornata";
                a.btnAction.BackColor = Color.ForestGreen;
                a.btnAction.Enabled   = false;
            }
            else
            {
                a.btnAction.Text      = "Aggiorna";
                a.btnAction.BackColor = Color.Crimson;
                a.btnAction.Enabled   = true;
            }
        }
        catch
        {
            a.lblAvailable.Text   = "Offline";
            a.btnAction.Text      = "Offline";
            a.btnAction.BackColor = Color.OrangeRed;
        }
    }
 
    // ── update flow ───────────────────────────────────────────────────────────
    private async void StartUpdate(ArchData a)
    {
        a.btnAction.Enabled = false;
        a.btnAction.Text    = "Avvio...";
        _cts = new CancellationTokenSource();
        ResetProgress();
 
        try
        {
            var xml = await _http.GetStringAsync(a.FeedUrl);
            var doc = XDocument.Parse(xml);
            var ns  = XNamespace.Get("http://www.andymatuschak.org/xml-namespaces/sparkle");
            var enc = doc.Descendants("enclosure")
                        .FirstOrDefault(e => e.Attribute("url")?.Value
                            .EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true);
 
            if (enc == null) throw new Exception("Feed non valido");
 
            string version = enc.Attribute(ns + "version")!.Value;
            string url     = enc.Attribute("url")!.Value;
 
            _downloadedFile = Path.Combine(ExeDir, $"Vivaldi-{version}-{a.ArchCode}.exe");
 
            lblStatus.Text = $"Download {a.Name} {version}...";
            await DownloadFileWithProgress(url, _downloadedFile);
 
            lblStatus.Text = "Estrazione e installazione...";
            await Task.Run(() => ExtractAndInstall(_downloadedFile, a.Folder, version, a.ArchCode));
 
            // ── desktop shortcut ──────────────────────────────────────────
            if (chkDesktopLink.Checked)
                CreateDesktopShortcut(a);
 
            MessageBox.Show($"Vivaldi {a.Name} aggiornato alla versione {version}!",
                "Completato", MessageBoxButtons.OK, MessageBoxIcon.Information);
 
            CleanupDownloadedFile();
            await Refresh(a);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore: {ex.Message}", "Errore",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            a.btnAction.Text      = "Errore";
            a.btnAction.BackColor = Color.IndianRed;
        }
        finally
        {
            a.btnAction.Enabled = true;
            ResetProgress();
            lblStatus.Text = "Pronto";
        }
    }
 
    // ── desktop shortcut creation ─────────────────────────────────────────────
    private void CreateDesktopShortcut(ArchData a)
    {
        try
        {
            string appDir    = Path.Combine(ExeDir, a.Folder, "Application");
            string targetExe = Path.Combine(appDir, "vivaldi.exe");
            string profileCommonDir = Path.Combine(ExeDir, "profile");
            string profileLocalDir  = Path.Combine(ExeDir, a.Folder, "profile");
 
            // 1) Simple launch
            CreateShortcut(DeskDir, $"{a.Folder}.lnk",
                targetExe, "");
 
            // 2) Shared profile (common across all arches)
            CreateShortcut(DeskDir, $"{a.Folder} - Common Profile.lnk",
                targetExe, $"--user-data-dir=\"{profileCommonDir}\"");
 
            // 3) Per-arch isolated profile
            CreateShortcut(DeskDir, $"{a.Folder} - Single Profile.lnk",
                targetExe, $"--user-data-dir=\"{profileLocalDir}\"");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Impossibile creare collegamento sul Desktop:\n{ex.Message}",
                "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
 
    private void CreateShortcut(string folder, string shortcutName, string targetPath, string arguments)
    {
        Type   shellType = Type.GetTypeFromProgID("WScript.Shell")!;
        dynamic shell    = Activator.CreateInstance(shellType)!;
        dynamic shortcut = shell.CreateShortcut(Path.Combine(folder, shortcutName));
        shortcut.TargetPath       = targetPath;
        shortcut.Arguments        = arguments;
        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
        shortcut.Save();
    }
 
    // ── download with progress ────────────────────────────────────────────────
    private async Task DownloadFileWithProgress(string url, string path)
    {
        using var response = await _http.GetAsync(
            url, HttpCompletionOption.ResponseHeadersRead, _cts!.Token);
        response.EnsureSuccessStatusCode();
 
        long   total      = response.Content.Headers.ContentLength ?? 0;
        long   downloaded = 0;
        var    sw         = Stopwatch.StartNew();
        var    buffer     = new byte[16384];
 
        progressBar.Visible = true;
        progressBar.Value   = 0;
 
        await using var stream = await response.Content.ReadAsStreamAsync(_cts.Token);
        await using var file   = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
 
        int read;
        while ((read = await stream.ReadAsync(buffer, _cts.Token)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read), _cts.Token);
            downloaded += read;
 
            if (sw.ElapsedMilliseconds >= 300)
            {
                sw.Restart();
                double percent = total > 0 ? downloaded * 100.0 / total : 0;
                double speed   = downloaded / 1_048_576.0 /
                                 (Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency);
                double eta     = total > 0 ? (total - downloaded) / (speed * 1_048_576) : 0;
 
                BeginInvoke(() =>
                {
                    progressBar.Value  = (int)Math.Min(100, percent);
                    lblProgress.Text   = $"{percent:F1}%  ({downloaded / 1_048_576:F1} / {total / 1_048_576:F0} MB)";
                    lblSpeed.Text      = $"Velocità: {speed:F2} MB/s";
                    lblEta.Text        = eta > 0 ? $"ETA: {TimeSpan.FromSeconds(eta):mm\\:ss}" : "";
                });
            }
        }
    }
 
    // ── progress reset ────────────────────────────────────────────────────────
    private void ResetProgress()
    {
        BeginInvoke(() =>
        {
            progressBar.Visible = false;
            progressBar.Value   = 0;
            lblProgress.Text    = "";
            lblSpeed.Text       = "";
            lblEta.Text         = "";
        });
    }
 
    // ── extract and install ───────────────────────────────────────────────────
    private void ExtractAndInstall(string installer, string targetFolder, string version, string arch)
    {
        if (!File.Exists(SevenZip))
            throw new FileNotFoundException("7zr.exe non trovato in Bin\\");
 
        string temp = Path.Combine(Path.GetTempPath(), "VivaldiUpd_" + Guid.NewGuid().ToString("N")[..12]);
        Directory.CreateDirectory(temp);
 
        string target       = Path.Combine(ExeDir, targetFolder);
        string updatesPath  = Path.Combine(target, "updates");
        string profileFile  = Path.Combine(target, "Profile.txt");
        string profileFolder= Path.Combine(target, "profile");
 
        string backup = Path.Combine(temp, "backup");
        Directory.CreateDirectory(backup);
 
        // ── backup user data ──────────────────────────────────────────────
        if (Directory.Exists(updatesPath))  CopyDir(updatesPath,  Path.Combine(backup, "updates"));
        if (File.Exists(profileFile))       File.Copy(profileFile, Path.Combine(backup, "Profile.txt"), true);
        if (Directory.Exists(profileFolder))CopyDir(profileFolder, Path.Combine(backup, "profile"));
 
        // ── wipe old install (keep profile/updates) ───────────────────────
        if (Directory.Exists(target))
        {
            foreach (var dir in Directory.GetDirectories(target, "*", SearchOption.AllDirectories).Reverse())
            {
                if (dir.EndsWith("\\updates", StringComparison.OrdinalIgnoreCase) ||
                    dir.EndsWith("\\profile",  StringComparison.OrdinalIgnoreCase))
                    continue;
                try { Directory.Delete(dir, true); } catch { }
            }
            foreach (var file in Directory.GetFiles(target, "*", SearchOption.TopDirectoryOnly))
                if (!string.Equals(Path.GetFileName(file), "Profile.txt", StringComparison.OrdinalIgnoreCase))
                    try { File.Delete(file); } catch { }
        }
        else Directory.CreateDirectory(target);
 
        // ── extract ───────────────────────────────────────────────────────
        Run7z($"x \"{installer}\" vivaldi.7z -o\"{temp}\" -y");
        string v7z = Path.Combine(temp, "vivaldi.7z");
        if (!File.Exists(v7z)) throw new FileNotFoundException("vivaldi.7z non trovato");
        Run7z($"x \"{v7z}\" -o\"{temp}\" -y");
 
        string bin = Path.Combine(temp, "Vivaldi-bin");
        if (!Directory.Exists(bin)) throw new FileNotFoundException("Cartella Vivaldi-bin non trovata");
 
        foreach (string file in Directory.GetFiles(bin, "*", SearchOption.AllDirectories))
        {
            string rel  = Path.GetRelativePath(bin, file);
            string dest = Path.Combine(target, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, true);
        }
 
        // ── restore user data ─────────────────────────────────────────────
        var buUpdates = Path.Combine(backup, "updates");
        if (Directory.Exists(buUpdates))   CopyDir(buUpdates, updatesPath, true);
        var buProfile = Path.Combine(backup, "Profile.txt");
        if (File.Exists(buProfile))        File.Copy(buProfile, profileFile, true);
        var buProfileFolder = Path.Combine(backup, "profile");
        if (Directory.Exists(buProfileFolder)) CopyDir(buProfileFolder, profileFolder, true);
 
        Directory.Delete(temp, true);
 
        // ── version log ───────────────────────────────────────────────────
        Directory.CreateDirectory(updatesPath);
        File.WriteAllText(Path.Combine(updatesPath, "Version.log"), $"{version}|Stable|{arch}");
    }
 
    // ── run 7zr ───────────────────────────────────────────────────────────────
    private void Run7z(string args)
    {
        var p = Process.Start(new ProcessStartInfo
        {
            FileName       = SevenZip,
            Arguments      = args,
            UseShellExecute = false,
            CreateNoWindow  = true
        });
        p?.WaitForExit();
        if (p?.ExitCode != 0 && p?.ExitCode != null)
            throw new Exception($"7zr.exe errore: {p.ExitCode}");
    }
 
    // ── dir copy ──────────────────────────────────────────────────────────────
    private void CopyDir(string source, string dest, bool overwrite = true)
    {
        Directory.CreateDirectory(dest);
        foreach (var f in Directory.GetFiles(source))
            File.Copy(f, Path.Combine(dest, Path.GetFileName(f)), overwrite);
        foreach (var d in Directory.GetDirectories(source))
            CopyDir(d, Path.Combine(dest, Path.GetFileName(d)), overwrite);
    }
 
    // ── cleanup ───────────────────────────────────────────────────────────────
    private void CleanupDownloadedFile()
    {
        if (chkDeleteTemp.Checked && _downloadedFile != null && File.Exists(_downloadedFile))
            try { File.Delete(_downloadedFile); } catch { }
    }
}
 
