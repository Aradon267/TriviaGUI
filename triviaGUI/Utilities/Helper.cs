using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using System.Net;


namespace triviaGUI
{
    public class Helper
    {
        public Helper() { }
        public byte[] serializeMsg(int code, byte[] json)
        {
            byte[] sizeAndCode = new byte[5];
            int size = json.Length;
            sizeAndCode[0] = (byte)(code);
            sizeAndCode[1] = (byte)(size >> 24);
            sizeAndCode[2] = (byte)(size >> 16);
            sizeAndCode[3] = (byte)(size >> 8);
            sizeAndCode[4] = (byte)size;
            byte[] request = sizeAndCode.Concat(json).ToArray();
            return request;
        }
        public byte[] getFullMsg(NetworkStream clientStream)
        {
            byte[] code = new byte[1];
            byte[] sizeResp = new byte[4];

            clientStream.Read(code, 0, 1);
            clientStream.Read(sizeResp, 0, 4);

            int size_int = Convert.ToInt32((byte)(sizeResp[0]) << 24 |
        (byte)(sizeResp[1]) << 16 |
        (byte)(sizeResp[2]) << 8 |
        (byte)(sizeResp[3]));
            byte[] status = new byte[size_int];
            clientStream.Read(status, 0, size_int);
            return status;
        }
    }
}
