using QuirkSoft.VirtualVolumeKnob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using gma.System.Windows;
using CoreAudioApi;
using System.Threading;
using Utilities;
using System.IO;

namespace QuirkSoft
{
    public partial class MainForm : Form
    {
        public static MainForm instance;
        public static IniFile iniFile;
        public event EventHandler VolumeChanged;
        string m_IniPath = Path.Combine(Environment.CurrentDirectory, "settings.ini");
        public float sensDivider = 1000f;
        SystemVolumeController m_SystemVolumeController;
        UserActivityHook actHook;
        bool _controlDown;
        Point _mouseLastPos;
        private readonly MMDevice m_device;
        int sldVolume;
        DateTime _currentTime;
        float _timeDelta;
        TimeSpan time;
        CursorData cursor;
        bool m_IsHorizontal { get { return m_Preferences.Axis.Value == 1; } }
        Preferences m_Preferences;

        public MainForm()
        {
            instance = this;
            cursor = new CursorData();

            IniFileInit();
            InitializeComponent();

            #region CoreAudioApi initialization

            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            m_device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            sldVolume = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            m_device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            #endregion

            LoadPreferences();

            m_SystemVolumeController = new SystemVolumeController();
            try
            {
                //actHook = new UserActivityHook(); // crate an instance with global hooks
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            // hang on events
            //   actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
            //   actHook.KeyDown += new KeyEventHandler(MyKeyDown);
            //   actHook.KeyPress += new KeyPressEventHandler(MyKeyPress);
            //   actHook.KeyUp += new KeyEventHandler(MyKeyUp);
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

        [Obsolete]
        private void MyKeyPress(object sender, KeyPressEventArgs e)
        {
            //throw new NotImplementedException();
        }
        [Obsolete]
        private void MyKeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.LControlKey)
            {
                _controlDown = false;
            }
        }
        [Obsolete]
        private void MyKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.LControlKey)
            {
                _controlDown = true;
            }
        }
        [Obsolete]
        private void MouseMoved(object sender, MouseEventArgs e)
        {
            if (_controlDown)
            {
                Win32Wrap.POINT p = new Win32Wrap.POINT();
                Win32Wrap.ClientToScreen(this.Handle, ref p);

                //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
                //Cursor.Clip = new Rectangle(this.Location, this.Size);

                int mouseX = Cursor.Position.X;// e.X;
                int mouseY = Cursor.Position.Y;//e.Y;
                int screenX = Screen.PrimaryScreen.Bounds.Width;
                int screenY = Screen.PrimaryScreen.Bounds.Height;

                labelCursorPos.Text = Cursor.Position.X + ":" + Cursor.Position.Y;
                //labelCursorPos.Text = mouseX + ":" + mouseY;

                if (mouseX <= 0)
                {


                    //Win32Wrap.SetCursorPos(screenX - 10, mouseY);

                }
                if (mouseX >= screenX - 1)
                {
                    Cursor.Position = new Point(0, mouseY);
                    //Win32Wrap.SetCursorPos(0, mouseY);

                }


                if (mouseY <= 0)
                {
                    Cursor.Position = new Point(mouseX, screenY - 10);
                }
                if (mouseY >= screenY - 1)
                {
                    Cursor.Position = new Point(mouseX, 0);
                }

                //if (mouseX < 0 || mouseX > screenX || mouseY < 0 || mouseY > screenY)
                //    return;



                var dX = _mouseLastPos.X - e.X;
                var dY = _mouseLastPos.Y - e.Y;

                float sns = 10 / 1000f;

                // richTextBox1.AppendText("MX" + (mouseX) + ";MY" + (mouseY) + ";DX" + (dX) + ";DY" + (dY) + "[" + 1 / time.TotalMilliseconds + "]" + "\n\r");
                var vol = m_device.AudioEndpointVolume.MasterVolumeLevelScalar;
                if (dY > 0)
                {
                    m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Lerp(vol, vol + dY, sns * _timeDelta);
                }
                else
                {
                    m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Lerp(vol, vol + dY, sns * _timeDelta);
                }
            }
            _mouseLastPos = new Point(e.X, e.Y);
            time = (DateTime.Now - _currentTime);
            _timeDelta = (float)(1f / time.TotalMilliseconds);
            _currentTime = DateTime.Now;
        }

        private void buttonVolUp_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.VolUp();
        }

        private void buttonVolDown_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.VolDown();
        }

        private void buttonMute_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.Mute();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Position = new Point(1000, 256);
        }

        private void GlobalMouseMove(object sender, MouseEventArgs e)
        {
            labelCursorPos.Text = cursor.Position.X + ":" + cursor.Position.Y;

            if ((Control.ModifierKeys & (Keys)(m_Preferences.Modifier.Value)) != 0)
            {
                float sns = trackBarSens.Value / sensDivider;
                int mouseX = e.X;
                int mouseY = e.Y;
                int screenX = Screen.PrimaryScreen.Bounds.Width;
                int screenY = Screen.PrimaryScreen.Bounds.Height;
                int dX = cursor.DeltaX;
                int dY = cursor.DeltaY;

                if (m_IsHorizontal)
                {
                    dX = -cursor.DeltaY;
                    dY = -cursor.DeltaX;
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

                m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Clamp01(MathHelper.Lerp(vol, vol + dY, sns * _timeDelta));

                if (VolumeChanged != null)
                    VolumeChanged(this, null);
            }
            _mouseLastPos = new Point(e.X, e.Y);
            time = (DateTime.Now - _currentTime);
            _timeDelta = (float)(1f / time.TotalMilliseconds);
            _currentTime = DateTime.Now;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cursor.DeltaX = cursor.Position.X - Cursor.Position.X;
            cursor.DeltaY = cursor.Position.Y - Cursor.Position.Y;
            if (cursor.Position != Cursor.Position)
            {
                GlobalMouseMove(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, cursor.DeltaX + cursor.DeltaY));
            }
            cursor.Position = Cursor.Position;
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

        private void trackBarSens_Scroll(object sender, EventArgs e)
        {
            labelSensValue.Text = trackBarSens.Value.ToString();
            m_Preferences.Sensitivity.Value = trackBarSens.Value;
        }

        private void VerticalAxis_CheckedChanged(object sender, EventArgs e)
        {
            m_Preferences.Axis.Value = radioButtonVerticalAxis.Checked ? 0 : 1;
        }

        private void radioButtonModCtrl_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModCtrl.Checked)
            {
                m_Preferences.Modifier.Value = (int)Keys.Control;
            }
        }

        private void radioButtonModAlt_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModAlt.Checked)
            {
                m_Preferences.Modifier.Value = (int)Keys.Alt;
            }
        }

        private void radioButtonModShift_CheckedChanged(object sender, EventArgs e)
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

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
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
