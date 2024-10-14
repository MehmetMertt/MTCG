using MCTG.Classes;
using MCTG.PresentationLayer;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


/*internal class Program
{

    public static void Main(string[] args)
    {
        User userA = new User("Mehmet", "StarkesPassword123");
        User userB = new User("Max", "StaerkeresPassword123");
        userA.BuyPackage();


        userA.Authentication.GenerateToken();
        Console.WriteLine(userA.Authentication.getToken());
    }
}*/


var server = new Server();
server.Start();