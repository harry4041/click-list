using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using TMPro;

public class GameManager : MonoBehaviour
{
    /*public DateTime DTLoggedIn;
    public string dateLoggedIn;
    public string timeLoggedIn;

    public DateTime currentTime;

    public TextMeshProUGUI dt;
    public TextMeshProUGUI d;
    public TextMeshProUGUI t;

    public FirebaseManager FB;

    private void Start()
    {
        //DTLoggedIn = GetNetworkTime();
        UpdateTime();
    }

    private void Update()
    {
        if (DTLoggedIn != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UpdateTime();
                Debug.Log(GetNetworkTime());
            }
        }
    }

    private void UpdateTime()
    {
        currentTime = GetNetworkTime();
    }

    public void LoginDT()
    {
        string stringifyGetNetworkTime = GetNetworkTime().ToString();
        if (stringifyGetNetworkTime == null)
        {
            LoginDT();
        }
        else
        {
            dateLoggedIn = stringifyGetNetworkTime.Substring(0, 10);
            timeLoggedIn = stringifyGetNetworkTime.Substring(11, 8);
            dt.text = stringifyGetNetworkTime;
            d.text = dateLoggedIn;
            t.text = timeLoggedIn;
        }
    }

    //Yes I know this isn't GMT, but as long as it's the same for everyone it doesnt matter
    public static DateTime GetNetworkTime()
    {
        //default Windows time server
        const string ntpServer = "ntp.my-inbox.co.uk";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

        //The UDP port number assigned to NTP is 123
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //NTP uses UDP

        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
        }

        //Offset to get to the "Transmit Timestamp" field (time at which the reply 
        //departed the server for the client, in 64-bit timestamp format."
        const byte serverReplyTime = 40;

        //Get the seconds part
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

        //Get the seconds fraction
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        //Convert From big-endian to little-endian
        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

        //**UTC** time
        var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Local)).AddMilliseconds((long)milliseconds);

        return networkDateTime;
    }

    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }*/
}

