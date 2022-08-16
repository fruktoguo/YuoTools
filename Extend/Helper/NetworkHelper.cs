using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace YuoTools.Extend.Helper
{
    public static class NetworkHelper
    {
        public static string[] GetAddressIPs()
        {
            var list = new List<string>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                foreach (var add in networkInterface.GetIPProperties().UnicastAddresses)
                    list.Add(add.Address.ToString());
            }

            return list.ToArray();
        }

        public static IPEndPoint ToIPEndPoint(string host, int port)
        {
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address)
        {
            var index = address.LastIndexOf(':');
            var host = address.Substring(0, index);
            var p = address.Substring(index + 1);
            var port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }
    }
}