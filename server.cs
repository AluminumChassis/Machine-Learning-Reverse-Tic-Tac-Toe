using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
#pragma warning disable 0649
namespace AI
{
    public class Server
    {

        public static List<decimal[]> inputs = new List<decimal[]>();

        public static void start() {
            Console.WriteLine("start up http server...");

            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener listener = new TcpListener(ipAddress, 3000);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("request incoming...");

                NetworkStream stream = client.GetStream();
                string request = toString(stream);

                string input = request.Substring(request.IndexOf("Connection: keep-alive") + "Connection: keep-alive".Length).Replace(System.Environment.NewLine, "");
                if (input.Contains("won"))
                {
                    Console.WriteLine("Computer Won!");
                    decimal[] outputs = new decimal[inputs.Count];
                    for (int i = 0; i < outputs.Length; i++)
                    {
                        outputs[i] = 1;
                    }
                    network.train(inputs, outputs, 100);
                    inputs.Clear();
                }
                else if (input.Contains("lost"))
                {
                    Console.WriteLine("Computer Lost!");
                    decimal[] outputs = new decimal[inputs.Count];
                    for (int i = 0; i < outputs.Length; i++) {
                        outputs[i] = 0;
                    }
                    network.train(inputs, outputs, 100);
                    inputs.Clear();
                }
                else
                {
                    string[] strings = input.Split(',');
                    decimal[] nums = new decimal[strings.Length];

                    for (int i = 0; i < strings.Length; i++)
                    {
                        nums[i] = decimal.Parse(strings[i]);
                    }
                    decimal[] best = new decimal[0];
                    decimal bestAccuracy = 0;
                    decimal accuracy;
                    for (int i = 0; i < nums.Length; i++)
                    {
                        if (nums[i] == 0)
                        {
                            decimal[] temp = (decimal[])nums.Clone();
                            temp[i] = -1;
                            accuracy = network.run(temp);

                            if (accuracy > bestAccuracy)
                            {
                                best = (decimal[])temp.Clone();
                                bestAccuracy = accuracy;
                            }
                        }
                    }
                    inputs.Add(best);

                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(@"HTTP/1.1 200 OK");
                    builder.AppendLine(@"Content-Type: text/html");
                    builder.AppendLine(@"Access-Control-Allow-Origin: *");
                    builder.AppendLine(@"");
                    builder.AppendLine(string.Join(",", best));


                    byte[] sendBytes = Encoding.UTF8.GetBytes(builder.ToString());
                    stream.Write(sendBytes, 0, sendBytes.Length);

                    stream.Close();
                    client.Close();
                }
            }
        }
        public static string toString(NetworkStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] data = new byte[256];
            int size;
            do
            {
                size = stream.Read(data, 0, data.Length);
                if (size == 0)
                {
                    Console.WriteLine("client disconnected...");
                    Console.ReadLine();
                    return null;
                }
                memoryStream.Write(data, 0, size);
            } while (stream.DataAvailable);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
