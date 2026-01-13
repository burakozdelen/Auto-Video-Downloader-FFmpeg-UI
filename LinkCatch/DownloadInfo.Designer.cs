namespace LinkCatch
{
    partial class DownloadInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadInfo));
            this.listViewResolutions = new System.Windows.Forms.ListView();
            this.columnResolution = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewDubs = new System.Windows.Forms.ListView();
            this.columnDubLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewSubtitles = new System.Windows.Forms.ListView();
            this.columnSubtitleLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonDownload = new System.Windows.Forms.Button();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // listViewResolutions
            // 
            this.listViewResolutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewResolutions.CheckBoxes = true;
            this.listViewResolutions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnResolution});
            this.listViewResolutions.FullRowSelect = true;
            this.listViewResolutions.GridLines = true;
            this.listViewResolutions.HideSelection = false;
            this.listViewResolutions.Location = new System.Drawing.Point(12, 12);
            this.listViewResolutions.Name = "listViewResolutions";
            this.listViewResolutions.Size = new System.Drawing.Size(760, 130);
            this.listViewResolutions.TabIndex = 0;
            this.listViewResolutions.UseCompatibleStateImageBehavior = false;
            this.listViewResolutions.View = System.Windows.Forms.View.Details;
            // 
            // columnResolution
            // 
            this.columnResolution.Text = "Resolution";
            this.columnResolution.Width = 202;
            // 
            // listViewDubs
            // 
            this.listViewDubs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewDubs.CheckBoxes = true;
            this.listViewDubs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDubLanguage});
            this.listViewDubs.FullRowSelect = true;
            this.listViewDubs.GridLines = true;
            this.listViewDubs.HideSelection = false;
            this.listViewDubs.Location = new System.Drawing.Point(12, 160);
            this.listViewDubs.Name = "listViewDubs";
            this.listViewDubs.Size = new System.Drawing.Size(360, 180);
            this.listViewDubs.TabIndex = 1;
            this.listViewDubs.UseCompatibleStateImageBehavior = false;
            this.listViewDubs.View = System.Windows.Forms.View.Details;
            // 
            // columnDubLanguage
            // 
            this.columnDubLanguage.Text = "Dub Language";
            this.columnDubLanguage.Width = 300;
            // 
            // listViewSubtitles
            // 
            this.listViewSubtitles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSubtitles.CheckBoxes = true;
            this.listViewSubtitles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSubtitleLanguage});
            this.listViewSubtitles.FullRowSelect = true;
            this.listViewSubtitles.GridLines = true;
            this.listViewSubtitles.HideSelection = false;
            this.listViewSubtitles.Location = new System.Drawing.Point(412, 160);
            this.listViewSubtitles.Name = "listViewSubtitles";
            this.listViewSubtitles.Size = new System.Drawing.Size(360, 180);
            this.listViewSubtitles.TabIndex = 2;
            this.listViewSubtitles.UseCompatibleStateImageBehavior = false;
            this.listViewSubtitles.View = System.Windows.Forms.View.Details;
            // 
            // columnSubtitleLanguage
            // 
            this.columnSubtitleLanguage.Text = "Subtitle Language";
            this.columnSubtitleLanguage.Width = 300;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDownload.Location = new System.Drawing.Point(12, 362);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(760, 32);
            this.buttonDownload.TabIndex = 3;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // rtbConsole
            // 
            this.rtbConsole.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rtbConsole.ForeColor = System.Drawing.Color.White;
            this.rtbConsole.Location = new System.Drawing.Point(13, 13);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbConsole.Size = new System.Drawing.Size(759, 327);
            this.rtbConsole.TabIndex = 4;
            this.rtbConsole.Text = "";
            this.rtbConsole.Visible = false;
            // 
            // DownloadInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 406);
            this.Controls.Add(this.rtbConsole);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.listViewSubtitles);
            this.Controls.Add(this.listViewDubs);
            this.Controls.Add(this.listViewResolutions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DownloadInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video Downloader";
            this.Load += new System.EventHandler(this.DownloadInfo_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewResolutions;
        private System.Windows.Forms.ColumnHeader columnResolution;
        private System.Windows.Forms.ListView listViewDubs;
        private System.Windows.Forms.ColumnHeader columnDubLanguage;
        private System.Windows.Forms.ListView listViewSubtitles;
        private System.Windows.Forms.ColumnHeader columnSubtitleLanguage;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.RichTextBox rtbConsole;
    }
}