namespace Hoard.Unity
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public class WebGLWebSocketProvider : IWebSocketProvider
    {
        private string Url = "";
        private int Socket = 0;
        private float TimeoutCounter = 0.0f;
        private readonly float TimeOut = 30.0f;

        [DllImport("__Internal")]
        private static extern int SocketCreate(string url);

        [DllImport("__Internal")]
        private static extern int SocketState(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSend(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketRecv(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern int SocketRecvLength(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketClose(int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError(int socketInstance, byte[] ptr, int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public WebGLWebSocketProvider(string url)
        {
            Url = url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> Connect(CancellationToken token)
        {
            try
            {
                TimeoutCounter = TimeOut;
		        Socket = SocketCreate(Url);
                while (SocketState(Socket) == 0)
                {
                    await Task.Yield();
                    TimeoutCounter -= Time.unscaledDeltaTime;
                    if (TimeoutCounter < 0.0f)
                    {
                        ErrorCallbackProvider.ReportError("Connection timeout!");
                        return false;
                    }
                }
            }
            catch (TimeoutException)
            {
                ErrorCallbackProvider.ReportError("Connection timeout!");
                return false;
            }

            if (SocketState(Socket) != 1)
            {
                ErrorCallbackProvider.ReportError("Cannot connect to destination host: " + Url);
                return false;
            }

            ErrorCallbackProvider.ReportInfo("Connection established");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            await Task.Yield();

            if (SocketState(Socket) == 1)
            {
                SocketClose(Socket);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionOpen()
        {
            return (SocketState(Socket) == 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<byte[]> Receive(CancellationToken token)
        {
            await Task.Yield();

            if (IsConnectionOpen() == false)
            {
                return null;
            }

            int length = SocketRecvLength(Socket);
            if (length == 0)
            {
                JObject jobj = new JObject();
                return Encoding.UTF8.GetBytes(jobj.ToString());
            }
            byte[] buffer = new byte[length];
            SocketRecv(Socket, buffer, length);
            ErrorCallbackProvider.ReportInfo(Encoding.UTF8.GetString(buffer));
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Send(byte[] data, CancellationToken token)
        {
            await Task.Yield();

            SocketSend(Socket, data, data.Length);
        }
    }
#endif
}
