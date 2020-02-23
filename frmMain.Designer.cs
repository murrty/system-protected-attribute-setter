namespace SystemProtectedAttribute {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tdNotification = new Ookii.Dialogs.TaskDialog(this.components);
            this.HideFile = new Ookii.Dialogs.TaskDialogButton(this.components);
            this.HideFolder = new Ookii.Dialogs.TaskDialogButton(this.components);
            this.ShowFile = new Ookii.Dialogs.TaskDialogButton(this.components);
            this.ShowFolder = new Ookii.Dialogs.TaskDialogButton(this.components);
            this.Cancel = new Ookii.Dialogs.TaskDialogButton(this.components);
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.tvFiles = new ExplorerTreeview();
            this.SuspendLayout();
            // 
            // tdNotification
            // 
            this.tdNotification.Buttons.Add(this.HideFile);
            this.tdNotification.Buttons.Add(this.HideFolder);
            this.tdNotification.Buttons.Add(this.ShowFile);
            this.tdNotification.Buttons.Add(this.ShowFolder);
            this.tdNotification.Buttons.Add(this.Cancel);
            this.tdNotification.ButtonStyle = Ookii.Dialogs.TaskDialogButtonStyle.CommandLinksNoIcon;
            this.tdNotification.CollapsedControlText = "More info";
            this.tdNotification.Content = "This tool will flag programs as system protected.\r\nYou can still find this file b" +
    "y enabling \"View system protected files\"";
            this.tdNotification.ExpandedInformation = "Viewing system files can be enabled in the folder + search options of windows";
            this.tdNotification.Footer = "alpha";
            this.tdNotification.MainIcon = Ookii.Dialogs.TaskDialogIcon.Shield;
            this.tdNotification.MainInstruction = "Select a file to mark as system protected";
            this.tdNotification.HyperlinkClicked += new System.EventHandler<Ookii.Dialogs.HyperlinkClickedEventArgs>(this.tdNotification_HyperlinkClicked);
            // 
            // HideFile
            // 
            this.HideFile.Text = "Select file(s) to be flagged as system protected";
            // 
            // HideFolder
            // 
            this.HideFolder.Text = "Select folder(s) to be flagged as system protected";
            // 
            // ShowFile
            // 
            this.ShowFile.Text = "Unflag file(s) as system protected";
            // 
            // ShowFolder
            // 
            this.ShowFolder.Text = "Unflag folder(s) as system protected";
            // 
            // Cancel
            // 
            this.Cancel.ButtonType = Ookii.Dialogs.ButtonType.Cancel;
            this.Cancel.Default = true;
            this.Cancel.Text = "Cancel";
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Fuchsia;
            this.ilIcons.Images.SetKeyName(0, "intermediate.bmp");
            this.ilIcons.Images.SetKeyName(1, "chk.bmp");
            this.ilIcons.Images.SetKeyName(2, "x.bmp");
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(367, 295);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.Location = new System.Drawing.Point(12, 295);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(349, 24);
            this.lbStatus.TabIndex = 2;
            this.lbStatus.Text = "Idle";
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tvFiles
            // 
            this.tvFiles.FullRowSelect = true;
            this.tvFiles.HotTracking = true;
            this.tvFiles.ImageIndex = 0;
            this.tvFiles.ImageList = this.ilIcons;
            this.tvFiles.Location = new System.Drawing.Point(12, 12);
            this.tvFiles.Name = "tvFiles";
            this.tvFiles.SelectedImageIndex = 0;
            this.tvFiles.ShowLines = false;
            this.tvFiles.Size = new System.Drawing.Size(430, 273);
            this.tvFiles.TabIndex = 0;
            this.tvFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFiles_AfterSelect);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(454, 331);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tvFiles);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Flagging status";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private Ookii.Dialogs.TaskDialog tdNotification;
        private Ookii.Dialogs.TaskDialogButton HideFile;
        private Ookii.Dialogs.TaskDialogButton HideFolder;
        private Ookii.Dialogs.TaskDialogButton Cancel;
        private Ookii.Dialogs.TaskDialogButton ShowFile;
        private Ookii.Dialogs.TaskDialogButton ShowFolder;
        private ExplorerTreeview tvFiles;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbStatus;

    }
}

