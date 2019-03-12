using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PubSubSockerApp.Tool
{
    public class SockerHelper
    {
        static byte[] m_result = new byte[1024];//存放接收的数据
        const int m_port = 8078;//端口号
        static string m_localIp = "127.0.0.1";
        static Socket m_serverSocket;//服务器socket
        static List<Socket> m_clientSocketList = new List<Socket>();//存放连接上的的客户端服务器

        public void CreateService()
        {
            IPAddress ipAddress = IPAddress.Parse(m_localIp);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, m_port);
            //创建服务器Socket对象，并设置相关属性  
            m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip和端口  
            m_serverSocket.Bind(ipEndPoint);
            //设置最长的连接请求队列长度  
            m_serverSocket.Listen(5000);
            Console.WriteLine("启动监听{0}成功", m_serverSocket.LocalEndPoint.ToString());
            //在新线程中监听客户端的连接  
            Thread thread = new Thread(ClientConnectListen);
            thread.Start();
            Console.ReadLine();
        }

        /// <summary>  
        /// 客户端连接请求监听  
        /// </summary>  
        static void ClientConnectListen()
        {
            while (true)
            {
                //为新的客户端连接创建一个Socket对象  
                Socket clientSocket = m_serverSocket.Accept();
                m_clientSocketList.Add(clientSocket);
                Console.WriteLine("客户端{0}成功连接", clientSocket.RemoteEndPoint.ToString());

                //向连接的客户端发送连接成功的数据
                NetBufferWriter writer = new NetBufferWriter();
                writer.WriteString("Connected Server Success");
                clientSocket.Send(writer.Finish());

                //每个客户端连接创建一个线程来接受该客户端发送的消息  
                Thread thread = new Thread(RecieveMessage);
                thread.Start(clientSocket);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>  
        /// 接收指定客户端Socket的消息  
        /// </summary>  
        static void RecieveMessage(object clientSocket)
        {
            Socket mClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = mClientSocket.Receive(m_result);
                    Console.WriteLine("接收客户端{0}消息， 长度为{1}", mClientSocket.RemoteEndPoint.ToString(), receiveNumber);

                    if (receiveNumber == 0)
                    {
                        //断开连接
                        Console.WriteLine("连接已断开{0}", mClientSocket.RemoteEndPoint.ToString());
                        RemoveClientSocket(mClientSocket);
                        break;
                    }

                    NetBufferReader reader = new NetBufferReader(m_result);
                    string data = reader.ReadString();
                    Console.WriteLine("数据内容：{0}", data);

                    SendMsg(data);  //给客户端发送消息
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    RemoveClientSocket(mClientSocket);
                    break;
                }
            }
        }

        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="data"></param>
        private static void SendMsg(string data)
        {
            //给所有连接上的客户端返回数据
            NetBufferWriter writer = new NetBufferWriter();
            writer.WriteString("Get Message:" + data);
            byte[] buffer = writer.Finish();
            foreach (Socket socket in m_clientSocketList)
            {
                socket.Send(buffer);
            }
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="clientSocket"></param>
        static void RemoveClientSocket(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            m_clientSocketList.Remove(clientSocket);
        }



    }
}
