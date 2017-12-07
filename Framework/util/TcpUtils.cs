using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.Util
{
    /// <summary>
    /// provide methods to work with TcpClient
    /// </summary>
    public class TcpUtils
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private static readonly Logger Log = Logger.Instance;

        /// <summary>
        /// initialize tcpclient and networkstream
        /// </summary>
        /// <param name="hostname">hostname or ip</param>
        /// <param name="port">port</param>
        public TcpUtils(string hostname, string port)
        {
            _client = new TcpClient(hostname, Convert.ToInt32(port)) { ReceiveTimeout = 10000 };
            _stream = _client.GetStream();
        }

        /// <summary>
        /// write data to stream and get response
        /// </summary>
        /// <param name="data">data to write</param>
        /// <returns>response</returns>
        public string WriteToStream(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            _stream.Write(bytes, 0, bytes.Length);
            Thread.Sleep(5000);
            var read = new byte[10000];
            _stream.Read(read, 0, read.Length);
            return Encoding.ASCII.GetString(read);
        }

        /// <summary>
        /// write data to stream 
        /// </summary>
        /// <param name="data">data to write</param>
        public void WriteToStreame(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// write data to stream and get response
        /// </summary>
        /// <param name="data">data to write</param>
        /// <returns>response</returns>
        public string WriteLineToStream(string data)
        {
            Log.Info("Выполняем команду: " + data);
            var bytes = Encoding.ASCII.GetBytes(data + "\r\n");
            _stream.Write(bytes, 0, bytes.Length);
            Thread.Sleep(7000);
            var read = new byte[10000];
            _stream.Read(read, 0, read.Length);
            Log.Info("Ответ на команду: " + new Regex("\\0", RegexOptions.IgnoreCase).Replace(Encoding.ASCII.GetString(read), string.Empty));
            return Encoding.ASCII.GetString(read);
        }

        /// <summary>
        /// close client and stream
        /// </summary>
        public void Close()
        {
            _stream.Close();
            _client.Close();
        }
    }
}
