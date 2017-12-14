namespace QuirkSoft
{
    partial class VolumeController
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelVolumeLevel = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.buttonMute = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelShift = new System.Windows.Forms.Label();
            this.radioButtonModShift = new System.Windows.Forms.RadioButton();
            this.labelAlt = new System.Windows.Forms.Label();
            this.radioButtonModAlt = new System.Windows.Forms.RadioButton();
            this.labelCtrl = new System.Windows.Forms.Label();
            this.radioButtonModCtrl = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorizontalAxis = new System.Windows.Forms.RadioButton();
            this.radioButtonVerticalAxis = new System.Windows.Forms.RadioButton();
            this.labelCursorPos = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelSensValue = new System.Windows.Forms.Label();
            this.trackBarSens = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.verticalProgressBar1 = new QuirkSoft.VerticalProgressBar();
            this.colorSlider1 = new ColorSlider.ColorSlider();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSens)).BeginInit();
            this.SuspendLayout();
            // 
            // labelVolumeLevel
            // 
            this.labelVolumeLevel.AutoSize = true;
            this.labelVolumeLevel.Location = new System.Drawing.Point(225, 199);
            this.labelVolumeLevel.Name = "labelVolumeLevel";
            this.labelVolumeLevel.Size = new System.Drawing.Size(19, 13);
            this.labelVolumeLevel.TabIndex = 19;
            this.labelVolumeLevel.Text = "50";
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(228, 3);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume.Size = new System.Drawing.Size(45, 198);
            this.trackBarVolume.TabIndex = 25;
            this.trackBarVolume.TabStop = false;
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.Value = 50;
            // 
            // buttonMute
            // 
            this.buttonMute.Location = new System.Drawing.Point(107, 219);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(115, 23);
            this.buttonMute.TabIndex = 18;
            this.buttonMute.TabStop = false;
            this.buttonMute.Text = "Mute";
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelShift);
            this.groupBox2.Controls.Add(this.radioButtonModShift);
            this.groupBox2.Controls.Add(this.labelAlt);
            this.groupBox2.Controls.Add(this.radioButtonModAlt);
            this.groupBox2.Controls.Add(this.labelCtrl);
            this.groupBox2.Controls.Add(this.radioButtonModCtrl);
            this.groupBox2.Location = new System.Drawing.Point(73, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(149, 62);
            this.groupBox2.TabIndex = 21;
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
            this.radioButtonModShift.Checked = true;
            this.radioButtonModShift.Location = new System.Drawing.Point(96, 19);
            this.radioButtonModShift.Name = "radioButtonModShift";
            this.radioButtonModShift.Size = new System.Drawing.Size(14, 13);
            this.radioButtonModShift.TabIndex = 4;
            this.radioButtonModShift.TabStop = true;
            this.radioButtonModShift.UseVisualStyleBackColor = true;
            this.radioButtonModShift.CheckedChanged += new System.EventHandler(this.radioButtonModShift_CheckedChanged);
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
            this.radioButtonModAlt.CheckedChanged += new System.EventHandler(this.radioButtonModAlt_CheckedChanged);
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
            this.radioButtonModCtrl.Location = new System.Drawing.Point(24, 19);
            this.radioButtonModCtrl.Name = "radioButtonModCtrl";
            this.radioButtonModCtrl.Size = new System.Drawing.Size(14, 13);
            this.radioButtonModCtrl.TabIndex = 0;
            this.radioButtonModCtrl.UseVisualStyleBackColor = true;
            this.radioButtonModCtrl.CheckedChanged += new System.EventHandler(this.radioButtonModCtrl_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonHorizontalAxis);
            this.groupBox1.Controls.Add(this.radioButtonVerticalAxis);
            this.groupBox1.Location = new System.Drawing.Point(73, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(146, 72);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis";
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
            // labelCursorPos
            // 
            this.labelCursorPos.AutoSize = true;
            this.labelCursorPos.Location = new System.Drawing.Point(76, 216);
            this.labelCursorPos.Name = "labelCursorPos";
            this.labelCursorPos.Size = new System.Drawing.Size(22, 13);
            this.labelCursorPos.TabIndex = 6;
            this.labelCursorPos.Text = "0:0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelSensValue);
            this.groupBox3.Controls.Add(this.trackBarSens);
            this.groupBox3.Location = new System.Drawing.Point(73, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(149, 66);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sensitivity";
            // 
            // labelSensValue
            // 
            this.labelSensValue.AutoSize = true;
            this.labelSensValue.Location = new System.Drawing.Point(60, 49);
            this.labelSensValue.Name = "labelSensValue";
            this.labelSensValue.Size = new System.Drawing.Size(19, 13);
            this.labelSensValue.TabIndex = 9;
            this.labelSensValue.Text = "20";
            // 
            // trackBarSens
            // 
            this.trackBarSens.Location = new System.Drawing.Point(3, 19);
            this.trackBarSens.Maximum = 100;
            this.trackBarSens.Minimum = 1;
            this.trackBarSens.Name = "trackBarSens";
            this.trackBarSens.Size = new System.Drawing.Size(140, 45);
            this.trackBarSens.TabIndex = 8;
            this.trackBarSens.TabStop = false;
            this.trackBarSens.Value = 20;
            this.trackBarSens.Scroll += new System.EventHandler(this.trackBarSens_Scroll);
            // 
            // timer1
            // 
            this.timer1.Interval = 6;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // verticalProgressBar1
            // 
            this.verticalProgressBar1.Location = new System.Drawing.Point(3, 3);
            this.verticalProgressBar1.Name = "verticalProgressBar1";
            this.verticalProgressBar1.Size = new System.Drawing.Size(10, 238);
            this.verticalProgressBar1.Step = 1;
            this.verticalProgressBar1.TabIndex = 22;
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.colorSlider1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.colorSlider1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ColorSchema = ColorSlider.ColorSlider.ColorSchemas.GreenColors;
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.Gray;
            this.colorSlider1.ElapsedPenColorBottom = System.Drawing.Color.Silver;
            this.colorSlider1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colorSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(19, 3);
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.colorSlider1.ScaleDivisions = 10;
            this.colorSlider1.ScaleSubDivisions = 5;
            this.colorSlider1.ShowDivisionsText = true;
            this.colorSlider1.ShowSmallScale = false;
            this.colorSlider1.Size = new System.Drawing.Size(48, 238);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 0;
            this.colorSlider1.Text = "colorSlider1";
            this.colorSlider1.ThumbImage = global::QuirkSoft.Properties.Resources.BTN_Thumb_GrayX2;
            this.colorSlider1.ThumbInnerColor = System.Drawing.Color.Green;
            this.colorSlider1.ThumbPenColor = System.Drawing.Color.Green;
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.colorSlider1.ThumbSize = new System.Drawing.Size(26, 16);
            this.colorSlider1.TickAdd = 0F;
            this.colorSlider1.TickColor = System.Drawing.SystemColors.ButtonFace;
            this.colorSlider1.TickDivide = 0F;
            this.colorSlider1.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
            this.colorSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            // 
            // VolumeController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelCursorPos);
            this.Controls.Add(this.verticalProgressBar1);
            this.Controls.Add(this.colorSlider1);
            this.Controls.Add(this.labelVolumeLevel);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.buttonMute);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "VolumeController";
            this.Size = new System.Drawing.Size(280, 247);
            this.Leave += new System.EventHandler(this.VolumeController_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSens)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelVolumeLevel;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Button buttonMute;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelShift;
        private System.Windows.Forms.RadioButton radioButtonModShift;
        private System.Windows.Forms.Label labelAlt;
        private System.Windows.Forms.RadioButton radioButtonModAlt;
        private System.Windows.Forms.Label labelCtrl;
        private System.Windows.Forms.RadioButton radioButtonModCtrl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonHorizontalAxis;
        private System.Windows.Forms.Label labelCursorPos;
        private System.Windows.Forms.RadioButton radioButtonVerticalAxis;
        private VerticalProgressBar verticalProgressBar1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelSensValue;
        private System.Windows.Forms.TrackBar trackBarSens;
        private System.Windows.Forms.Timer timer1;
        private ColorSlider.ColorSlider colorSlider1;
    }
}
