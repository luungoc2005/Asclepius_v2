using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Asclepius.Helpers
{
    public class GadgetHelper
    {
        DispatcherTimer updateTimer;
        Connectivity.BluetoothConnection bluetooth;

        private static volatile GadgetHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        
        public delegate void ValueReceivedHandler(float value);
        public delegate void StateChangedHandler(bool isconnected);
        public event ValueReceivedHandler TemperatureChanged;
        public event ValueReceivedHandler HeartRateChanged;
        public event StateChangedHandler GadgetStateChanged;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (!_isEnabled)
                {
                    if (bluetooth != null && bluetooth.IsConnected) bluetooth.Terminate();
                    if (updateTimer != null) updateTimer.Stop();
                }
                else
                {
                    updateTimer.Start();
                }
            }
        }

        public GadgetHelper()
        {
            bluetooth = new Connectivity.BluetoothConnection();
            bluetooth.MessageReceived += bluetooth_MessageReceived;
            bluetooth.OnStateChanged += bluetooth_OnStateChanged;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(5);
            updateTimer.Tick += updateTimer_Tick;
            if (IsEnabled) updateTimer.Start();
        }

        void bluetooth_OnStateChanged(object sender, bool isconnected)
        {
            if (!isconnected)
            {
                updateTimer.Start();
            }
            if (GadgetStateChanged != null) GadgetStateChanged(isconnected);
        }

        public bool IsConnected
        {
            get
            {
                return bluetooth.IsConnected;
            }
        }

        public static GadgetHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new GadgetHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        async void updateTimer_Tick(object sender, EventArgs e)
        {
            if (!bluetooth.IsConnected)
            {
                await bluetooth.EnumerateDevices();
                if (bluetooth.listNames.Count > 0)
                {
                    for (int i = 0; i < bluetooth.listNames.Count; i++)
                    {
                        if (bluetooth.listNames[i] == "Meleco")
                        {
                            bluetooth.Connect(bluetooth.listDevices[i]);
                            updateTimer.Stop();
                            break;
                        }
                    }
                }
            }
        }

        float[] _samples = new float[5];
        int _count = 0;

        void bluetooth_MessageReceived(float num1, float num2)
        {
            if (TemperatureChanged != null) TemperatureChanged(num1);

            //fill the array
            _samples[_count] = num2;
            _count += 1;
            if (_count == _samples.Length) _count = 0;
            
            if (HeartRateChanged != null)
            {
                if (Common.CommonMethods.WeightedVariance(_samples) < 25) 
                {
                    HeartRateChanged(num2);
                } //std.var within 5
                else
                {
                    //trash value
                    HeartRateChanged(0);
                }
            }
        }
    }
}
