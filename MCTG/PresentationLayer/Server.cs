using System.Net.Sockets;
using System.Net;
using System.Text;
using MCTG.DAL;
using System.Reflection.PortableExecutable;
using MCTG.Classes;

namespace MCTG.PresentationLayer;

public class Server
{
    public readonly int PORT = 8080;

    private readonly RequestHandler _requestHandler;

    public Server()
    {
        var userRepository = new UserRepository();
        _requestHandler = new RequestHandler(userRepository);
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
            string jsonBody = httpMethod == "POST" ? request.Split("\r\n\r\n")[1] : null;

            HttpResponse response = _requestHandler.HandleRequest(requestUrl, httpMethod, jsonBody);




            SendHttpResponse(stream, response);

        }
    }


    private void SendHttpResponse(NetworkStream stream, HttpResponse httpResponse)
    {
        string statusCode = httpResponse.getStatusCode().ToString();
        string statusMessage = "test"; //TODO: Ändern!


        string responseHeader = $"HTTP/1.1 {statusCode} {statusMessage}\r\n";

        // Füge alle angegebenen Header hinzu
        if (httpResponse.GetHeader() != null)
        {
            foreach (var header in httpResponse.GetHeader())
            {
                responseHeader += $"{header.Key}: {header.Value}\r\n";
            }
        }


        // Füge eine leere Zeile nach den Headern hinzu (Trennung von Header und Body)
        responseHeader += "\r\n";

        // Konvertiere Header und Body in Bytes
        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseHeader + httpResponse.GetContent());

        // Sende die Antwort zurück über den Stream
        stream.Write(responseBuffer, 0, responseBuffer.Length);
    }


    private string GetStatusMessage(int statusCode)
    {
        // Liefert die entsprechende Nachricht für den HTTP-Statuscode
        return statusCode switch
        {
            200 => "OK",
            404 => "Not Found",
            500 => "Internal Server Error"

        };
    }
}

