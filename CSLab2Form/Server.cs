using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSLab2Form
{
    class Server
    {
        private TcpListener tcpListener;
        private String path;
        private Thread listenThread;
        private String[] files;
        private String log;
        formServer form;

        public Server(int PORT, String PATH, formServer FORM)
        {
            tcpListener = new TcpListener(IPAddress.Any, PORT);
            path = PATH;
            files = Directory.GetFiles(path, "*.html");
            form = FORM;
        }

        public void Stop()
        {
            log += "Server shut down\r\n";
            form.UpdatingTextBox(log);
            tcpListener.Stop();
            listenThread.Abort();
        }

        public void Start()
        {
            log += "Server is up\r\n";
            form.UpdatingTextBox(log);
            listenThread = new Thread(() => ListenForClients());
            listenThread.Start();
        }

        public void ListenForClients()
        {
            tcpListener.Start();
            log += "Waiting...\r\n";
            form.UpdatingTextBox(log);
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(() =>
                {
                    using (NetworkStream clientStream = client.GetStream())
                    {
                        log += "Connected...\r\n";
                        form.UpdatingTextBox(log);
                        int bytesRead;
                        Byte[] messageReceived = new byte[1024];
                        bytesRead = clientStream.Read(messageReceived, 0, messageReceived.Length);
                        String stringReceived = Encoding.ASCII.GetString(messageReceived, 0, bytesRead);
                        log += "Received: " + stringReceived + "\r\n";
                        var seeker = stringReceived.IndexOf(@"GET /");
                        String fileToSend;
                        String fileName;

                        if (seeker != -1)
                        {
                            if (stringReceived.Substring(seeker)[5] == ' ')
                            {
                                fileName = @"index.html";
                            }
                            else
                            {
                                String[] shreddedString = stringReceived.Substring(seeker+5).Split(' ');
                                fileName = shreddedString[0];

                                if (!files.Contains(path + @"\" + fileName))
                                {
                                    fileName = @"404.html";
                                }
                            }


                            fileToSend = File.ReadAllText(path + @"\" + fileName);

                            String head = "HTTP/1.1 200 Ok\n Server: localhost\n Date: " + DateTime.Now.ToString() + "\n\n";
                            Byte[] message = Encoding.ASCII.GetBytes(head + fileToSend);

                            clientStream.Write(message, 0, message.Length);
                            log += "Sent: " + fileName + "\r\n";

                            form.UpdatingTextBox(log);
                        }
                        
                    }
                    client.Close();
                }
                );
                clientThread.Start();
            }
        }
    }
}
