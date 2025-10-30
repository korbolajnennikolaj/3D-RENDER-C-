import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

public class EchoServer {
    private final int port;
    private List<Socket> clients = new ArrayList<>();

    public EchoServer(int port) {
        this.port = port;
    }

    public void start() throws IOException {
        ServerSocket serverSocket = new ServerSocket(port);
        System.out.println("Server started on port " + port);
        
        // Запускаем бесконечный цикл для приема новых клиентов
        while (true) {
            Socket socket = serverSocket.accept();
            InetAddress address = socket.getInetAddress();
            System.out.println("New client connected from: " + address.getHostName());
            
            // Добавляем клиента в список
            clients.add(socket);
            
            // Создаем новый поток для обработки сообщений от клиента
            Thread thread = new Thread(() -> handleClient(socket));
            thread.start();
        }
    }

    private void handleClient(Socket socket) {
        try {
            while (true) {
                byte[] buffer = new byte[1024];
                int readBytes = socket.getInputStream().read(buffer);
                String message = new String(buffer, 0, readBytes);
                System.out.println("Received from client: " + message.trim());

                // Отправка сообщения всем клиентам кроме отправителя
                for (Socket client : clients) {
                    if (!client.equals(socket)) {
                        client.getOutputStream().write(message.getBytes());
                    }
                }
            }
        } catch (IOException e) {
            System.err.println("Client disconnected");
            clients.remove(socket); // Удаляем клиента из списка при разрыве соединения
        }
    }

    public static void main(String[] args) {
        try {
            EchoServer server = new EchoServer(24117);
            server.start();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
