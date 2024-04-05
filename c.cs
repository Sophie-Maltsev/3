using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 12345;

        try
        {
            TcpClient client = new TcpClient(serverIP, serverPort);

            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];


            bool exitRequested = false;
            while (!exitRequested) 
            {
                Console.WriteLine("Введите действие (GET, PUT или DELETE):");
                string action = Console.ReadLine().ToUpper();
                string request = "";

                if (action == "EXIT")
                {
                    request = action;
                    exitRequested = true;
                }
                else
                {
                    Console.WriteLine("Введите имя файла:");
                    string fileName = Console.ReadLine();

                    if (action == "PUT")
                    {
                        Console.WriteLine("Введите содержимое файла:");
                        string fileContent = Console.ReadLine();
                        request = action + " " + fileName + " " + fileContent;
                    }
                    else
                    {
                        request = action + " " + fileName;
                    }
                }
                buffer = Encoding.UTF8.GetBytes(request);
                stream.Write(buffer, 0, buffer.Length);

                if (action == "EXIT")
                {
                    Console.WriteLine("Выход с сервера");

                }

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                if (response.StartsWith("200"))
                {
                    if (action == "GET")
                    {
                        string fileContent = response.Substring(4);
                        Console.WriteLine("Содержимое файла: " + fileContent);
                    }
                    else if (action == "PUT")
                    {
                        Console.WriteLine("The response says that the file was created!");
                    }
                    else if (action == "DELETE")
                    {
                        Console.WriteLine("The response says that the file was successfully deleted!");
                    }
                }
                else if (response.StartsWith("403"))
                {
                    Console.WriteLine("The response says that creating the file was forbidden!");
                }
                else if (response.StartsWith("404"))
                {
                    if (action == "GET")
                    {
                        Console.WriteLine("The response says that the file was not found!");
                    }
                    else if (action == "DELETE")
                    {
                        Console.WriteLine("The response says that the file was not found!");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестный ответ от сервера.");
                }
            }
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при подключении к серверу: " + ex.Message);
        }
    }
}
