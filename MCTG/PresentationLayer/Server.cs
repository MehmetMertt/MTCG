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
        CardRepository.InitDb(DBCONNECTIONSTRING);
        _requestHandler = new RequestHandler();
    }

    public void Start()
    {
        // Starte den TCP-Listener auf Port 8080, der eingehende Verbindungen akzeptiert
        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine($"Server started, listening on port {PORT}...");

        while (true)
        {
            // Akzeptiere eingehende TCP-Verbindungen
            using TcpClient client = listener.AcceptTcpClient();
            using NetworkStream? stream = client.GetStream();

            // Erstellt einen Puffer (ein Array von Bytes), um die eingehenden Daten zu speichern.
            // Die Größe des Puffers wird basierend auf der maximalen Empfangsgröße des Clients festgelegt.
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Parse die HTTP-Anfrage aus dem eingehenden TCP-Datenstrom
            string request =
                Encoding.UTF8.GetString(buffer, 0, bytesRead); //Konvertieren der bytes aus buffer in string
            string[] requestLines = request.Split("\r\n");
            string httpMethod = requestLines[0].Split(' ')[0]; // Erhalte die HTTP-Methode (GET, POST, DELETE)
            string requestUrl = requestLines[0].Split(' ')[1]; // Erhalte die URL der Anfrage

            // Parse Body for POST requests
            string jsonBody = request.Split("\r\n\r\n")[1];

            HttpResponse response = _requestHandler.HandleRequest(requestUrl, httpMethod, jsonBody);




            SendHttpResponse(stream, response);

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

