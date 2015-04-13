using Asclepius.Connectivity;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Asclepius.Helpers
{
    class SocialHelper
    {
        private static volatile SocialHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        UDPClientFinder _finder;
        private StreamSocketListener _listener = new StreamSocketListener();
        private Dictionary<StreamSocket, User.AppUser> _connections = new Dictionary<StreamSocket, User.AppUser>();
        private string _tcpPort = "4197";
        User.AppUser _user;
        
        public static SocialHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new SocialHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        public SocialHelper()
        {
            _finder = new UDPClientFinder();
            _finder.OnClientFound += _finder_OnClientFound;
            _listener.ConnectionReceived += _listener_ConnectionReceived;
            _user = User.AccountsManager.Instance.CurrentUser;
        }

        private bool _discovery = false;
        public bool IsDiscoveryEnabled
        {
            get
            {
                return _discovery;
            }
            set
            {
                if (DeviceNetworkInformation.IsWiFiEnabled)
                    {
                    _discovery = value;
                    if (_discovery)
                    {
                        _finder.StartFinder();
                        _finder.BroadcastIP();
                        StartListen();
                    }
                    else
                    {
                        _finder.StopFinder();
                        StopListen();
                    }
                }
            }
        }
        
        private async void _finder_OnClientFound(byte[] clientIP)
        {
            StreamSocket _socket = new StreamSocket();
            //attempt to connect
            string _host = clientIP[0].ToString() + "." + clientIP[1].ToString() + "." + clientIP[2].ToString() + "." + clientIP[3].ToString();
            try
            {
                await _socket.ConnectAsync(new HostName(_host), _tcpPort);
                //successful
                _connections.Add(_socket, new User.AppUser());
                WaitForData(_socket);
            }
            catch
            {
                //connection failed
            }
        }

        #region "TCP Methods"

        private async void StartListen()
        {
            if (_listener == null) _listener = new StreamSocketListener();
            await _listener.BindServiceNameAsync(_tcpPort);
            _listener.ConnectionReceived += _listener_ConnectionReceived;            
        }

        private void StopListen()
        {
            _listener.Dispose();
        }

        void _listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                _connections.Add(args.Socket, new User.AppUser());
                WaitForData(args.Socket);
                SendUserData(args.Socket);
            }
            catch
            {
                //listener failed
            }
        }

        private void OnClientDisconnected(StreamSocket socket)
        {
            _connections.Remove(socket);
            socket.Dispose();
        }

        private async void WaitForData(StreamSocket socket)
        {
            try
            {
                var dataReader = new DataReader(socket.InputStream);

                //msgType
                var size = await dataReader.LoadAsync(5);

                if (size == 0)
                {
                    //disconnected
                    OnClientDisconnected(socket);
                }

                byte type = dataReader.ReadByte();
                int length = dataReader.ReadInt32();

                size = await dataReader.LoadAsync((uint)length);

                if (size != length)
                {
                    OnClientDisconnected(socket);
                }

                byte[] data=new byte[length];
                dataReader.ReadBytes(data);

                OnMessageReceived(socket, type, data);

                WaitForData(socket); //continue getting data
            }
            catch
            {
                //receive exception
            }
        }

        private async void SendBytes(StreamSocket socket, byte msgType, byte[] data)
        {
            try
            {
                var dataWriter = new DataWriter(socket.OutputStream);
                dataWriter.WriteByte(msgType);
                dataWriter.WriteInt32(data.Length);
                dataWriter.WriteBytes(data);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            catch
            {
                //send exception
            }
        }

        private void SendString(StreamSocket socket, byte msgType, string data)
        {
            if (data!=null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                SendBytes(socket, msgType, buffer);
            }
        }

        #endregion

        private void OnMessageReceived(StreamSocket socket, byte msgType, byte[] data)
        {
            User.AppUser _sourceUser;
            if (_connections.TryGetValue(socket, out _sourceUser))
            {
                switch (msgType)
                {
                    case 0:
                        break;
                    case 1:
                        _sourceUser.FileName = GetString(data);
                        break;
                    case 2:
                        _sourceUser.Username = GetString(data);
                        break;
                    case 3:
                        _sourceUser.UserAvatarSerialized = data;
                        //finish
                        SendUserData(socket);
                        break;
                    default:
                        break;
                }
            }

        }

        private string GetString(byte[] data)
        {
            if (data != null || data.Length == 0) return Encoding.UTF8.GetString(data, 0, data.Length);
            return "";
        }

        private void SendUserData(StreamSocket socket)
        {
            if (_user != null)
            {
                SendString(socket, 1, _user.FileName);
                SendString(socket, 2, _user.Username);
                if (_user.UserAvatarSerialized != null) SendBytes(socket, 3, _user.UserAvatarSerialized);
            }
        }
    }
}
