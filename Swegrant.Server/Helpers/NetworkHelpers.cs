using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Swegrant.Server.Helpers
{
    public class NetworkHelpers
    {
        public static string GetLocalIPv4()
        {
            string ip = GetLocalIPv4(NetworkInterfaceType.Ethernet) ;
            if (string.IsNullOrEmpty(ip))
            {
                ip = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            }
            return ip;
        }
        private static string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        public static string[] GetAllIPs()
        {
            List<string> ips = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ips.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ips.ToArray();
        }
    }
}
