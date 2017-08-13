using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace easyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始连接服务器...");
            connectServer();
            Console.ReadKey();
        }
        private static async void connectServer()
        {
            var client = new EasyClient();
            client.Initialize(new MyReceiveFilter(), (request) =>
            {
                //回复格式：地址 功能码 字节数 数值……
                //01 03 02 00 01   地址:01 功能码:03 字节数:02 数据:00 01
                Console.WriteLine(DateTime.Now + " 收到16进制数据：" + request.HexAllData);
            });
            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));
            
            if (connected)
            {
               
                Console.WriteLine("已连接到服务器...");
                while(client.IsConnected) 
                {
                    client.Send(DataHelper.HexToByte(GetByteA(new Random().Next(5, 20))));
                    Thread.Sleep(2000);
                    client.Send(DataHelper.HexToByte(GetByteB(new Random().Next(5, 20))));
                    Thread.Sleep(2000);
                }
            }
            else
            {
                Console.WriteLine("连接服务器失败");
            }
        }
        private static string GetByteA(int length)
        {
            Random r = new Random();
            List<byte> bytes = new List<byte>();
            while(length > 0)
            {
                var k = r.Next(1, 255);
                bytes.Add((byte)k);
                length--;
            }
            string len = DataHelper.DecToHex(bytes.Count);
            string result = "7E 45 " + len + " " + DataHelper.ByteToHex(bytes.ToArray(), bytes.Count);
            Console.WriteLine(result);
            return result;
        }
        private static string GetByteB(int length)
        {
            Random r = new Random();
            List<byte> bytes = new List<byte>();
            while (length > 0)
            {
                var k = r.Next(1, 255);
                bytes.Add((byte)k);
                length--;
            }
            string result = "7E 46 " + DataHelper.ByteToHex(bytes.ToArray(), bytes.Count) + "0D";
            Console.WriteLine(result);
            return result;
        }
    }
}
