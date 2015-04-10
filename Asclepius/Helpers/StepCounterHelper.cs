using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.Windows.Threading;
using Microsoft.Phone.Applications.Common;
using System.Windows;

namespace Asclepius.Helpers
{
    class StepCounterHelper
    {
        public delegate void StepCounterReport(Record record);
        public event StepCounterReport OnStepCounterReport;

        private static volatile StepCounterHelper _singletonInstance;
        private static Object _syncRoot = new Object();

        private AccelerometerHelper _accelerometer = AccelerometerHelper.Instance;

        uint _totalSteps = 0; uint _runningSteps = 0;

        private StepCounterHelper()
        {

        }

        public static StepCounterHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new StepCounterHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        public void StartCounter()
        {
            _accelerometer.Active = true;
            _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;

            _pollTimer = new DispatcherTimer();
            _pollTimer.Interval = TimeSpan.FromSeconds(1);
            _pollTimer.Tick += _pollTimer_Tick;
            _pollTimer.Start();

            _reportTimer = new DispatcherTimer();
            _reportTimer.Interval = TimeSpan.FromSeconds(1);
            _reportTimer.Tick += _reportTimer_Tick;
            _reportTimer.Start();
        }

        Simple3DVector _last;
        int _samplesCount = 0;
        double[] _dsamples = new double[10];
        int _skip = 0;
        bool _isSkipping = false;
        double _lastAverage = 0;

        void _accelerometer_ReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            if (_last == null)
            {
                _last = e.RawAcceleration;
            }
            else
            {
                Simple3DVector _current = e.RawAcceleration;

                double _cosa = (_current.X * _last.X + _current.Y * _last.Y + _current.Z * _last.Z) /
                (Math.Sqrt(_current.X * _current.X + _current.Y * _current.Y + _current.Z * _current.Z) *
                Math.Sqrt(_last.X * _last.X + _last.Y * _last.Y + _last.Z * _last.Z));

                if (_samplesCount < 10)
                {
                    _dsamples[_samplesCount] += _cosa;
                    _samplesCount += 1;
                }
                else
                {
                    Array.Copy(_dsamples, 1, _dsamples, 0, _dsamples.Length - 1);
                    _dsamples[9] = _cosa;

                    if (_skip >= 10)
                    {
                        _skip = 0;
                        _isSkipping = false;
                    }
                    else
                    {
                        if (_isSkipping) _skip += 1;
                    }

                    if (!_isSkipping)
                    {
                        //double _weightedAverage = (double)Enumerable.Range(0, 9).Select(x => (x + 2) * _dsamples[x]).Sum() / 55;
                        ////don't know why +2 works but as long as it does...or not

                        //non-LINQ alternative
                        double _weightedAverage = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            _weightedAverage += (i + 1) * _dsamples[i];
                        }
                        _weightedAverage = _weightedAverage / 55;

                        if (_weightedAverage > _lastAverage && _weightedAverage < 0.95) //hardcoded threshold
                        {
                            OnStepDetected();
                            _isSkipping = true;
                            _skip = 0;
                        }
                        _lastAverage = _weightedAverage;
                    }
                }

                _last = _current;
            }
        }

        public uint WalkTime { get; set; }
        public uint RunTime { get; set; }
        public bool IsRunning { get; set; }
        public bool IsWalking { get; set; }

        private int _stepCount = 0;
        private int _lastCount = 0;
        private int _changeCount = 0;
        private int[] _changeRecord = new int[5];

        private void OnStepDetected()
        {
            _stepCount += 1;
            _totalSteps += 1;
            if (IsRunning) _runningSteps += 1;
        }

        void _reportTimer_Tick(object sender, EventArgs e)
        {
            Record ret = new Record();
            ret.WalkingStepCount = (int)(_totalSteps - _runningSteps);
            ret.RunningStepCount = (int)_runningSteps;
            ret.RunTime = RunTime;
            ret.WalkTime = WalkTime;

            _totalSteps = 0;
            _runningSteps = 0;
            RunTime = 0;
            WalkTime = 0;

            if (OnStepCounterReport != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { OnStepCounterReport(ret); });
            }
        }

        DispatcherTimer _pollTimer;
        DispatcherTimer _reportTimer;

        private void _pollTimer_Tick(object sender, EventArgs e)
        {
            _changeRecord[_changeCount] = _stepCount - _lastCount;

            if (_changeCount >= 4)
            {
                double _average = Enumerable.Range(0, 4).Select(x => (double)_changeRecord[x]).Sum() / 4;
                if (_average == 0)
                {
                    IsWalking = false;
                    IsRunning = false;
                }
                else if (_average <= 3)
                {
                    IsWalking = true;
                    IsRunning = false;
                }
                else
                {
                    IsWalking = false;
                    IsRunning = true;
                }
            }
            _changeCount = (_changeCount >= 4 ? 0 : _changeCount + 1);

            if (IsWalking) { WalkTime += 1; }
            if (IsRunning) { RunTime += 1; }
        }

        public void StopCounter()
        {
            _accelerometer.Active = false;
            _reportTimer.Stop();
            _pollTimer.Stop();
        }
    }
}
