using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Asclepius.Connectivity
{
    class UDPClientFinder
    {
        public event ClientFoundEvent OnClientFound;
        public delegate void ClientFoundEvent(byte[] clientIP);

        private const int GROUP_PORT = 7194;
        private const string GROUP_ADDRESS = "224.0.1.2";
        bool _joined = false;
        private byte[] _receiveBuffer;

        // Maximum size of a message in this communication
        private const int MAX_MESSAGE_SIZE = 512;

        public static string LocalIPAddress()
        {
            List<string> ipAddresses = new List<string>();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                //IanaInterfaceType == 71 => Wifi
                //IanaInterfaceType == 6 => Ethernet (Emulator)
                if (hn.IPInformation != null &&
                    (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
                    || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6))
                {
                    string ipAddress = hn.DisplayName;
                    ipAddresses.Add(ipAddress);
                }
            }

            if (ipAddresses.Count < 1)
            {
                return null;
            }
            else if (ipAddresses.Count == 1)
            {
                return ipAddresses[0];
            }
            else
            {
                //if multiple suitable address were found use the last one
                //(regularly the external interface of an emulated device)
                return ipAddresses[ipAddresses.Count - 1];
            }
        }

        UdpAnySourceMulticastClient _client;
        public void StartFinder()
        {
            try
            {
                _receiveBuffer = new byte[MAX_MESSAGE_SIZE];

                // Create the UdpAnySourceMulticastClient instance using the defined 
                // GROUP_ADDRESS and GROUP_PORT constants. UdpAnySourceMulticastClient is a 
                // client receiver for multicast traffic from any source, also known as Any Source Multicast (ASM)
                _client = new UdpAnySourceMulticastClient(IPAddress.Parse(GROUP_ADDRESS), GROUP_PORT);

                // Make a request to join the group.
                _client.BeginJoinGroup(
                    result =>
                    {
                        // Complete the join
                        _client.EndJoinGroup(result);

                        // The MulticastLoopback property controls whether you receive multicast 
                        // packets that you send to the multicast group. Default value is true, 
                        // meaning that you also receive the packets you send to the multicast group. 
                        // To stop receiving these packets, you can set the property following to false
                        _client.MulticastLoopback = false;

                        // Set a flag indicating that we have now joined the multicast group 
                        _joined = true;

                        // Wait for data from the group. This is an asynchronous operation 
                        // and will not block the UI thread.
                        Receive();
                    }, null);
            }
            catch { }
        }

        public void StopFinder()
        {
            _joined = false;
        }

        public void BroadcastIP()
        {
            if (_joined)
            {
                Send(IPMessage());
            }
        }

        private byte[] IPMessage()
        {
            //IP Address to byte()
            string[] strIPTemp = LocalIPAddress().Split('.');
            //if (strIPTemp.Length != 4) throw new Exception("Invalid IP Address");
            return Enumerable.Range(0, 5).Select(x =>
                        (x == 0) ? (byte)1 : Convert.ToByte(strIPTemp[x - 1])).ToArray();
        }

        private void Send(byte[] data)
        {
            // Attempt the send only if you have already joined the group.
            if (_joined)
            {
                _client.BeginSendToGroup(data, 0, data.Length,
                    result =>
                    {
                        _client.EndSendToGroup(result);

                        // Log what we just sent
                        //Log(message, true);

                    }, null);
            }
            else
            {
                //Log("Message was not sent since you are not joined to the group", true);
            }
        }

        /// <summary>
        /// Receive data from the group and log it.
        /// </summary>
        private void Receive()
        {
            // Only attempt to receive if you have already joined the group
            if (_joined)
            {
                Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);
                _client.BeginReceiveFromGroup(_receiveBuffer, 0, _receiveBuffer.Length,
                    result =>
                    {
                        IPEndPoint source;

                        // Complete the asynchronous operation. The source field will 
                        // contain the IP address of the device that sent the message
                        _client.EndReceiveFromGroup(result, out source);

                        if (OnClientFound != null && _receiveBuffer[0] == 1)
                        {
                            byte[] temp = new byte[4];
                            Array.Copy(_receiveBuffer, 1, temp, 0, 4);
                            OnClientFound(temp);
                        }

                        // Call receive again to continue to "listen" for the next message from the group
                        Receive();
                    }, null);
            }
            else
            {
                //Log("Cannot receive. You are currently not joined to the group", true);
            }
        }

    }
}
