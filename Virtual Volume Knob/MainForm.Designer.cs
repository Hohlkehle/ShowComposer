namespace QuirkSoft
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonVolUp = new System.Windows.Forms.Button();
            this.buttonVolDown = new System.Windows.Forms.Button();
            this.buttonMute = new System.Windows.Forms.Button();
            this.labelCursorPos = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.trackBarSens = new System.Windows.Forms.TrackBar();
            this.labelSensValue = new System.Windows.Forms.Label();
            this.radioButtonHorizontalAxis = new System.Windows.Forms.RadioButton();
            this.radioButtonVerticalAxis = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelShift = new System.Windows.Forms.Label();
            this.radioButtonModShift = new System.Windows.Forms.RadioButton();
            this.labelAlt = new System.Windows.Forms.Label();
            this.radioButtonModAlt = new System.Windows.Forms.RadioButton();
            this.labelCtrl = new System.Windows.Forms.Label();
            this.radioButtonModCtrl = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.labelVolumeLevel = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalProgressBar1 = new QuirkSoft.VerticalProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSens)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonVolUp
            // 
            this.buttonVolUp.Location = new System.Drawing.Point(12, 253);
            this.buttonVolUp.Name = "buttonVolUp";
            this.buttonVolUp.Size = new System.Drawing.Size(75, 23);
            this.buttonVolUp.TabIndex = 0;
            this.buttonVolUp.Text = "Vol Up";
            this.buttonVolUp.UseVisualStyleBackColor = true;
            this.buttonVolUp.Click += new System.EventHandler(this.ButtonVolUp_Click);
            // 
            // buttonVolDown
            // 
            this.buttonVolDown.Location = new System.Drawing.Point(12, 285);
            this.buttonVolDown.Name = "buttonVolDown";
            this.buttonVolDown.Size = new System.Drawing.Size(75, 23);
            this.buttonVolDown.TabIndex = 1;
            this.buttonVolDown.Text = "Vol Down";
            this.buttonVolDown.UseVisualStyleBackColor = true;
            this.buttonVolDown.Click += new System.EventHandler(this.ButtonVolDown_Click);
            // 
            // buttonMute
            // 
            this.buttonMute.Location = new System.Drawing.Point(12, 224);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(75, 23);
            this.buttonMute.TabIndex = 2;
            this.buttonMute.Text = "Mute";
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.ButtonMute_Click);
            // 
            // labelCursorPos
            // 
            this.labelCursorPos.AutoSize = true;
            this.labelCursorPos.Location = new System.Drawing.Point(87, 23);
            this.labelCursorPos.Name = "labelCursorPos";
            this.labelCursorPos.Size = new System.Drawing.Size(22, 13);
            this.labelCursorPos.TabIndex = 6;
            this.labelCursorPos.Text = "0:0";
            // 
            // timer1
            // 
            this.timer1.Interval = 6;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // trackBarSens
            // 
            this.trackBarSens.Location = new System.Drawing.Point(3, 19);
            this.trackBarSens.Maximum = 100;
            this.trackBarSens.Minimum = 1;
            this.trackBarSens.Name = "trackBarSens";
            this.trackBarSens.Size = new System.Drawing.Size(131, 45);
            this.trackBarSens.TabIndex = 8;
            this.trackBarSens.Value = 50;
            this.trackBarSens.Scroll += new System.EventHandler(this.TrackBarSens_Scroll);
            // 
            // labelSensValue
            // 
            this.labelSensValue.AutoSize = true;
            this.labelSensValue.Location = new System.Drawing.Point(60, 49);
            this.labelSensValue.Name = "labelSensValue";
            this.labelSensValue.Size = new System.Drawing.Size(19, 13);
            this.labelSensValue.TabIndex = 9;
            this.labelSensValue.Text = "50";
            // 
            // radioButtonHorizontalAxis
            // 
            this.radioButtonHorizontalAxis.AutoSize = true;
            this.radioButtonHorizontalAxis.Location = new System.Drawing.Point(6, 39);
            this.radioButtonHorizontalAxis.Name = "radioButtonHorizontalAxis";
            this.radioButtonHorizontalAxis.Size = new System.Drawing.Size(75, 17);
            this.radioButtonHorizontalAxis.TabIndex = 11;
            this.radioButtonHorizontalAxis.Text = "Horizontal ";
            this.radioButtonHorizontalAxis.UseVisualStyleBackColor = true;
            // 
            // radioButtonVerticalAxis
            // 
            this.radioButtonVerticalAxis.AutoSize = true;
            this.radioButtonVerticalAxis.Checked = true;
            this.radioButtonVerticalAxis.Location = new System.Drawing.Point(6, 19);
            this.radioButtonVerticalAxis.Name = "radioButtonVerticalAxis";
            this.radioButtonVerticalAxis.Size = new System.Drawing.Size(63, 17);
            this.radioButtonVerticalAxis.TabIndex = 12;
            this.radioButtonVerticalAxis.TabStop = true;
            this.radioButtonVerticalAxis.Text = "Vertical ";
            this.radioButtonVerticalAxis.UseVisualStyleBackColor = true;
            this.radioButtonVerticalAxis.CheckedChanged += new System.EventHandler(this.VerticalAxis_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonHorizontalAxis);
            this.groupBox1.Controls.Add(this.labelCursorPos);
            this.groupBox1.Controls.Add(this.radioButtonVerticalAxis);
            this.groupBox1.Location = new System.Drawing.Point(12, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 72);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStrip1.Size = new System.Drawing.Size(93, 26);
            // 
            // exitToolStripMenuItem2
            // 
            this.exitToolStripMenuItem2.Name = "exitToolStripMenuItem2";
            this.exitToolStripMenuItem2.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem2.Text = "Exit";
            this.exitToolStripMenuItem2.Click += new System.EventHandler(this.ExitToolStripMenuItem2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelShift);
            this.groupBox2.Controls.Add(this.radioButtonModShift);
            this.groupBox2.Controls.Add(this.labelAlt);
            this.groupBox2.Controls.Add(this.radioButtonModAlt);
            this.groupBox2.Controls.Add(this.labelCtrl);
            this.groupBox2.Controls.Add(this.radioButtonModCtrl);
            this.groupBox2.Location = new System.Drawing.Point(12, 156);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 62);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modifiers";
            // 
            // labelShift
            // 
            this.labelShift.AutoSize = true;
            this.labelShift.Location = new System.Drawing.Point(91, 35);
            this.labelShift.Name = "labelShift";
            this.labelShift.Size = new System.Drawing.Size(28, 13);
            this.labelShift.TabIndex = 5;
            this.labelShift.Text = "Shift";
            // 
            // radioButtonModShift
            // 
            this.radioButtonModShift.AutoSize = true;
            this.radioButtonModShift.Location = new System.Drawing.Point(96, 19);
            this.radioButtonModShift.Name = "radioButtonModShift";
            this.radioButtonModShift.Size = new System.Drawing.Size(14, 13);
            this.radioButtonModShift.TabIndex = 4;
            this.radioButtonModShift.UseVisualStyleBackColor = true;
            this.radioButtonModShift.CheckedChanged += new System.EventHandler(this.RadioButtonModShift_CheckedChanged);
            // 
            // labelAlt
            // 
            this.labelAlt.AutoSize = true;
            this.labelAlt.Location = new System.Drawing.Point(55, 35);
            this.labelAlt.Name = "labelAlt";
            this.labelAlt.Size = new System.Drawing.Size(19, 13);
            this.labelAlt.TabIndex = 3;
            this.labelAlt.Text = "Alt";
            // 
            // radioButtonModAlt
            // 
            this.radioButtonModAlt.AutoSize = true;
            this.radioButtonModAlt.Location = new System.Drawing.Point(60, 19);
            this.radioButtonModAlt.Name = "radioButtonModAlt";
            this.radioButtonModAlt.Size = new System.Drawing.Size(14, 13);
            this.radioButtonModAlt.TabIndex = 2;
            this.radioButtonModAlt.UseVisualStyleBackColor = true;
            this.radioButtonModAlt.CheckedChanged += new System.EventHandler(this.RadioButtonModAlt_CheckedChanged);
            // 
            // labelCtrl
            // 
            this.labelCtrl.AutoSize = true;
            this.labelCtrl.Location = new System.Drawing.Point(19, 35);
            this.labelCtrl.Name = "labelCtrl";
            this.labelCtrl.Size = new System.Drawing.Size(22, 13);
            this.labelCtrl.TabIndex = 1;
            this.labelCtrl.Text = "Ctrl";
            // 
            // radioButtonModCtrl
            // 
            this.radioButtonModCtrl.AutoSize = true;
            this.radioButtonModCtrl.Checked = true;
            this.radioButtonModCtrl.Location = new System.Drawing.Point(24, 19);
            this.radioButtonModCtrl.Name = "radioButtonModCtrl";
            this.radioButtonModCtrl.Size = new System.Drawing.Size(14, 13);
            this.radioButtonModCtrl.TabIndex = 0;
            this.radioButtonModCtrl.TabStop = true;
            this.radioButtonModCtrl.UseVisualStyleBackColor = true;
            this.radioButtonModCtrl.CheckedChanged += new System.EventHandler(this.RadioButtonModCtrl_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelSensValue);
            this.groupBox3.Controls.Add(this.trackBarSens);
            this.groupBox3.Location = new System.Drawing.Point(12, 30);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 66);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sensitivity";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.labelVolumeLevel);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.trackBarVolume);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.buttonClose);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.buttonMute);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.buttonVolDown);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.groupBox2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.buttonVolUp);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.groupBox1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.verticalProgressBar1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(183, 319);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(183, 343);
            this.toolStripContainer1.TabIndex = 17;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // labelVolumeLevel
            // 
            this.labelVolumeLevel.AutoSize = true;
            this.labelVolumeLevel.Location = new System.Drawing.Point(156, 201);
            this.labelVolumeLevel.Name = "labelVolumeLevel";
            this.labelVolumeLevel.Size = new System.Drawing.Size(19, 13);
            this.labelVolumeLevel.TabIndex = 10;
            this.labelVolumeLevel.Text = "50";
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Enabled = false;
            this.trackBarVolume.Location = new System.Drawing.Point(158, 6);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume.Size = new System.Drawing.Size(45, 198);
            this.trackBarVolume.TabIndex = 17;
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.Value = 50;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(102, 224);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 16;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(183, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.exitToolStripMenuItem.Text = "Pause";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.ExitToolStripMenuItem1_Click);
            // 
            // verticalProgressBar1
            // 
            this.verticalProgressBar1.Location = new System.Drawing.Point(153, 20);
            this.verticalProgressBar1.Name = "verticalProgressBar1";
            this.verticalProgressBar1.Size = new System.Drawing.Size(10, 175);
            this.verticalProgressBar1.Step = 1;
            this.verticalProgressBar1.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(183, 343);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.toolStripContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Volume Knob";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSens)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonVolUp;
        private System.Windows.Forms.Button buttonVolDown;
        private System.Windows.Forms.Button buttonMute;
        private System.Windows.Forms.Label labelCursorPos;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar trackBarSens;
        private System.Windows.Forms.Label labelSensValue;
        private System.Windows.Forms.RadioButton radioButtonHorizontalAxis;
        private System.Windows.Forms.RadioButton radioButtonVerticalAxis;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonModCtrl;
        private System.Windows.Forms.Label labelShift;
        private System.Windows.Forms.RadioButton radioButtonModShift;
        private System.Windows.Forms.Label labelAlt;
        private System.Windows.Forms.RadioButton radioButtonModAlt;
        private System.Windows.Forms.Label labelCtrl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private VerticalProgressBar verticalProgressBar1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem2;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelVolumeLevel;
    }
}

