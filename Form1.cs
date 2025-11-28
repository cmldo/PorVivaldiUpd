using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace PorVivaldiUpd;

public partial class Form1 : Form
{
    private readonly HttpClient _http;
    private CancellationTokenSource? _cts;
    private string? _downloadedFile;

    private static readonly string ExeDir = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string SevenZip = Path.Combine(ExeDir, "Bin", "7zr.exe");

    private Label lblStatus = null!;
    private Label lblProgress = null!;
    private Label lblSpeed = null!;
    private Label lblEta = null!;
    private Label lblSysArch = null!;
    private ProgressBar progressBar = null!;
    private CheckBox chkDeleteTemp = null!;

    private record ArchData(
        string Name,
        string Folder,
        string FeedUrl,
        string ArchCode,
        Label lblInstalled,
        Label lblAvailable,
        Button btnAction);

    private ArchData[] arches = null!;

    public Form1()
    {
        Text = "PorVivaldiUpd – Portable Vivaldi Updater";
        Size = new Size(600, 400);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5F);
        BackColor = Color.FromArgb(248, 249, 250);
        ForeColor = Color.Black;

        var handler = new HttpClientHandler
        {
            UseProxy = true,
            Proxy = WebRequest.DefaultWebProxy,
            DefaultProxyCredentials = CredentialCache.DefaultCredentials
        };
        _http = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("PorVivaldiUpd/10.2");

        lblSysArch = new Label
        {
            Text = $"Sistema: {RuntimeInformation.OSArchitecture} • {RuntimeInformation.ProcessArchitecture}",
            Dock = DockStyle.Top,
            Height = 36,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 10, 0, 0),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 120, 215)
        };

        arches = new[]
        {
            new ArchData("x86",   "Vivaldi Stable x86",   "https://update.vivaldi.com/update/1.0/public/appcast.xml",      "x86",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton()),
            new ArchData("x64",   "Vivaldi Stable x64",   "https://update.vivaldi.com/update/1.0/public/appcast.x64.xml",  "x64",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton()),
            new ArchData("ARM64", "Vivaldi Stable arm64", "https://update.vivaldi.com/update/1.0/public/appcast.arm64.xml","arm64",
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                new Label { Width = 145, TextAlign = ContentAlignment.MiddleLeft },
                CreateButton())
        };

        foreach (var a in arches)
            a.btnAction.Click += (_, _) => StartUpdate(a);

        chkDeleteTemp = new CheckBox
        {
            Text = "Elimina file scaricato dopo l'aggiornamento",
            Checked = true,
            AutoSize = true,
            Location = new Point(300, 335),
            Font = new Font("Segoe UI", 9.5F)
        };

        var statusPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 110,
            RowCount = 4,
            ColumnCount = 1
        };
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        statusPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        lblStatus   = new Label { Dock = DockStyle.Fill, Text = "Pronto", Padding = new Padding(12, 6, 0, 0) };
        progressBar = new ProgressBar { Dock = DockStyle.Fill, Margin = new Padding(12, 4, 12, 4), Visible = false };
        lblProgress = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
        lblSpeed    = new Label { Dock = DockStyle.Fill, Padding = new Padding(12, 0, 0, 0) };
        lblEta      = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 12, 0) };

        statusPanel.Controls.Add(lblStatus, 0, 0);
        statusPanel.Controls.Add(progressBar, 0, 1);
        statusPanel.Controls.Add(lblProgress, 0, 1);
        statusPanel.Controls.Add(lblSpeed, 0, 2);
        statusPanel.Controls.Add(lblEta, 0, 3);

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 180,
            Padding = new Padding(16, 8, 16, 8)
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        table.Controls.Add(Bold("Architettura"), 0, 0);
        table.Controls.Add(Bold("Installata"), 1, 0);
        table.Controls.Add(Bold("Disponibile"), 2, 0);
        table.Controls.Add(Bold("Stato"), 3, 0);

        for (int i = 0; i < arches.Length; i++)
        {
            var a = arches[i];
            table.Controls.Add(new Label { Text = a.Name, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }, 0, i + 1);
            table.Controls.Add(a.lblInstalled, 1, i + 1);
            table.Controls.Add(a.lblAvailable, 2, i + 1);
            table.Controls.Add(a.btnAction, 3, i + 1);
        }

        Controls.Add(table);
        Controls.Add(chkDeleteTemp);
        Controls.Add(statusPanel);
        Controls.Add(lblSysArch);

        FormClosing += (_, e) => CleanupDownloadedFile();
        Load += async (_, _) => await RefreshAll();
    }

    private Label Bold(string t) => new() { Text = t, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

    private Button CreateButton() => new()
    {
        Width = 120,
        Height = 40,
        FlatStyle = FlatStyle.Flat,
        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor = Color.White,
        FlatAppearance = { BorderSize = 0 }
    };

    private async Task RefreshAll()
    {
        foreach (var a in arches) await Refresh(a);
    }

    private async Task Refresh(ArchData a)
    {
        string logPath = Path.Combine(ExeDir, a.Folder, "updates", "Version.log");
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
            var doc = XDocument.Parse(xml);
            var ns = XNamespace.Get("http://www.andymatuschak.org/xml-namespaces/sparkle");
            var enc = doc.Descendants("enclosure")
                .FirstOrDefault(e => e.Attribute("url")?.Value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true);

            string latest = enc?.Attribute(ns + "version")?.Value ?? "?";
            a.lblAvailable.Text = latest;

            if (installed == "Non installata")
            {
                a.btnAction.Text = "Installa";
                a.btnAction.BackColor = Color.FromArgb(70, 70, 70);
                a.btnAction.Enabled = true;
            }
            else if (installed == latest)
            {
                a.btnAction.Text = "Aggiornata";
                a.btnAction.BackColor = Color.ForestGreen;
                a.btnAction.Enabled = false;
            }
            else
            {
                a.btnAction.Text = "Aggiorna";
                a.btnAction.BackColor = Color.Crimson;
                a.btnAction.Enabled = true;
            }
        }
        catch
        {
            a.lblAvailable.Text = "Offline";
            a.btnAction.Text = "Offline";
            a.btnAction.BackColor = Color.OrangeRed;
        }
    }

    private async void StartUpdate(ArchData a)
    {
        a.btnAction.Enabled = false;
        a.btnAction.Text = "Avvio...";
        _cts = new CancellationTokenSource();
        ResetProgress();

        try
        {
            var xml = await _http.GetStringAsync(a.FeedUrl);
            var doc = XDocument.Parse(xml);
            var ns = XNamespace.Get("http://www.andymatuschak.org/xml-namespaces/sparkle");
            var enc = doc.Descendants("enclosure")
                .FirstOrDefault(e => e.Attribute("url")?.Value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true);

            if (enc == null) throw new Exception("Feed non valido");

            string version = enc.Attribute(ns + "version")!.Value;
            string url = enc.Attribute("url")!.Value;

            _downloadedFile = Path.Combine(ExeDir, $"Vivaldi-{version}-{a.ArchCode}.exe");

            lblStatus.Text = $"Download {a.Name} {version}...";
            await DownloadFileWithProgress(url, _downloadedFile);

            lblStatus.Text = "Estrazione e installazione...";
            await Task.Run(() => ExtractAndInstall(_downloadedFile, a.Folder, version, a.ArchCode));

            MessageBox.Show($"Vivaldi {a.Name} aggiornato alla versione {version}!", "Completato",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            CleanupDownloadedFile();
            await Refresh(a);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            a.btnAction.Text = "Errore";
            a.btnAction.BackColor = Color.IndianRed;
        }
        finally
        {
            a.btnAction.Enabled = true;
            ResetProgress();
            lblStatus.Text = "Pronto";
        }
    }

    private async Task DownloadFileWithProgress(string url, string path)
    {
        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, _cts!.Token);
        response.EnsureSuccessStatusCode();

        long total = response.Content.Headers.ContentLength ?? 0;
        long downloaded = 0;
        var sw = Stopwatch.StartNew();
        var buffer = new byte[16384];

        progressBar.Visible = true;
        progressBar.Value = 0;

        await using var stream = await response.Content.ReadAsStreamAsync(_cts.Token);
        await using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

        int read;
        while ((read = await stream.ReadAsync(buffer, _cts.Token)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read), _cts.Token);
            downloaded += read;

            if (sw.ElapsedMilliseconds >= 300)
            {
                sw.Restart();
                double percent = total > 0 ? downloaded * 100.0 / total : 0;
                double speed = downloaded / 1048576.0 / (Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency);
                double eta = total > 0 ? (total - downloaded) / (speed * 1048576) : 0;

                BeginInvoke(() =>
                {
                    progressBar.Value = (int)Math.Min(100, percent);
                    lblProgress.Text = $"{percent:F1}% ({downloaded / 1048576:F1} / {total / 1048576:F0} MB)";
                    lblSpeed.Text = $"Velocità: {speed:F2} MB/s";
                    lblEta.Text = eta > 0 ? $"ETA: {TimeSpan.FromSeconds(eta):mm\\:ss}" : "";
                });
            }
        }
    }

    private void ResetProgress()
    {
        BeginInvoke(() =>
        {
            progressBar.Visible = false;
            progressBar.Value = 0;
            lblProgress.Text = "";
            lblSpeed.Text = "";
            lblEta.Text = "";
        });
    }

    private void ExtractAndInstall(string installer, string targetFolder, string version, string arch)
    {
        if (!File.Exists(SevenZip)) throw new FileNotFoundException("7zr.exe non trovato in Bin\\");

        string temp = Path.Combine(Path.GetTempPath(), "VivaldiUpd_" + Guid.NewGuid().ToString("N")[..12]);
        Directory.CreateDirectory(temp);

        string target = Path.Combine(ExeDir, targetFolder);
        string updatesPath = Path.Combine(target, "updates");
        string profileFile = Path.Combine(target, "Profile.txt");
        string profileFolder = Path.Combine(target, "profile");

        string backup = Path.Combine(temp, "backup");
        Directory.CreateDirectory(backup);

        if (Directory.Exists(updatesPath)) CopyDir(updatesPath, Path.Combine(backup, "updates"));
        if (File.Exists(profileFile)) File.Copy(profileFile, Path.Combine(backup, "Profile.txt"), true);
        if (Directory.Exists(profileFolder)) CopyDir(profileFolder, Path.Combine(backup, "profile"));

        if (Directory.Exists(target))
        {
            foreach (var dir in Directory.GetDirectories(target, "*", SearchOption.AllDirectories).Reverse())
            {
                if (dir.EndsWith("\\updates", StringComparison.OrdinalIgnoreCase) ||
                    dir.EndsWith("\\profile", StringComparison.OrdinalIgnoreCase))
                    continue;
                try { Directory.Delete(dir, true); } catch { }
            }
            foreach (var file in Directory.GetFiles(target, "*", SearchOption.TopDirectoryOnly))
                if (!string.Equals(Path.GetFileName(file), "Profile.txt", StringComparison.OrdinalIgnoreCase))
                    try { File.Delete(file); } catch { }
        }
        else Directory.CreateDirectory(target);

        Run7z($"x \"{installer}\" vivaldi.7z -o\"{temp}\" -y");
        string v7z = Path.Combine(temp, "vivaldi.7z");
        if (!File.Exists(v7z)) throw new FileNotFoundException("vivaldi.7z non trovato");
        Run7z($"x \"{v7z}\" -o\"{temp}\" -y");

        string bin = Path.Combine(temp, "Vivaldi-bin");
        if (!Directory.Exists(bin)) throw new FileNotFoundException("Cartella Vivaldi-bin non trovata");

        foreach (string file in Directory.GetFiles(bin, "*", SearchOption.AllDirectories))
        {
            string rel = Path.GetRelativePath(bin, file);
            string dest = Path.Combine(target, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, true);
        }

        var buUpdates = Path.Combine(backup, "updates");
        if (Directory.Exists(buUpdates)) CopyDir(buUpdates, updatesPath, true);
        var buProfile = Path.Combine(backup, "Profile.txt");
        if (File.Exists(buProfile)) File.Copy(buProfile, profileFile, true);
        var buProfileFolder = Path.Combine(backup, "profile");
        if (Directory.Exists(buProfileFolder)) CopyDir(buProfileFolder, profileFolder, true);

        Directory.Delete(temp, true);

        Directory.CreateDirectory(updatesPath);
        File.WriteAllText(Path.Combine(updatesPath, "Version.log"), $"{version}|Stable|{arch}");
    }

    private void Run7z(string args)
    {
        var p = Process.Start(new ProcessStartInfo
        {
            FileName = SevenZip,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        p?.WaitForExit();
        if (p?.ExitCode != 0 && p?.ExitCode != null)
            throw new Exception($"7zr.exe errore: {p.ExitCode}");
    }

    private void CopyDir(string source, string dest, bool overwrite = true)
    {
        Directory.CreateDirectory(dest);
        foreach (var f in Directory.GetFiles(source))
            File.Copy(f, Path.Combine(dest, Path.GetFileName(f)), overwrite);
        foreach (var d in Directory.GetDirectories(source))
            CopyDir(d, Path.Combine(dest, Path.GetFileName(d)), overwrite); // CORRETTO: Path.GetFileName(d)
    }

    private void CleanupDownloadedFile()
    {
        if (chkDeleteTemp.Checked && _downloadedFile != null && File.Exists(_downloadedFile))
            try { File.Delete(_downloadedFile); } catch { }
    }
}