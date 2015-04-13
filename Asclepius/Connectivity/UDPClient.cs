using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage.Streams;
using System.Net.Sockets;

namespace Asclepius.Connectivity
{
    class UDPClient
    {
        public delegate void DataReceivedEvent(byte[] dest, byte msgType, byte[] data);
        public event DataReceivedEvent OnDataReceived;

        Socket socket;
        Socket socket2;

        int udptPort = 4197;
        int MAX_LENGTH = 256;

        bool _isListening = false;
        
        public bool IsActive
        {
            get
            {
                return _isListening;
            }
            set
            {
                _isListening = value;
                if (_isListening) Receive();
            }
        }

        public void Receive()
        {
            try
            {
                if (socket2==null) socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                EndPoint socketEndPoint = new IPEndPoint(IPAddress.Any, udptPort);
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Any, udptPort);
                socketEventArg.Completed += socketEventArg_Completed;
                socketEventArg.SetBuffer(new byte[MAX_LENGTH], 0, MAX_LENGTH);
                socket2.Bind(socketEndPoint);
                

                try
                {
                    //socket2.ReceiveAsync(socketEventArg);
                }
                catch
                {
                    //throw;
                }
            }
            catch { }
        }

        private MemoryStream dataReceived = new MemoryStream();
        private DataWriter dataWriter;
        private byte[] destination = new byte[4];
        private byte msgType = 0;
        private int totalLength = 0;
        private void socketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[MAX_LENGTH];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);

                if (dataWriter==null) dataWriter = new DataWriter(dataReceived.AsOutputStream());

                if (dataReceived.Length == 0)
                {
                    Array.Copy(data, 0, destination, 0, 4);
                    msgType = data[4];
                    totalLength = BitConverter.ToInt32(data, 5);

                    byte[] buffer = new byte[MAX_LENGTH - 5 - sizeof(int)];
                    Array.Copy(data, 5 + sizeof(int), buffer, 0, buffer.Length);
                    dataWriter.WriteBytes(buffer);
                }
                else
                {
                    dataWriter.WriteBytes(data);
                }

                if (totalLength != 0 && dataReceived.Length>=totalLength)
                {
                    dataReceived.Seek(0, 0);
                    byte[] final = new byte[totalLength];
                    dataReceived.Read(final, 0, totalLength);
                    if (OnDataReceived != null) OnDataReceived(destination, msgType, final);

                    if (dataWriter != null)
                    {
                        dataWriter.Dispose();
                        dataWriter = null;
                    }
                    totalLength = 0;
                    dataReceived = new MemoryStream();
                    destination = new byte[4];
                    msgType = 0;
                }
            }
            else //socket error, discard data
            {
                if (dataWriter!=null)
                {
                    dataWriter.Dispose();
                    dataWriter = null;
                }
                totalLength = 0;
                dataReceived = new MemoryStream();
                destination = new byte[4];
                msgType = 0;
            }
            if (_isListening)
            {
                //Receive();
            }
        }

        public async Task SendMessage(byte msgType, byte[] message, byte[] host)
        {
            string _host = host[0].ToString() + "." + host[1].ToString() + "." + host[2].ToString() + "." + host[3].ToString();

            var ms = new MemoryStream();
            var writer = new DataWriter(ms.AsOutputStream());

            writer.WriteBytes(host);
            writer.WriteByte(msgType);
            writer.WriteInt32(message.Length);
            writer.WriteBytes(message);

            await writer.StoreAsync();

            byte[] allData = ms.ToArray();
            writer.Dispose();

            int sentData = 0;
            //ms.Seek(0, 0);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


            while (sentData<allData.Length)
            {
                byte[] buffer = new byte[512];
                //ms.Read(buffer, sentData, MAX_LENGTH);
                Array.Copy(allData, sentData, buffer, 0, Math.Min(buffer.Length, allData.Length - sentData));

                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(_host), udptPort);
                socketEventArg.SetBuffer(buffer, 0, buffer.Length);
                socket.ConnectAsync(socketEventArg);

                sentData += MAX_LENGTH;
            }

        }
    }
}
