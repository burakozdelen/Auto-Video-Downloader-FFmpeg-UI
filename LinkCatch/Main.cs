using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using static LinkCatch.Main;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace LinkCatch
{
    public partial class Main : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private ProxyServer _proxyServer;
        private ExplicitProxyEndPoint _explicitEndPoint;
        private readonly HashSet<string> _capturedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lockObject = new object();
        private List<ListViewItem> _backgroundSubtitles = new List<ListViewItem>(); //We will use this to transfer the subtitle URLs to DownloadInfo
        public Main()
        {
            InitializeComponent();
            // To stop the proxy when the application closes
            this.FormClosing += Main_FormClosing;
        }
        public class VideoExtraData
        {
            public string MasterUrl { get; set; }
            public string Referrer { get; set; }
            public string Useragent { get; set; }
            public string Origin { get; set; }
        }

        public class VideoDetail
        {
            public string Url { get; set; }
            public string Name { get; set; }   
        }

        public class VideoDetailPacket
        {
            public List<VideoDetail> _Dubbing { get; set; } = new List<VideoDetail>();
            public List<VideoDetail> _Resolution { get; set; } = new List<VideoDetail>();
            public List<VideoDetail> _Subtitle { get; set; } = new List<VideoDetail>();
            public List<VideoDetail> _Subtitle_extra { get; set; } = new List<VideoDetail>();
            public List<VideoExtraData> _ReferrerInfo { get; set; } = new List<VideoExtraData>();
        }
        public class DownloadSession
        {
            public List<ListViewItem> AllSubtitles { get; set; } = new List<ListViewItem>();
        }
        // URL Merge Assistant
        private string BuildUrl(string masterUrl, string relativeUrl)
        {
            // 1. If it already starts with http/https (Absolute URL), do not touch it
            if (relativeUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return relativeUrl;
            }

            // 2. If it starts with / (Root Relative -> https://site.com/dir/master.m3u8 + /new/file.ts)
            if (relativeUrl.StartsWith("/"))
            {
                // Takes the domain (host) of the Master URL and appends it to the end
                Uri baseUri = new Uri(masterUrl);
                return new Uri(baseUri, relativeUrl).AbsoluteUri;
            }
            // 3. If it does not start with / (Sibling Relative -> sublist.m3u8)
            else
            {
                // Deletes the file name at the end of the Master URL (master.txt) and adds it to the folder it is located in.
                int lastSlashIndex = masterUrl.LastIndexOf('/');
                if (lastSlashIndex != -1)
                {
                    string baseDirectory = masterUrl.Substring(0, lastSlashIndex + 1); // Including slash
                    return baseDirectory + relativeUrl;
                }
                else
                {
                    // Rare case: If there is no slash in the URL, add it directly
                    return masterUrl + "/" + relativeUrl;
                }
            }
        }
        public async Task<VideoDetailPacket> _regex(VideoExtraData data)
        {
            string m_url = data.MasterUrl;

            VideoDetailPacket videoDetailPacket = new VideoDetailPacket();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, m_url);

                // Add referer if available
                if (!string.IsNullOrEmpty(data.Referrer) && data.Referrer != "Null")
                {
                    request.Headers.TryAddWithoutValidation("Referer", data.Referrer);
                }

                // Add User-Agent if present
                if (!string.IsNullOrEmpty(data.Useragent) && data.Useragent != "Null")
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", data.Useragent);
                }

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); // If there is an error, it will fall into the catch block
                string content = await response.Content.ReadAsStringAsync();

                string dubbingPattern = @"TYPE=AUDIO.*?NAME=""(?<language>[^""]+)"".*?URI=""(?<link>[^""]+)""";
                string resPattern = @"#EXT-X-STREAM-INF:.*?RESOLUTION=(?<res>\d+x\d+).*?[\r\n]+(?<link>[^\r\n#]+)";
                string subtitlePattern = @"TYPE=SUBTITLES.*?NAME=""(?<language>[^""]+)"".*?URI=""(?<link>[^""]+)""";

                MatchCollection dubbingPattern_m = Regex.Matches(content, dubbingPattern);
                MatchCollection resPattern_m = Regex.Matches(content, resPattern);
                MatchCollection subtitlePattern_m = Regex.Matches(content, subtitlePattern);

                if (dubbingPattern_m.Count > 0)
                {
                    foreach (Match m in dubbingPattern_m)
                    {
                        string dub_d = m.Groups["language"].Value;
                        string link_d = m.Groups["link"].Value;
                        string fullUrl_d = BuildUrl(m_url, link_d);
                        videoDetailPacket._Dubbing.Add(new VideoDetail
                        {
                            Name = dub_d,
                            Url = fullUrl_d
                        });
                    }
                }

                if (resPattern_m.Count > 0)
                {
                    foreach (Match m in resPattern_m)
                    {
                        string res_r = m.Groups["res"].Value;
                        string link_r = m.Groups["link"].Value;
                        string fullUrl_r = BuildUrl(m_url, link_r);

                        videoDetailPacket._Resolution.Add(new VideoDetail
                        {
                            Name = res_r,
                            Url = fullUrl_r
                        });
                    }
                }

                if (subtitlePattern_m.Count > 0)
                {
                    foreach (Match m in subtitlePattern_m)
                    {
                        string sub_s = m.Groups["language"].Value;
                        string link_s = m.Groups["link"].Value;
                        string fullUrl_s = BuildUrl(m_url, link_s);

                        videoDetailPacket._Subtitle.Add(new VideoDetail
                        {
                            Name = sub_s,
                            Url = fullUrl_s
                        });
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Internet or Server Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            return videoDetailPacket;
        }
        private void Urlsearch_Click(object sender, EventArgs e)
        {
            if (_proxyServer != null && _proxyServer.ProxyRunning) return;
            StartSniffing();
            MessageBox.Show("Open the video in your browser");
            nextBt.Visible = true;
        }
        private void StartSniffing()
        {
            _proxyServer = new ProxyServer();
            // Certificate settings for decrypting HTTPS traffic
            _proxyServer.CertificateManager.CreateRootCertificate();
            _proxyServer.CertificateManager.TrustRootCertificate(true);
            _proxyServer.BeforeResponse += OnResponse;
            _explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);
            _proxyServer.AddEndPoint(_explicitEndPoint);
            _proxyServer.Start();
            // Redirect the system proxy to our application
            _proxyServer.SetAsSystemProxy(_explicitEndPoint, ProxyProtocolType.AllHttp);
        }
        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            // 1. URL and Header Preparation
            var responseHeaders = e.HttpClient.Response.Headers;
            var contentTypeHeader = responseHeaders.FirstOrDefault(h => h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));

            string contentType = (contentTypeHeader?.Value ?? "").ToLowerInvariant();
            string url = e.HttpClient.Request.Url.ToString().ToLowerInvariant();

            // ==============================================================================
            // A. TRASH FILTER (Discard unnecessary files)
            // ==============================================================================
            if (url.Contains("google") || url.Contains("facebook") || url.Contains("analytics") ||
                url.Contains("doubleclick") || url.Contains("fonts.") || url.Contains("favicon") ||
                url.EndsWith(".js") || url.EndsWith(".css") || url.EndsWith(".png") ||
                url.EndsWith(".jpg") || url.EndsWith(".jpeg") || url.EndsWith(".gif") ||
                url.EndsWith(".ico") || url.EndsWith(".svg") || url.EndsWith(".woff") || url.EndsWith(".woff2") ||
                contentType.Contains("javascript") || contentType.Contains("font"))
            {
                return;
            }

            // ==============================================================================
            // DETECTION LOGIC (PRIORITY ORDER IS IMPORTANT)
            // ==============================================================================
            bool isSubtitle = false;
            bool isVideo = false;

            // STEP 1: SUBTITLE CHECK FIRST (We trust the URL regardless of Content-Type)
            // If URL contains or ends with .vtt/.srt, it is definitely a subtitle.
            if (url.EndsWith(".vtt") || url.Contains(".vtt?") ||
                url.EndsWith(".srt") || url.Contains(".srt?") ||
                url.EndsWith(".ass") || url.EndsWith(".dfxp"))
            {
                isSubtitle = true;
            }
            // If we can't tell from the URL, check Content-Type
            else if (contentType.Contains("text/vtt") ||
                     contentType.Contains("application/x-subrip") ||
                     contentType.Contains("application/ttml+xml"))
            {
                isSubtitle = true;
            }
            // If URL contains "subtitle" keyword and is not image/html
            else if ((url.Contains("subtitle") || url.Contains("caption") || url.Contains("cc_")) &&
                     (!contentType.Contains("video") && !contentType.Contains("html") && !contentType.Contains("image")))
            {
                isSubtitle = true;
            }

            // STEP 2: CHECK FOR VIDEO ONLY IF IT IS NOT A SUBTITLE
            if (!isSubtitle)
            {
                // Standard Video Types
                if (contentType.Contains("application/vnd.apple.mpegurl") ||
                    contentType.Contains("application/x-mpegurl") ||
                    contentType.Contains("application/dash+xml") ||
                    contentType.Contains("video/"))
                {
                    isVideo = true;
                }
                // Extension Check
                else if (url.Contains(".mp4") || url.Contains(".m3u8") || url.Contains(".ts") || url.Contains(".mkv"))
                {
                    if (!contentType.Contains("html")) isVideo = true;
                }
                // UNKNOWN FILES (Octet-Stream)
                // This part was catching .vtt files shown in your screenshot as "application/octet-stream" 
                // and treating them as video. Now that we check "if (!isSubtitle)" above, subtitles will never enter here.
                else if (contentType.Contains("application/octet-stream") ||
                         contentType.Contains("binary/octet-stream"))
                {
                    // Exclude system files
                    if (!url.EndsWith(".dll") && !url.EndsWith(".exe") && !url.EndsWith(".zip") &&
                        !url.EndsWith(".pdf") && !url.EndsWith(".json") && !url.EndsWith(".key"))
                    {
                        isVideo = true;
                    }
                }
            }
            // ==============================================================================
            // D. ADDING TO LIST PROCESS
            // ==============================================================================
            if (isVideo || isSubtitle)
            {
                lock (_lockObject)
                {
                    if (_capturedUrls.Contains(e.HttpClient.Request.Url)) return;
                    _capturedUrls.Add(e.HttpClient.Request.Url);
                }

                var requestHeaders = e.HttpClient.Request.Headers;
                string referer = requestHeaders.FirstOrDefault(h => h.Name.Equals("Referer", StringComparison.OrdinalIgnoreCase))?.Value ?? "Null";
                string useragent = requestHeaders.FirstOrDefault(h => h.Name.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))?.Value ?? "Null";
                string origin = requestHeaders.FirstOrDefault(h => h.Name.Equals("Origin", StringComparison.OrdinalIgnoreCase))?.Value ?? "Null";

                if (this.IsDisposed) return;

                this.Invoke(new MethodInvoker(delegate
                {
                    string fullUrl = e.HttpClient.Request.Url.ToString();
                    string displayUrl = fullUrl.Length > 60 ? "..." + fullUrl.Substring(fullUrl.Length - 60) : fullUrl;

                    ListViewItem item = new ListViewItem(displayUrl);
                    item.SubItems.Add(contentType);

                    item.Tag = new VideoExtraData
                    {
                        MasterUrl = fullUrl,
                        Referrer = referer,
                        Useragent = useragent,
                        Origin = origin
                    };

                    if (isVideo)
                    {
                        // If VIDEO, add to screen list
                        videoList.Items.Add(item);
                    }
                    else if (isSubtitle)
                    {
                        // If SUBTITLE, just add to background (Not visible on screen, appears when Next is clicked)
                        lock (_backgroundSubtitles)
                        {
                            _backgroundSubtitles.Add(item);
                        }
                    }
                }));
            }
        }
        public void StopProxy()
        {
            if (_proxyServer != null)
            {
                // 1. First, unsubscribe from the Event (Prevents memory leaks)
                _proxyServer.BeforeResponse -= OnResponse;

                // 2. If the proxy is working, restore the system settings
                if (_proxyServer.ProxyRunning)
                {
                    _proxyServer.DisableSystemProxy(Titanium.Web.Proxy.Models.ProxyProtocolType.AllHttp);
                    _proxyServer.Stop();
                }

                // 3. Completely destroy the object (Dispose)
                _proxyServer.Dispose();
                _proxyServer = null;
            }
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                var openDownloads = Application.OpenForms.OfType<DownloadInfo>().ToList();

                foreach (var dwForm in openDownloads)
                {
                    // Kill FFmpeg
                    dwForm.ForceKill();
                    dwForm.Close();
                }
                StopProxy();
            }
            catch{}
            finally
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private async void nextBt_Click(object sender, EventArgs e)
        {
            if (videoList.SelectedItems.Count > 0)
            {
                VideoDetailPacket videoDetailPacket = new VideoDetailPacket();               
                DownloadSession session = new DownloadSession();

                // At least one box is checked!
                foreach (ListViewItem checkItem in videoList.SelectedItems)
                {
                    if (checkItem.Tag is VideoExtraData data)
                    {
                        videoDetailPacket._ReferrerInfo.Add(data);

                        VideoDetailPacket in_data = await _regex(data);
                        if (in_data != null)
                        {
                            // We are merging the lists (Merge operation)
                            videoDetailPacket._Dubbing.AddRange(in_data._Dubbing);
                            videoDetailPacket._Resolution.AddRange(in_data._Resolution);
                            videoDetailPacket._Subtitle.AddRange(in_data._Subtitle);
                        }
                    }
                    // Only those who have checked the box will enter here
                    string url = checkItem.Text;
                    string tur = checkItem.SubItems[1].Text; // Assuming you wrote the type in the second column;
                }
                lock (_backgroundSubtitles)
                {
                    foreach (ListViewItem subItem in _backgroundSubtitles)
                    {
                        session.AllSubtitles.Add((ListViewItem)subItem.Clone());
                        string subtitle_full_url = "";
                        if (subItem.Tag is VideoExtraData data)
                            subtitle_full_url = data.MasterUrl;

                        else
                            subtitle_full_url = subItem.Text;

                        videoDetailPacket._Subtitle_extra.Add(new VideoDetail 
                        { 
                            Name = subItem.SubItems[0].Text,
                            Url = subtitle_full_url
                        });
                    }

                }     
                DownloadInfo dwInfo = new DownloadInfo(videoDetailPacket, this);
                dwInfo.SessionData = session;
                dwInfo.Show();
            }
            else
            {
                // No selected.
                MessageBox.Show("Please select a video from the list first.");
            }
        }
        private void ShowHelpGuide()
        {
            Form helpForm = new Form();
            helpForm.Text = "Video Downlaoder - User Guide & Tips";
            helpForm.Size = new Size(600, 620);
            helpForm.StartPosition = FormStartPosition.CenterParent;
            helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            helpForm.MaximizeBox = false;
            helpForm.MinimizeBox = false;

            RichTextBox rtbHelp = new RichTextBox();
            rtbHelp.Dock = DockStyle.Fill;
            rtbHelp.ReadOnly = true;
            rtbHelp.BackColor = Color.White;
            rtbHelp.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbHelp.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            rtbHelp.SelectionIndent = 10;
            rtbHelp.SelectionRightIndent = 10;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=== HOW TO USE Video Downlaoder ===");
            sb.AppendLine("");
            sb.AppendLine("1. INITIAL SETUP & CERTIFICATES");
            sb.AppendLine("- When you start the 'Start Search' mode for the first time, the app may ask you to install a 'Root Certificate' to read HTTPS traffic. You must approve this.");
            sb.AppendLine("- During the download phase, if FFmpeg and N_m3u8DL-RE tools are missing, they will be downloaded automatically.");
            sb.AppendLine("- IMPORTANT: After these tools are automatically installed and you see the 'Success' message, you must click the 'Download' button AGAIN to start the actual process.");
            sb.AppendLine("");
            sb.AppendLine("2. CAPTURING THE RIGHT LINK (IMPORTANT!)");
            sb.AppendLine("- CLOSE OTHER TABS in your browser. Keep only the video tab open.");
            sb.AppendLine("  (Otherwise, background requests from other tabs will clutter the list.)");
            sb.AppendLine("- Click the 'Start Search' button and then play the video.");
            sb.AppendLine("- You generally need to find the 'Master URL' in the list.");
            sb.AppendLine("- Master URL Tips:");
            sb.AppendLine("  * If a link contains the word 'master', that is likely the correct one.");
            sb.AppendLine("  * If other links end with numbers like 1080, 720, but one does NOT, that one is the 'Master'.");
            sb.AppendLine("  * A Master URL might not always end in '.m3u8'; it could be '.txt' or have a complex extension.");
            sb.AppendLine("");
            sb.AppendLine("3. DOWNLOAD OPTIONS");
            sb.AppendLine("- You can only start downloading ONE video at a time.");
            sb.AppendLine("- However, you can select all resolutions, audio tracks, and subtitles *within* that single video.");
            sb.AppendLine("- Subtitles: If you see multiple files for the same language (e.g., multiple 'eng.vtt'), SELECT ALL OF THEM. Some might be empty or broken; you can verify them after downloading.");
            sb.AppendLine("");
            sb.AppendLine("4. UNSUPPORTED PLATFORMS & LINKS");
            sb.AppendLine("- DRM Protected Sites: Platforms like YouTube, Netflix, Disney+, Amazon Prime, etc., use encryption and CANNOT be downloaded with this tool.");
            sb.AppendLine("- Live & Dynamic Streams: Some m3u8 links send data second-by-second or use dynamic keys.");
            sb.AppendLine("  * If a download finishes instantly but you only get 3-5 seconds of footage, this link type is not supported.");
            sb.AppendLine("  * This is not a bug; it is due to the stream's protection or delivery method.");
            sb.AppendLine("");
            sb.AppendLine("5. TROUBLESHOOTING (NO LINKS FOUND)");
            sb.AppendLine("- If the video is playing but no Master URL appears in the list:");
            sb.AppendLine("  A) Close the browser tab.");
            sb.AppendLine("  B) Clear Cache and Force Refresh (usually CTRL + F5) and try again.");
            sb.AppendLine("");
            sb.AppendLine("6. PLAYBACK RECOMMENDATION");
            sb.AppendLine("- We strongly recommend using VLC Media Player to watch the downloaded videos.");
            sb.AppendLine("- Standard players (like Windows Media Player) often fail to detect subtitles or switch between audio tracks correctly.");
            sb.AppendLine("");
            sb.AppendLine("!!! CRITICAL WARNING: IF THE APP CRASHES !!!");
            sb.AppendLine("- This app runs a 'Proxy Server' in the background.");
            sb.AppendLine("- If the app crashes unexpectedly or is force-closed via Task Manager, Windows Proxy settings may remain active, cutting off your internet access.");
            sb.AppendLine("- SOLUTION: Go to Windows Settings > Network & Internet > Proxy and turn 'Use a proxy server' to OFF.");
            sb.AppendLine("");

            rtbHelp.Text = sb.ToString();

            HighlightText(rtbHelp, "=== HOW TO USE Video Downlaoder ===", FontStyle.Bold);
            HighlightText(rtbHelp, "!!! CRITICAL WARNING: IF THE APP CRASHES !!!", FontStyle.Bold);
            HighlightText(rtbHelp, "CLOSE OTHER TABS", FontStyle.Bold);
            HighlightText(rtbHelp, "4. UNSUPPORTED PLATFORMS & LINKS", FontStyle.Bold);
            HighlightText(rtbHelp, "click the 'Download' button AGAIN", FontStyle.Bold);
            HighlightText(rtbHelp, "VLC Media Player", FontStyle.Bold);

            helpForm.Controls.Add(rtbHelp);
            helpForm.ShowDialog();
        }

        // Helper method remains the same
        private void HighlightText(RichTextBox rtb, string textToHighlight, FontStyle style)
        {
            int index = rtb.Text.IndexOf(textToHighlight);
            if (index != -1)
            {
                rtb.Select(index, textToHighlight.Length);
                rtb.SelectionFont = new Font(rtb.Font, style);
                rtb.SelectionColor = Color.DarkRed;
            }
        }

        private void howtouseLinklabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowHelpGuide();
        }
    }
    
}
