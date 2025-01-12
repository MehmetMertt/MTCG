using System.Net.Sockets;
using System.Net;
using System.Text;
using MTCG.DAL;
using System.Reflection.PortableExecutable;
using MTCG.Classes;

namespace MTCG.PresentationLayer;

public class Server
{
    public readonly int PORT = 8080;

    private readonly RequestHandler _requestHandler;

    public Server()
    {
        const string DBCONNECTIONSTRING = "Host=localhost;Username=admin;Password=admin;Database=postgres";
        UserRepository.InitDb(DBCONNECTIONSTRING);
        DeckRepository.InitDb(DBCONNECTIONSTRING);
        CardRepository.InitDb(DBCONNECTIONSTRING);
        TradingShopRepository.InitDb(DBCONNECTIONSTRING);
        UnitOfWork.InitUnitOfWork(DBCONNECTIONSTRING);
        ShopService ss = new ShopService(UnitOfWork.Instance);
        _requestHandler = new RequestHandler(ss);
    }

    public void Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine($"Server started, listening on port {PORT}...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();

            // Spawn a new task for each client
            Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(object? clientObj)
    {
        TcpClient client = (TcpClient)clientObj!;
        try
        {
            using (client)
            {
                using NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] requestLines = request.Split("\r\n");
                string httpMethod = requestLines[0].Split(' ')[0];
                string requestUrl = requestLines[0].Split(' ')[1];
                string jsonBody = request.Contains("\r\n\r\n") ? request.Split("\r\n\r\n")[1] : string.Empty;

                HttpResponse response = _requestHandler.HandleRequest(requestUrl, httpMethod, jsonBody);
                SendHttpResponse(stream, response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }


    private void SendHttpResponse(NetworkStream stream, HttpResponse httpResponse)
    {
        string statusCode = httpResponse.GetStatusCode().ToString();
        string statusMessage = "test"; //TODO: Ändern!


        string responseHeader = $"HTTP/1.1 {statusCode} {statusMessage}\r\n";

        if (httpResponse.GetHeaders() != null)
        {
            foreach (var header in httpResponse.GetHeaders())
            {
                responseHeader += $"{header.Key}: {header.Value}\r\n";
            }
        }


        responseHeader += "\r\n";

        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseHeader + httpResponse.GetContent());

        stream.Write(responseBuffer, 0, responseBuffer.Length);
    }


    private string GetStatusMessage(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            404 => "Not Found",
            500 => "Internal Server Error"

        };
    }
}

