using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Foundation;
using Windows.Networking.Proximity;
using System.Windows;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace Asclepius.Connectivity
{
    public class BluetoothConnection
    {
        #region "Variables"
        private IAsyncOperation<RfcommDeviceService> connectService;

        private RfcommDeviceService rfcommService;

        private StreamSocket socket;

        private StreamWriter dataWriter;

        private StreamReader dataReader;

        private BackgroundWorker dataReadWorker;

        public delegate void MessageReceivedHandler(float num1, float num2);
        public delegate void StateChanged(object sender, bool isconnected);

        public event MessageReceivedHandler MessageReceived;
        public event EventHandler BluetoothNotSupported;
        public event StateChanged OnStateChanged;
        #endregion

        #region "Public methods"

        public BluetoothConnection()
        {
            socket = new StreamSocket();
            dataReadWorker = new BackgroundWorker();
            dataReadWorker.WorkerSupportsCancellation = true;
            dataReadWorker.DoWork += new DoWorkEventHandler(ReceiveMessages);
        }

        public void Terminate()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (dataReadWorker != null)
            {
                dataReadWorker.CancelAsync();
                dataReadWorker = null;
            }
            if (OnStateChanged != null) OnStateChanged(this, false);
        }

        public async Task EnumerateDevices()
        {
            try
            {
                var serviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
                //var retList = new List<DeviceInformation>();
                listDevices.Clear();
                listNames.Clear();
                foreach (var serviceInfo in serviceInfoCollection)
                {
                    listNames.Add(serviceInfo.Name);
                    listDevices.Add(serviceInfo);
                }
                //return retList;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    if (BluetoothNotSupported != null) BluetoothNotSupported(this, new EventArgs());
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return (socket != null && dataReader != null && dataWriter != null);
            }
        }

        public List<DeviceInformation> listDevices = new List<DeviceInformation>();
        public List<string> listNames = new List<string>();

        public async void Connect(DeviceInformation serviceInfo)
        {
            if (socket != null)
            {
                try
                {
                    connectService = RfcommDeviceService.FromIdAsync(serviceInfo.Id);
                    rfcommService = await connectService;
                    if (rfcommService != null)
                    {
                        await socket.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                        dataReader = new StreamReader(socket.InputStream.AsStreamForRead());
                        dataWriter = new StreamWriter(socket.OutputStream.AsStreamForWrite());
                        dataReadWorker.RunWorkerAsync();
                        if (OnStateChanged != null) OnStateChanged(this, true);
                    }
                    else
                    {
                        MessageBox.Show("Connection to gadget failed");
                    }
                    //await socket.ConnectAsync(serviceInfo., rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    //dataReader = new DataReader(socket.InputStream);
                    //dataReadWorker.RunWorkerAsync();
                    //dataWriter = new DataWriter(socket.OutputStream);
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task<bool> SendCommand(byte[] buffer)
        {
            //if (dataWriter != null)
            //{
            //    dataWriter.WriteBytes(buffer);

            //    try
            //    {
            //        await dataWriter.StoreAsync();
            //        return true;
            //    }
            //    catch
            //    {
            //        return false;
            //        throw;
            //    }
            //}
            //else return false;
            return true;
        }

        #endregion

        #region "Private methods"

        private void ReceiveMessages(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    if (dataReader != null)
                    {
                        try
                        {
                            float num1; float num2;
                            num1 = Convert.ToSingle(dataReader.ReadLine());
                            num2 = Convert.ToSingle(dataReader.ReadLine());

                            if (MessageReceived != null)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() => { MessageReceived(num1, num2); });
                            }
                        }
                        catch { }
                    }
                }
            }
            catch //(Exception ex)
            {
                Terminate();
            }

        }


        #endregion

    }
}
