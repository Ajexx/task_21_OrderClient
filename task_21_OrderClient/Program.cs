using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5000;

using TcpClient tcpClient = new TcpClient();

IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, port);

await tcpClient.ConnectAsync(iPEndPoint);

using var stream = tcpClient.GetStream();

while (true)
{
    Console.WriteLine("=====Menu=====");
    Console.WriteLine("1. Add new order");
    Console.WriteLine("2. Exit");
    Console.Write("> ");
    if (int.TryParse(Console.ReadLine(), out int choice))
    {
        switch (choice)
        {
            case 1:
                Console.WriteLine("Enter name(ORDER or another): ");
                Console.Write("> ");
                string order = Console.ReadLine() ?? "";

                Console.WriteLine("Enter product name: ");
                Console.Write("> ");
                string productName = Console.ReadLine() ?? "";

                Console.WriteLine("Enter product count: ");
                Console.Write("> ");
                string productCount = Console.ReadLine() ?? "";

                Console.WriteLine("Enter product price: ");
                Console.Write("> ");
                string productprice = Console.ReadLine() ?? "";

                var message = $"{order};{productName};{productCount};{productprice}";

                await SendLineAsync(stream, message);

                string? response = await ReadLineAsync(stream);

                if (response == null)
                {
                    Console.WriteLine("Server closed the connection.");
                    return;
                }

                Console.WriteLine($"Message from server: {response}");
                break;

            case 2:
                await SendLineAsync(stream, "EXIT");
                return;

            default:
                Console.WriteLine("Incorrect input!");
                break;
        }
    }

    else
    {
        Console.WriteLine("Incorrect input!");
    }

}
 async Task<string?> ReadLineAsync(NetworkStream stream)
{
    var bytes = new List<byte>();
    byte[] one = new byte[1];

    while (true)
    {
        int read = await stream.ReadAsync(one, 0, one.Length);

        if (read == 0)
        {
            return null;
        }

        if (one[0] == (byte)'\n')
        {
            break;
        }
        bytes.Add(one[0]);
    }
    var result = Encoding.UTF8.GetString(bytes.ToArray()).Trim();
    return result;
}

async Task SendLineAsync(NetworkStream stream, string text)
{
    var bytes = Encoding.UTF8.GetBytes(text + "\n");
    await stream.WriteAsync(bytes, 0, bytes.Length);
}
