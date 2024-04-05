using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 12345;

        TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), serverPort);
        listener.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        bool exitRequested = false;

        while (!exitRequested)
        {
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string response = ProcessRequest(request);

            if (response == "EXIT")
            {
                exitRequested = true;
                Console.WriteLine("Клиент запросил выход. Разрыв соединения.");
                client.Close();
                continue;
            }

            byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBuffer, 0, responseBuffer.Length);
        }

        listener.Stop();
        Console.WriteLine("Сервер остановлен.");
    }

    static string ProcessRequest(string request)
    {
        if (request.Trim().ToUpper() == "EXIT")
        {
            return "EXIT"; 
        }

        string[] parts = request.Split(' ');
        string action = parts[0];
        string fileName = parts[1];

        if (action == "PUT" && parts.Length >= 3)
        {
            string fileContent = request.Substring(action.Length + fileName.Length + 2); 
            try
            {
                string serverFolderPath = "C:\\ServerFiles\\";

                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }

                File.WriteAllText(Path.Combine(serverFolderPath, fileName), fileContent);

                return "200"; 
            }
            catch (Exception ex)
            {
                return "500 Internal Server Error: " + ex.Message;
            }
        }
        else if (action == "GET")
        {
            try
            {
                string serverFolderPath = "C:\\ServerFiles\\";

                string filePath = Path.Combine(serverFolderPath, fileName);

                if (File.Exists(filePath))
                {
                    string fileContent = File.ReadAllText(filePath);

                    return "200 " + fileContent; 
                }
                else
                {
                    return "404 File Not Found"; 
                }
            }
            catch (Exception ex)
            {
                return "500 Internal Server Error: " + ex.Message; 
            }
        }
        else if (action == "DELETE")
        {
            try
            {
                string serverFolderPath = "C:\\ServerFiles\\";

                string filePath = Path.Combine(serverFolderPath, fileName);


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return "200"; 
                }
                else
                {
                    return "404 File Not Found"; 
                }
            }
            catch (Exception ex)
            {
                return "500 Internal Server Error: " + ex.Message; 
            }
        }
        else
        {
            return "400 Invalid Request";
        }
    }
}
