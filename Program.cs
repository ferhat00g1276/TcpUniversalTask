using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

var endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.110"), 27001);
var server = new TcpListener(endPoint);

server.Start();
Console.WriteLine("Server is running");

while (true)
{
    using (var client = server.AcceptTcpClient())
    {
        using (var stream = client.GetStream())
        {
            var fileNameBytes = new byte[1024];
            var fileNameLength = stream.Read(fileNameBytes, 0, fileNameBytes.Length);
            var fileName = Encoding.UTF8.GetString(fileNameBytes, 0, fileNameLength);
            var fileLengthBytes = new byte[1024];
            int length = stream.Read(fileLengthBytes, 0, fileLengthBytes.Length);
            var fileLengthStr = Encoding.UTF8.GetString(fileLengthBytes, 0, length).Trim();
            int fileLength = int.Parse(fileLengthStr);
            var path = $"{DateTime.Now:HH.mm.ss}{Path.GetExtension(fileName)}";
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var totalReceivedBytes = 0;
                while (true)
                {
                    var buffer = new byte[5000];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    fileStream.Write(buffer, 0, bytesRead);
                    totalReceivedBytes += bytesRead;
                    if (totalReceivedBytes >= fileLength)
                        break;
                }
            }
            Console.WriteLine("File Downloaded: " + path);
        }
    }
}
