using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreAudioApi;
using QuirkSoft.VirtualVolumeKnob;

namespace QuirkSoft
{
    public partial class VolumeController : UserControl
    {
        class Preferences
        {
            public int Sensitivity { set; get; }
            public int Axis { set; get; }
            public int Modifier { set; get; }
        }
        public event EventHandler VolumeChanged;
        public float sensDivider = 1000f;
        CursorData cursor;
        private readonly MMDevice m_device;
        int sldVolume;
        SystemVolumeController m_SystemVolumeController;
        Preferences m_Preferences = new Preferences();
        Point _mouseLastPos;
        DateTime _currentTime;
        float _timeDelta;
        TimeSpan time;

        bool m_IsHorizontal { get { return m_Preferences.Axis == 1; } }

        public VolumeController()
        {
            InitializeComponent();

            cursor = new CursorData();

            m_Preferences.Axis = radioButtonVerticalAxis.Checked ? 0 : 1;
            m_Preferences.Modifier = (int)Keys.Shift;
            m_Preferences.Sensitivity = trackBarSens.Value;

            #region CoreAudioApi initialization

            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            m_device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            sldVolume = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            m_device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            #endregion

            m_SystemVolumeController = new SystemVolumeController();

            VolumeChanged += VolumeController_VolumeChanged;
            timer1.Start();

            VolumeController_VolumeChanged(null, null);
        }

        private void VolumeController_VolumeChanged(object sender, EventArgs e)
        {
            var value = (int)(m_device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            verticalProgressBar1.Value = value;
            colorSlider1.Value = value;
            trackBarVolume.Value = value;
            labelVolumeLevel.Text = value.ToString();
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

        private void trackBarSens_Scroll(object sender, EventArgs e)
        {
            labelSensValue.Text = trackBarSens.Value.ToString();
            m_Preferences.Sensitivity = trackBarSens.Value;
        }

        private void VerticalAxis_CheckedChanged(object sender, EventArgs e)
        {
            m_Preferences.Axis = radioButtonVerticalAxis.Checked ? 0 : 1;
        }

        private void radioButtonModCtrl_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModCtrl.Checked)
            {
                m_Preferences.Modifier = (int)Keys.Control;
            }
        }

        private void radioButtonModAlt_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModAlt.Checked)
            {
                m_Preferences.Modifier = (int)Keys.Alt;
            }
        }

        private void radioButtonModShift_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModShift.Checked)
            {
                m_Preferences.Modifier = (int)Keys.Shift;
            }
        }

        private void buttonMute_Click(object sender, EventArgs e)
        {
            m_SystemVolumeController.Mute();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {

        }

        private void GlobalMouseMove(object sender, MouseEventArgs e)
        {
            labelCursorPos.Text = cursor.Position.X + ":" + cursor.Position.Y;

            if ((Control.ModifierKeys & (Keys)(m_Preferences.Modifier)) != 0)
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

        public void SetVolumeScalar(float value)
        {
            m_device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
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

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Clamp01(colorSlider1.Value / 100f);
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & (Keys)(m_Preferences.Modifier)) != 0)
                return;

            m_device.AudioEndpointVolume.MasterVolumeLevelScalar = MathHelper.Clamp01(colorSlider1.Value / 100f);
        }

        public void Stop()
        {
            timer1?.Stop();
        }

        private void VolumeController_Leave(object sender, EventArgs e)
        {

        }
    }
}
