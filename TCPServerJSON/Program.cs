using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

Console.WriteLine("TcpServerTest");

TcpListener listener = new TcpListener(IPAddress.Any, 7);
listener.Start();
while (true)
{
    TcpClient socket = listener.AcceptTcpClient();
    IPEndPoint iPEndPoint = socket.Client.RemoteEndPoint as IPEndPoint;

    Console.WriteLine("client connected");
    Task.Run(() => HandleClient(socket));
}

listener.Stop();

void HandleClient(TcpClient socket)
{
    NetworkStream ns = socket.GetStream();
    StreamReader reader = new StreamReader(ns);
    StreamWriter writer = new StreamWriter(ns);
    writer.AutoFlush = true;

    while (socket.Connected)
    {
        try
        {
            string message = reader.ReadLine();
            if (string.IsNullOrEmpty(message))
            {
                writer.WriteLine(JsonSerializer.Serialize(new { error = "Du skal sende noget" }));
                continue;
            }

            var jsonMessage = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(message);
            var method = jsonMessage["method"].GetString();

            Console.WriteLine(message);

            switch (method)
            {
                case "Metoder":
                    var metoderResult = new
                    {
                        message = "Hello World",
                        methods = new[] { "Add", "Subtract", "Random", "Stop" }
                    };
                    writer.WriteLine(JsonSerializer.Serialize(metoderResult));
                    break;

                case "Tak":
                    var takResult = new { message = "Det var så lidt" };
                    writer.WriteLine(JsonSerializer.Serialize(takResult));
                    break;

                case "Stop":
                    var stopResult = new { message = "Goodbye World" };
                    writer.WriteLine(JsonSerializer.Serialize(stopResult));
                    writer.Flush();
                    socket.Close();
                    break;

                case "Add":
                    writer.WriteLine(JsonSerializer.Serialize(new { message = "Input Numbers" }));
                    string addNumbersInput = reader.ReadLine();
                    Console.WriteLine(addNumbersInput);

                    var addNumbers = addNumbersInput.Split(new[] { ' ', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (addNumbers.Length == 2 && int.TryParse(addNumbers[0], out int addNum1) && int.TryParse(addNumbers[1], out int addNum2))
                    {
                        int addResult = addNum1 + addNum2;
                        writer.WriteLine(JsonSerializer.Serialize(new { result = addResult }));
                    }
                    else
                    {
                        writer.WriteLine(JsonSerializer.Serialize(new { error = "Invalid input. Please use the format <num1> <num2>" }));
                    }
                    break;

                case "Subtract":
                    writer.WriteLine(JsonSerializer.Serialize(new { message = "Input Numbers" }));
                    string subtractNumbersInput = reader.ReadLine();
                    Console.WriteLine(subtractNumbersInput);

                    var subtractNumbers = subtractNumbersInput.Split(new[] { ' ', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (subtractNumbers.Length == 2 && int.TryParse(subtractNumbers[0], out int subtractNum1) && int.TryParse(subtractNumbers[1], out int subtractNum2))
                    {
                        int subtractResult = subtractNum1 - subtractNum2;
                        writer.WriteLine(JsonSerializer.Serialize(new { result = subtractResult }));
                    }
                    else
                    {
                        writer.WriteLine(JsonSerializer.Serialize(new { error = "Invalid input. Please use the format <num1> <num2>" }));
                    }
                    break;

                case "Random":
                    writer.WriteLine(JsonSerializer.Serialize(new { message = "Input Numbers" }));
                    string randomNumbersInput = reader.ReadLine();
                    Console.WriteLine(randomNumbersInput);

                    var randomNumbers = randomNumbersInput.Split(new[] { ' ', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    if (randomNumbers.Length == 2 && int.TryParse(randomNumbers[0], out int randomNum1) && int.TryParse(randomNumbers[1], out int randomNum2))
                    {
                        if (randomNum1 > randomNum2)
                        {
                            writer.WriteLine(JsonSerializer.Serialize(new { error = "Invalid input. Please num1 has to be smaller than num2" }));
                        }
                        else
                        {
                            Random random = new Random();
                            int randomResult = random.Next(randomNum1, randomNum2);
                            writer.WriteLine(JsonSerializer.Serialize(new { result = randomResult }));
                        }
                    }
                    else
                    {
                        writer.WriteLine(JsonSerializer.Serialize(new { error = "Invalid input. Please use the format <num1> <num2>" }));
                    }
                    break;

                default:
                    writer.WriteLine(JsonSerializer.Serialize(new { error = "Unknown method" }));
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            socket.Close();
        }
    }
}