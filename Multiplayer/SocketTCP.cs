using System.Net.Sockets;
using System.Text;

namespace Multiplayer{
    public class SocketTCP{
        private Socket socket;

        private CancellationTokenSource? _cancellationTokenSource;

        public delegate void ReceiveMessageEventHandler(string Message);
        public event ReceiveMessageEventHandler? OnReceiveMessage;

        public SocketTCP(){
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectToEchoServer(string HOST, int PORT){
            socket.Connect(HOST, PORT);
            _cancellationTokenSource = new CancellationTokenSource();
    
            Task.Run(() => GetMessageFromServer(_cancellationTokenSource.Token));
        }

        public void SendMessage(string Message){
            byte[] sendBytes = Encoding.UTF8.GetBytes(Message);
            socket.Send(sendBytes);
        }

        public void Disconnect(){
            _cancellationTokenSource?.Cancel();
            socket.Close();
        }
    
        private async Task GetMessageFromServer(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    
                    byte[] responseBytes = new byte[1024];
                    int bytesReceived = await socket.ReceiveAsync(responseBytes, SocketFlags.None, cancellationToken);

                    if (bytesReceived == 0 || cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    string message = System.Text.Encoding.UTF8.GetString(responseBytes, 0, bytesReceived);
                    OnReceiveMessage?.Invoke(message);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
            }
        }
    }
}
