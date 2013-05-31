using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using PacketDotNet;
using System.Net.NetworkInformation;
using System.Net;

namespace Sniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}", ver);


            CaptureDeviceList devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices found.");
            }

            Console.WriteLine("Devices found:");
            devices.ToList().ForEach(Console.WriteLine);


            ICaptureDevice localAreaNetworkNIC = devices[1];
            localAreaNetworkNIC.OnPacketArrival += new PacketArrivalEventHandler(localAreaNetworkNIC_OnPacketArrival);

            // open device
            int readTimeoutMill = 1000;
            localAreaNetworkNIC.Open(DeviceMode.Promiscuous, readTimeoutMill);

            localAreaNetworkNIC.Filter = "ip and tcp";

            Console.WriteLine("Filtering: {0}", localAreaNetworkNIC.Filter);
            Console.WriteLine("Listening on {0}", localAreaNetworkNIC.Description);

            localAreaNetworkNIC.StartCapture();

            Console.ReadLine();

            localAreaNetworkNIC.StopCapture();

            localAreaNetworkNIC.Close();
        }

        static void localAreaNetworkNIC_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            //DateTime time = e.Packet.Timeval.Date;
            //int len = e.Packet.Data.Length;
            //string content = ByteArrayToString(e.Packet.Data);
            //Console.WriteLine("{0}:{1}:{2},{3} Len={4}: {5}",
            //    time.Hour, time.Minute, time.Second, time.Millisecond, len, content);

            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

            if (e.Packet.LinkLayerType == LinkLayers.Ethernet)
            {
                EthernetPacket ethernetPacket = (EthernetPacket)packet;
                PhysicalAddress destMacAddress = ethernetPacket.DestinationHwAddress;
                PhysicalAddress srcMacAddress = ethernetPacket.SourceHwAddress;
                //Console.WriteLine("Mac: {0} -> {1}", srcMacAddress, destMacAddress);
            }

            IpPacket ipPacket = IpPacket.GetEncapsulated(packet);
            if (ipPacket != null)
            {
                IPAddress srcIpAddress = ipPacket.SourceAddress;
                IPAddress destIpAddress = ipPacket.DestinationAddress;
                //Console.WriteLine("IP: {0} -> {1}", srcIpAddress, destIpAddress);

                TcpPacket tcpPacket = TcpPacket.GetEncapsulated(packet);
                if (tcpPacket != null)
                {
                    int srcPort = tcpPacket.SourcePort;
                    int destPort = tcpPacket.DestinationPort;
                    //Console.WriteLine("TCP Port: {0} -> {1}", srcPort, destPort);

                    byte[] tcpBody = RipHeader(tcpPacket.Bytes, tcpPacket.Header);
                    string packetString = ByteArrayToString(tcpBody);
                    if (packetString.Contains("HTTP"))
                    {
                        Console.WriteLine("IP: {0} -> {1}", srcIpAddress, destIpAddress);
                        Console.WriteLine("TCP Port: {0} -> {1}", srcPort, destPort);
                        Console.WriteLine(ByteArrayToString(tcpBody));
                    }
                }

                

                //UdpPacket udpPacket = UdpPacket.GetEncapsulated(packet);
                //if (udpPacket != null)
                //{
                //    int srcPort = udpPacket.SourcePort;
                //    int destPort = udpPacket.DestinationPort;
                //    Console.WriteLine("UDP Port: {0} -> {1}", srcPort, destPort);
                //}
            }

            //string packetString = PacketString(packet);

            //Console.WriteLine(packetString);
            //Console.WriteLine();
        }

        static string PacketString(Packet p)
        {
            if (p != null)
            {
                StringBuilder s = new StringBuilder(p.PrintHex());
                return s.ToString();
            }
            return string.Empty;
        }

        

        static string ByteArrayToString(byte[] byteArray, int length = -1)
        {
            if (length < 0)
            {
                length = byteArray.Length;
            }

            //StringBuilder content = new StringBuilder();
            //for (int i = 0; i < length; i++)
            //{
            //    content.Append(i.ToString() + " ");
            //}
            //return content.ToString();

            string content = Encoding.ASCII.GetString(byteArray);
            return content;
        }

        static byte[] RipHeader(byte[] full, byte[] header)
        {
            byte[] body;

            body = full.Skip(header.Length).ToArray();

            return body;
        }
    }
}
