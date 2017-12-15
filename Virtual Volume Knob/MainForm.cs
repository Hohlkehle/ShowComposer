using CoreAudioApi;
using QuirkSoft.VirtualVolumeKnob;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Utilities;

namespace QuirkSoft
{
    public partial class MainForm : Form
    {
        private readonly MMDevice m_device;
        public static MainForm instance;
        public static IniFile iniFile;
        public event EventHandler VolumeChanged;

        private string m_IniPath = Path.Combine(Environment.CurrentDirectory, "settings.ini");
        private Preferences m_Preferences;
        private SystemVolumeController m_SystemVolumeController;
        private Point m_MouseLastPos;
        private CursorData m_CursorData;
        private float m_SensDivider = 1000f;
        private int m_SoundVolume;
        private DateTime m_CurrentTime;
        private float m_TimeDelta;
        private TimeSpan m_WholeTime;
        private bool m_IsHorizontal { get { return m_Preferences.Axis.Value == 1; } }

        public MainForm()
        {
            instance = this;
            m_CursorData = new CursorData();

            IniFileInit();
            InitializeComponent();

            #region CoreAudioApi initialization
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            m_device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            m_SoundVolume = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            m_device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            #endregion

            LoadPreferences();

            m_SystemVolumeController = new SystemVolumeController();
           
            VolumeChanged += MainForm_VolumeChanged;
            timer1.Start();
        }


        private void LoadPreferences()
        {
            m_Preferences = new Preferences(iniFile);

            // Volume
            var value = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            verticalProgressBar1.Value = value;
            trackBarVolume.Value = value;
            labelVolumeLevel.Text = value.ToString();

            // Axes setup
            if (m_Preferences.Axis.Value == 0)
            {
                radioButtonVerticalAxis.Checked = true;
                radioButtonHorizontalAxis.Checked = false;
            }
            else
            {
                radioButtonVerticalAxis.Checked = false;
                radioButtonHorizontalAxis.Checked = true;
            }

            // Sensetivity
            trackBarSens.Value = MathHelper.Clamp(m_Preferences.Sensitivity.Value, 1, 100);
            labelSensValue.Text = m_Preferences.Sensitivity.Value.ToString();

            switch ((Keys)(m_Preferences.Modifier.Value))
            {
                case Keys.Control:
                    radioButtonModCtrl.Checked = true;
                    radioButtonModAlt.Checked = false;
                    radioButtonModShift.Checked = false;
                    break;
                case Keys.Alt:
                    radioButtonModCtrl.Checked = false;
                    radioButtonModAlt.Checked = true;
                    radioButtonModShift.Checked = false;
                    break;
                case Keys.Shift:
                    radioButtonModCtrl.Checked = false;
                    radioButtonModAlt.Checked = false;
                    radioButtonModShift.Checked = true;
                    break;
                default:
                    break;
            }
        }

        void IniFileInit()
        {
            if (!File.Exists(m_IniPath))
            {
                var fstreem = File.Create(m_IniPath);
                fstreem.Close();

                iniFile = new IniFile(m_IniPath);
                SaveSettings();
            }
            else
            {
                iniFile = new IniFile(m_IniPath);
            }
        }

        void SaveSettings()
        {
            try
            {
                m_Preferences.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения настроек. " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void ButtonVolUp_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.VolUp();
        }

        private void ButtonVolDown_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.VolDown();
        }

        private void ButtonMute_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.Mute();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Cursor.Position = new Point(1000, 256);
        }

        private void GlobalMouseMove(object sender, MouseEventArgs e)
        {
            labelCursorPos.Text = m_CursorData.Position.X + ":" + m_CursorData.Position.Y;

            if ((Control.ModifierKeys & (Keys)(m_Preferences.Modifier.Value)) != 0)
            {
                float sns = trackBarSens.Value / m_SensDivider;
                int mouseX = e.X;
                int mouseY = e.Y;
                int screenX = Screen.PrimaryScreen.Bounds.Width;
                int screenY = Screen.PrimaryScreen.Bounds.Height;
                int dX = m_CursorData.DeltaX;
                int dY = m_CursorData.DeltaY;

                if (m_IsHorizontal)
                {
                    dX = -m_CursorData.DeltaY;
                    dY = -m_CursorData.DeltaX;
                }

                if (mouseX <= 0)
                {
                    Cursor.Position = new Point(screenX - 1, mouseY);
                }
                else if (mouseX >= screenX - 1)
                {
                    Cursor.Position = new Point(0, mouseY);
                }
                if (mouseY <= 0)
                {
                    Cursor.Position = new Point(mouseX, screenY - 1);
                }
                else if (mouseY >= screenY - 1)
                {
                    Cursor.Position = new Point(mouseX, 0);
                }

                // richTextBox1.AppendText("MX" + (mouseX) + ";MY" + (mouseY) + ";DX" + (dX) + ";DY" + (dY) + "[" + 1 / time.TotalMilliseconds + "]" + "\n\r");
                var vol = m_device.AudioEndpointVolume.MasterVolumeLevelScalar;

                m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Clamp01(MathHelper.Lerp(vol, vol + dY, sns * m_TimeDelta));

                if (VolumeChanged != null)
                    VolumeChanged(this, null);
            }
            m_MouseLastPos = new Point(e.X, e.Y);
            m_WholeTime = (DateTime.Now - m_CurrentTime);
            m_TimeDelta = (float)(1f / m_WholeTime.TotalMilliseconds);
            m_CurrentTime = DateTime.Now;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            m_CursorData.DeltaX = m_CursorData.Position.X - Cursor.Position.X;
            m_CursorData.DeltaY = m_CursorData.Position.Y - Cursor.Position.Y;
            if (m_CursorData.Position != Cursor.Position)
            {
                GlobalMouseMove(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, m_CursorData.DeltaX + m_CursorData.DeltaY));
            }
            m_CursorData.Position = Cursor.Position;
            LateUpdate();
        }

        private void LateUpdate()
        {
            verticalProgressBar1.Value = (int)(m_device.AudioMeterInformation.MasterPeakValue * 100);
        }

        private void MainForm_VolumeChanged(object sender, EventArgs e)
        {
            var value = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            verticalProgressBar1.Value = value;
            trackBarVolume.Value = value;
            labelVolumeLevel.Text = value.ToString();
        }

        private void TrackBarSens_Scroll(object sender, EventArgs e)
        {
            labelSensValue.Text = trackBarSens.Value.ToString();
            m_Preferences.Sensitivity.Value = trackBarSens.Value;
        }

        private void VerticalAxis_CheckedChanged(object sender, EventArgs e)
        {
            m_Preferences.Axis.Value = radioButtonVerticalAxis.Checked ? 0 : 1;
        }

        private void RadioButtonModCtrl_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModCtrl.Checked)
            {
                m_Preferences.Modifier.Value = (int)Keys.Control;
            }
        }

        private void RadioButtonModAlt_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModAlt.Checked)
            {
                m_Preferences.Modifier.Value = (int)Keys.Alt;
            }
        }

        private void RadioButtonModShift_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModShift.Checked)
            {
                m_Preferences.Modifier.Value = (int)Keys.Shift;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.Close();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                exitToolStripMenuItem.Text = "Resume";
            }
            else
            {
                timer1.Start();
                exitToolStripMenuItem.Text = "Pause";
            }
        }

        private void ExitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = false;

                this.Show();

                this.WindowState = FormWindowState.Normal;
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                //notifyIcon1.Visible = true;
                this.Hide();

                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
