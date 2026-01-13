using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LinkCatch.Main;
using static System.Collections.Specialized.BitVector32;

namespace LinkCatch
{
    public partial class DownloadInfo : Form
    {
        private Main _mainForm;
        public DownloadSession SessionData { get; set; }
        private Main.VideoDetailPacket _packet;
        public DownloadInfo(Main.VideoDetailPacket data, Main mainForm)
        {
            InitializeComponent();
            _packet = data;
            _mainForm = mainForm;
        }
        private void DownloadInfo_Load(object sender, EventArgs e)
        {
            if (SessionData != null)
            {
                // Add subtitles
                foreach (var s in SessionData.AllSubtitles)
                {
                    ListViewItem clonedItem = (ListViewItem)s.Clone();

                    // If the Tag is empty (Main.cs fills it, but just to be sure) accept the Text as a URL
                    if (clonedItem.Tag == null)
                    {
                        clonedItem.Tag = new VideoExtraData { MasterUrl = s.Text };
                    }

                    // We are shortening the text for display purposes, but the URL inside the Tag remains intact.
                    string fullUrl = (clonedItem.Tag as VideoExtraData).MasterUrl;
                    if (!string.IsNullOrEmpty(fullUrl) && fullUrl.Length > 30)
                    {
                        clonedItem.Text = "..." + fullUrl.Substring(fullUrl.Length - 30);
                    }
                    else
                    {
                        // Or create a new one and put it inside
                        clonedItem.Tag = new VideoExtraData { MasterUrl = fullUrl };
                    }

                    listViewSubtitles.Items.Add(clonedItem);
                }
            }

            foreach (var s in _packet._Subtitle)
            {
                // Name to appear in the list (e.g., English)
                ListViewItem item = new ListViewItem(s.Name);

                // CRITICAL POINT: We are storing the Full URL (s.Url) you created in Main.cs here.
                string urlToSave = string.IsNullOrEmpty(s.Url) ? s.Name : s.Url;

                item.Tag = new VideoExtraData
                {
                    MasterUrl = urlToSave, // Full URL goes here
                    Referrer = "", // Can be removed from the packet if necessary
                    Useragent = ""
                };

                listViewSubtitles.Items.Add(item);
            }
            foreach (var d in _packet._Dubbing)
            {
                listViewDubs.Items.Add(d.Name);
            }

            foreach (var r in _packet._Resolution)
            {
                listViewResolutions.Items.Add(r.Name);
            }
        }  
        public async Task PrepareExternalTools()
        {
            // File names
            string ffmpegFile = "ffmpeg.exe";
            string reFile = "N_m3u8DL-RE.exe";

            // Temporary zip files
            string ffmpegZip = "ffmpeg_temp.zip";
            string reZip = "re_temp.zip";

            // Download Links (If the links change over time, you can update them here)
            string ffmpegUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
            string reUrl = "https://github.com/nilaoda/N_m3u8DL-RE/releases/download/v0.5.1-beta/N_m3u8DL-RE_v0.5.1-beta_win-x64_20251029.zip";

            // 1. If both files already exist, do not perform the operation.
            if (File.Exists(ffmpegFile) && File.Exists(reFile)) return;

            // 2. Ask the user (Specify what is missing)
            string missingFiles = "";
            if (!File.Exists(ffmpegFile)) missingFiles += "FFmpeg\n";
            if (!File.Exists(reFile)) missingFiles += "N_m3u8DL-RE";

            DialogResult result = MessageBox.Show(
                $"The following required components are missing:\n\n{missingFiles}\nDo you want to download and install them automatically?",
                "Missing Components",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No) return;

            // --- CREATING A DYNAMIC DOWNLOAD WINDOW ---
            Form downloadForm = new Form();
            downloadForm.Text = "Downloading Tools...";
            downloadForm.Size = new Size(400, 150);
            downloadForm.StartPosition = FormStartPosition.CenterParent;
            downloadForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            downloadForm.MaximizeBox = false;
            downloadForm.MinimizeBox = false;

            Label lblStatus = new Label();
            lblStatus.Text = "Starting...";
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 20);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 50);
            progressBar.Size = new Size(340, 25);
            progressBar.Style = ProgressBarStyle.Continuous;

            downloadForm.Controls.Add(lblStatus);
            downloadForm.Controls.Add(progressBar);
            downloadForm.Show(); // Show form

            try
            {
                // Download FFmpeg if it is missing
                if (!File.Exists(ffmpegFile))
                {
                    downloadForm.Text = "Downloading FFmpeg...";
                    await DownloadAndExtractHelper(ffmpegUrl, ffmpegZip, ffmpegFile, lblStatus, progressBar);
                }

                // N_m3u8DL-RE Download if missing
                if (!File.Exists(reFile))
                {
                    // Reset the bar
                    progressBar.Value = 0;
                    downloadForm.Text = "Downloading N_m3u8DL-RE...";
                    await DownloadAndExtractHelper(reUrl, reZip, reFile, lblStatus, progressBar);
                }

                downloadForm.Close();
                MessageBox.Show("All components installed successfully!", "Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                downloadForm.Close();
                MessageBox.Show("An error occurred during download: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cleanup: Delete zip files
                if (File.Exists(ffmpegZip)) File.Delete(ffmpegZip);
                if (File.Exists(reZip)) File.Delete(reZip);
            }
        }

        // --- HELPER METHOD (To avoid code repetition) ---
        private async Task DownloadAndExtractHelper(string url, string zipPath, string targetExeName, Label lblStatus, ProgressBar progressBar)
        {
            using (HttpClient client = new HttpClient())
            {
                // Adding a User-Agent is sometimes required for GitHub downloads.
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;
                        bool isMoreToRead = true;

                        do
                        {
                            bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                isMoreToRead = false;
                                continue;
                            }
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            // UI Update
                            if (totalBytes.HasValue)
                            {
                                int percentage = (int)((totalRead * 100) / totalBytes.Value);
                                progressBar.Value = percentage;

                                double mbRead = (double)totalRead / 1024 / 1024;
                                double mbTotal = (double)totalBytes.Value / 1024 / 1024;
                                lblStatus.Text = $"Downloading {targetExeName}... {percentage}% - {mbRead:F2} MB / {mbTotal:F2} MB";
                            }
                            else
                            {
                                lblStatus.Text = $"Downloading {targetExeName}... {(totalRead / 1024 / 1024):F2} MB";
                            }

                            Application.DoEvents();
                        }
                        while (isMoreToRead);
                    }
                }
            }

            lblStatus.Text = $"Extracting {targetExeName}...";

            // Unzipping Process
            await Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    // Find the file name inside the zip file (it will find it even if it is inside a folder)
                    var targetEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(targetExeName, StringComparison.OrdinalIgnoreCase));

                    if (targetEntry != null)
                    {
                        // If an old file exists, allow overwriting it
                        if (File.Exists(targetExeName)) File.Delete(targetExeName);
                        targetEntry.ExtractToFile(targetExeName, true);
                    }
                    else
                    {
                        throw new FileNotFoundException($"The file '{targetExeName}' was not found inside the downloaded zip archive.");
                    }
                }
            });
        }
        private void AddLog(string text)
        {
            if (rtbConsole.InvokeRequired)
            {
                rtbConsole.Invoke(new Action(() => AddLog(text)));
            }
            else
            {
                // Timestamp and text
                rtbConsole.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
                rtbConsole.ScrollToCaret(); // Auto scroll to the last line
            }
        }
        private async Task StartDownloadProcess(string savePathBase)
        {
            // --- PREPARATION ---
            string nRePath = "N_m3u8DL-RE.exe";
            string ffmpegPath = "ffmpeg.exe";

            // We are parsing the folder path and file name
            string saveDir = Path.GetDirectoryName(savePathBase); // Folder
            string fileName = Path.GetFileName(savePathBase);     // File name (without extension)

            string rawVideoName = "raw_video"; // Name without extension (adds N_m3u8DL-RE to the end)

            string finalFileName = fileName;

            // Receive packet data
            var refInfo = _packet._ReferrerInfo.FirstOrDefault();
            string masterUrl = refInfo?.MasterUrl ?? "";
            string userAgent = refInfo?.Useragent != "Null" ? refInfo?.Useragent : "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
            string referer = refInfo?.Referrer != "Null" ? refInfo?.Referrer : "";

            AddLog($"--- PROCESS STARTED: {finalFileName} ---");

            // ========================================================================
            // PHASE 1: DOWNLOAD SUBTITLES (UPDATED AND LOGGED VERSION)
            // ========================================================================
            var downloadedSubtitles = new List<(string FilePath, string LangName)>();
            var selectedSubsItems = listViewSubtitles.CheckedItems.Cast<ListViewItem>().ToList();

            if (selectedSubsItems.Count > 0)
            {
                AddLog($"Found {selectedSubsItems.Count} selected subtitle(s). Checking URLs...");

                using (HttpClient subClient = new HttpClient())
                {
                    // Add the headers
                    subClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    if (!string.IsNullOrEmpty(referer)) subClient.DefaultRequestHeaders.Add("Referer", referer);

                    for (int i = 0; i < selectedSubsItems.Count; i++)
                    {
                        var item = selectedSubsItems[i];
                        string subUrl = "";
                        string langName = item.Text; // Like "English"

                        // 1. Try pulling the URL from the TAG
                        if (item.Tag is VideoExtraData data && !string.IsNullOrEmpty(data.MasterUrl))
                        {
                            subUrl = data.MasterUrl;
                        }
                        // 2. If the Tag is empty but the Text is a URL, use it
                        else if (item.Text.StartsWith("http"))
                        {
                            subUrl = item.Text;
                        }

                        // Debug Log: Let's see which URL we tried
                        if (string.IsNullOrEmpty(subUrl))
                        {
                            AddLog($"[SKIP] No URL found for subtitle: {langName}");
                            continue;
                        }

                        if (!subUrl.StartsWith("http"))
                        {
                            AddLog($"[SKIP] Invalid URL format for {langName}: {subUrl}");
                            continue;
                        }

                        // If everything is okay, try downloading
                        string extension = ".vtt"; // Default
                        if (subUrl.Contains(".srt")) extension = ".srt";

                        // Add a random number to the file name so it doesn't conflict
                        string tempSubPath = $"temp_sub_{i}_{DateTime.Now.Ticks}{extension}";

                        try
                        {
                            AddLog($"Downloading Sub: {langName}...");
                            byte[] subData = await subClient.GetByteArrayAsync(subUrl);

                            if (subData.Length > 0)
                            {
                                File.WriteAllBytes(tempSubPath, subData);
                                downloadedSubtitles.Add((tempSubPath, langName));
                                AddLog($"[OK] Subtitle saved: {langName}");
                            }
                            else
                            {
                                AddLog($"[FAIL] Subtitle file was empty: {langName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog($"[ERROR] Failed to download {langName}: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                AddLog("No subtitles selected to download.");
            }
            // Prepare the lists
            var selectedResolutions = listViewResolutions.CheckedItems.Cast<ListViewItem>().Select(i => i.Text).ToList();
            selectedResolutions.Sort((a, b) => { // Sort from largest to smallest
                return CalculatePixels(b).CompareTo(CalculatePixels(a));
            });


            var selectedDubs = listViewDubs.CheckedItems.Cast<ListViewItem>().Select(i => i.Text).ToList();

            // ========================================================================
            // PHASE 2: DOWNLOAD VIDEO & AUDIO 
            // ========================================================================
            AddLog("Starting video download process...");

            // 1. Determine the Link Type
            bool useFFmpeg = false;

            // RULE 1: If the link contains .txt, .m3u8, or .mpd, the Stream tool MUST be used.
            // (This rule should take precedence even if the link contains the word mp4)
            if (masterUrl.Contains(".m3u8") || masterUrl.Contains(".mpd") || masterUrl.Contains(".txt"))
            {
                useFFmpeg = false; // Use N_m3u8DL-RE 
            }
            else
            {
                // RULE 2: If it's not a stream, check if it has an extension like MP4, AVI, etc.
                string[] directVideoExtensions = { ".mp4", ".avi", ".mkv", ".webm", ".mov", ".flv", ".wmv", ".3gp" };
                foreach (var ext in directVideoExtensions)
                {
                    // Is there an extension?
                    if (masterUrl.IndexOf(ext, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        useFFmpeg = true; // Use FFmpeg
                        break;
                    }
                }
            }

            if (!useFFmpeg)
            {
                // --- SCENARIO A: STREAM (N_m3u8DL-RE) ---
                // This tool decodes the master.txt, .m3u8, and tampered .jpg segments.
                AddLog("Stream or List detected (m3u8/mpd/txt). Using N_m3u8DL-RE...");

                StringBuilder argsBuilder = new StringBuilder();
                argsBuilder.Append($"\"{masterUrl}\" ");
                argsBuilder.Append($"--header \"User-Agent: {userAgent}\" ");
                if (!string.IsNullOrEmpty(referer)) argsBuilder.Append($"--header \"Referer: {referer}\" ");

                argsBuilder.Append($"--save-dir \"{saveDir}\" "); // Download the video to the new folder we created
                argsBuilder.Append($"--save-name \"{rawVideoName}\" ");
                argsBuilder.Append("--mux-after-done mp4 ");

                // IMPORTANT ADDITION: Binary-merge may be required to download hidden segments such as .jpg.
                // However, N_m3u8DL-RE usually recognizes this automatically.
                // If necessary: argsBuilder.Append(“--binary-merge ”); can be added.

                // Resolution Selection
                if (selectedResolutions.Count > 0)
                {
                    string resPattern = string.Join("|", selectedResolutions);
                    argsBuilder.Append($"--select-video res=\"{resPattern}\":for=best{selectedResolutions.Count} ");
                }

                // Audio Selection
                if (selectedDubs.Count > 0)
                {
                    string audioPattern = string.Join("|", selectedDubs);
                    argsBuilder.Append($"-sa name=\"{audioPattern}\":for=best{selectedDubs.Count} ");
                }
                await RunExternalProcess(nRePath, argsBuilder.ToString());
            }
            else
            {
                // --- SCENARIO B: DIRECT MP4 ---
                AddLog("Direct static video file detected. Using FFmpeg...");

                StringBuilder ffmpegDownloadArgs = new StringBuilder();
                ffmpegDownloadArgs.Append("-y ");
                if (!string.IsNullOrEmpty(userAgent)) ffmpegDownloadArgs.Append($"-user_agent \"{userAgent}\" ");
                //The \r\n in the Headers section can sometimes cause problems.
                if (!string.IsNullOrEmpty(referer)) ffmpegDownloadArgs.Append($"-headers \"Referer: {referer}\" ");
                ffmpegDownloadArgs.Append($"-i \"{masterUrl}\" ");
                // We are telling it to save the file not just by name, but to the folder path you created (saveDir).
                string outputFullSavePath = Path.Combine(saveDir, $"{rawVideoName}.mp4");
                ffmpegDownloadArgs.Append($"-c copy \"{outputFullSavePath}\"");
                await RunExternalProcess(ffmpegPath, ffmpegDownloadArgs.ToString());
            }

            // ========================================================================
            // PHASE 3: MERGE AND TAG WITH FFMPEG
            // ========================================================================
            await Task.Delay(500);
            string downloadedVideoFile = Path.Combine(saveDir, $"{rawVideoName}.mp4");

            if (File.Exists(downloadedVideoFile))
            {
                AddLog("Main video file found. Starting Final Merge/Tagging...");
                StringBuilder ffmpegArgs = new StringBuilder();
                // 1. Input: Main Video
                ffmpegArgs.Append($"-y -i \"{downloadedVideoFile}\" ");

                // 2. Inputs: Downloaded Subtitles
                foreach (var sub in downloadedSubtitles)
                {
                    ffmpegArgs.Append($"-i \"{sub.FilePath}\" ");
                }

                // --- MAPPING & METADATA ---
                ffmpegArgs.Append("-map 0 ");  // Take everything from the video
                ffmpegArgs.Append("-c copy "); // Copy without converting (Quick)
                ffmpegArgs.Append("-c:s mov_text "); // MP4-compatible subtitle format

                // A) Video Tags (Only tags selected in Stream mode)
                if (!useFFmpeg && selectedResolutions.Count > 0)
                {
                    for (int i = 0; i < selectedResolutions.Count; i++)
                    {
                        // Note: Direct MP4 files usually contain a single video track; this loop only makes sense in Stream.
                        string resName = selectedResolutions[i];
                        ffmpegArgs.Append($"-metadata:s:v:{i} title=\"{resName}\" ");
                        ffmpegArgs.Append($"-metadata:s:v:{i} handler_name=\"{resName}\" ");
                    }
                }

                // B) Audio Tags
                if (!useFFmpeg && selectedDubs.Count > 0)
                {
                    for (int i = 0; i < selectedDubs.Count; i++)
                    {
                        string dubName = selectedDubs[i];
                        ffmpegArgs.Append($"-metadata:s:a:{i} title=\"{dubName}\" ");
                        ffmpegArgs.Append($"-metadata:s:a:{i} handler_name=\"{dubName}\" ");
                    }
                }

                // C) Subtitle Tags (from Phase 1)
                for (int i = 0; i < downloadedSubtitles.Count; i++)
                {
                    int inputIndex = i + 1; // 0 was the main video
                    string langName = downloadedSubtitles[i].LangName;
                    ffmpegArgs.Append($"-map {inputIndex} ");
                    ffmpegArgs.Append($"-metadata:s:s:{i} handler_name=\"{langName}\" ");
                    ffmpegArgs.Append($"-metadata:s:s:{i} title=\"{langName}\" ");
                }

                //ffmpegArgs.Append($“\”{finalFileName}.mp4\“”);
                string outputFullPath = Path.Combine(saveDir, finalFileName + ".mp4");
                ffmpegArgs.Append($"\"{outputFullPath}\"");
                await RunExternalProcess(ffmpegPath, ffmpegArgs.ToString());
                AddLog("Merging completed successfully!");

                // ========================================================================
                // PHASE 4: CLEANUP
                // ========================================================================
                try
                {
                    if (File.Exists(downloadedVideoFile)) File.Delete(downloadedVideoFile);
                    foreach (var sub in downloadedSubtitles)
                    {
                        if (File.Exists(sub.FilePath)) File.Delete(sub.FilePath);
                    }
                    AddLog("Temporary files cleaned up.");
                }
                catch { }

                MessageBox.Show($"Process Completed!\nFile: {finalFileName}.mp4", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                AddLog("CRITICAL ERROR: Raw video file not found. Download may have failed.");
                MessageBox.Show("Video download failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        private long CalculatePixels(string resName)
        {
            try
            {
                // Example: “1920x1080” -> Find 1920 and 1080
                var match = System.Text.RegularExpressions.Regex.Match(resName, @"(\d+)x(\d+)");
                if (match.Success)
                {
                    long width = long.Parse(match.Groups[1].Value);
                    long height = long.Parse(match.Groups[2].Value);
                    return width * height;
                }
            }
            catch { }
            return 0; // If there is an error, it will be moved to the end
        }
        private async Task RunExternalProcess(string fileName, string arguments)
        {
            AddLog($"Executing: {fileName}...");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = psi;
                proc.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AddLog(e.Data); };
                proc.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AddLog("LOG: " + e.Data); };

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                await Task.Run(() => proc.WaitForExit());
            }
        }
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (_packet == null || (listViewResolutions.Items.Count > 0 && listViewResolutions.CheckedItems.Count == 0))
            {
                MessageBox.Show("Please select a video quality to proceed.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                // 2. Creating a SaveFileDialog (File Save Dialog)
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Save Video";
                    sfd.Filter = "MP4 Video|*.mp4"; // We only allow MP4 files
                    sfd.FileName = "Video_Name";     // Default file name
                    sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Start on the desktop

                    // If the user clicks “Save”
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Selected full file path (e.g., C:\Users\User\Desktop\Movie.mp4)
                            string fullPath = sfd.FileName;

                            // Get the filename without the extension (e.g., Movie)
                            string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);

                            // Get the parent folder path (e.g., C:\Users\User\Desktop)
                            string parentFolder = System.IO.Path.GetDirectoryName(fullPath);

                            // Path of the new folder to be created (e.g., C:\Users\User\Desktop\Movie)
                            string newFolder = System.IO.Path.Combine(parentFolder, fileName);

                            // Create the folder (if it doesn't exist)
                            if (!System.IO.Directory.Exists(newFolder))
                            {
                                System.IO.Directory.CreateDirectory(newFolder);
                            }

                            // Base path for download (e.g., C:\Users\User\Desktop\Movie\Movie)
                            // We will add the extension (.mp4) in StartDownloadProcess.

                            string finalSaveBase = System.IO.Path.Combine(newFolder, fileName);

                            _mainForm?.StopProxy();

                            PrepareExternalTools();

                            listViewResolutions.Visible = false;
                            listViewDubs.Visible = false;
                            listViewSubtitles.Visible = false;
                            rtbConsole.Visible = true;
                            rtbConsole.Clear();
                            AddLog("External tools are ready. Initializing download process...");
                            StartDownloadProcess(finalSaveBase);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error creating folder: " + ex.Message);
                        }

                    }
                }
            }
        }
    }
}
