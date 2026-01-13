namespace LinkCatch
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Urlsearch = new System.Windows.Forms.Button();
            this.videoList = new System.Windows.Forms.ListView();
            this.videoUrlColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.videoTypeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nextBt = new System.Windows.Forms.Button();
            this.howtouseLinklabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Urlsearch
            // 
            this.Urlsearch.Location = new System.Drawing.Point(14, 12);
            this.Urlsearch.Name = "Urlsearch";
            this.Urlsearch.Size = new System.Drawing.Size(99, 27);
            this.Urlsearch.TabIndex = 0;
            this.Urlsearch.Text = "Start Search";
            this.Urlsearch.UseVisualStyleBackColor = true;
            this.Urlsearch.Click += new System.EventHandler(this.Urlsearch_Click);
            // 
            // videoList
            // 
            this.videoList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.videoUrlColumn,
            this.videoTypeColumn});
            this.videoList.FullRowSelect = true;
            this.videoList.GridLines = true;
            this.videoList.HideSelection = false;
            this.videoList.Location = new System.Drawing.Point(12, 54);
            this.videoList.MultiSelect = false;
            this.videoList.Name = "videoList";
            this.videoList.Size = new System.Drawing.Size(723, 140);
            this.videoList.TabIndex = 1;
            this.videoList.UseCompatibleStateImageBehavior = false;
            this.videoList.View = System.Windows.Forms.View.Details;
            // 
            // videoUrlColumn
            // 
            this.videoUrlColumn.Text = "Video List";
            this.videoUrlColumn.Width = 540;
            // 
            // videoTypeColumn
            // 
            this.videoTypeColumn.Text = "Video Type";
            this.videoTypeColumn.Width = 180;
            // 
            // nextBt
            // 
            this.nextBt.Location = new System.Drawing.Point(138, 12);
            this.nextBt.Name = "nextBt";
            this.nextBt.Size = new System.Drawing.Size(99, 27);
            this.nextBt.TabIndex = 3;
            this.nextBt.Text = "Next";
            this.nextBt.UseVisualStyleBackColor = true;
            this.nextBt.Visible = false;
            this.nextBt.Click += new System.EventHandler(this.nextBt_Click);
            // 
            // howtouseLinklabel
            // 
            this.howtouseLinklabel.AutoSize = true;
            this.howtouseLinklabel.Location = new System.Drawing.Point(665, 19);
            this.howtouseLinklabel.Name = "howtouseLinklabel";
            this.howtouseLinklabel.Size = new System.Drawing.Size(70, 13);
            this.howtouseLinklabel.TabIndex = 4;
            this.howtouseLinklabel.TabStop = true;
            this.howtouseLinklabel.Text = "How to use ?";
            this.howtouseLinklabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.howtouseLinklabel_LinkClicked);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 207);
            this.Controls.Add(this.howtouseLinklabel);
            this.Controls.Add(this.nextBt);
            this.Controls.Add(this.videoList);
            this.Controls.Add(this.Urlsearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video Downloader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Urlsearch;
        private System.Windows.Forms.ListView videoList;
        private System.Windows.Forms.ColumnHeader videoUrlColumn;
        private System.Windows.Forms.ColumnHeader videoTypeColumn;
        private System.Windows.Forms.Button nextBt;
        private System.Windows.Forms.LinkLabel howtouseLinklabel;
    }
}

