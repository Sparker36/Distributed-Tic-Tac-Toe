using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class ClientProgram
    {
        private static readonly byte[] _incomingBuffer = new byte[1024];
        private const string END_OF_MESSAGE = "<END>";  //protocol-required end-of-message tag
        
        static void Main(string[] args)
        {
            #region Setup
            string BORDERLINE = String.Concat(Enumerable.Repeat("-", Console.WindowWidth));
            try
            {

                // 2. Establish a remote endpoint for the socket
                IPHostEntry HostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress IPAddress = HostInfo.AddressList[0];
                IPEndPoint RemoteEndPoint = new IPEndPoint(IPAddress, 1200);

                // 3. Create the socket
                Socket ServerCommunications = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            #endregion

                try
                {
                    // 4. Connect the socket to the remote endpoint
                    ServerCommunications.Connect(RemoteEndPoint);
                    Console.WriteLine($"Connected to server at {ServerCommunications.RemoteEndPoint}!\n{BORDERLINE}");

                    HandleConnection(ServerCommunications);
                }//try
                catch (Exception e)
                {

                    Console.WriteLine(e);
                }//catch
            }//try
            catch (Exception e)
            {
                Console.WriteLine(e);
            }//catch
        }//main

        /// <summary>
        /// Handles all activities expected from server once connected.
        /// </summary>
        /// <param name="ServerCommunications">Socket connection is made through.</param>
        private static void HandleConnection(Socket ServerCommunications)
        {
            string data = "";
            int index = 0, BytesReceived;

            while (true)        // Listen for Welcome message
            {
                BytesReceived = ServerCommunications.Receive(_incomingBuffer);
                data += Encoding.ASCII.GetString(_incomingBuffer, 0, BytesReceived);
                index = data.IndexOf(END_OF_MESSAGE);
                if (index > -1)
                {
                    break;
                }//if
            }//while

            var question = data.Substring(0, index);    //assemble question from server
            var choice = ChooseXO(question);            //get answer from user
            byte[] SendingMessage = Encoding.ASCII.GetBytes($"{choice}" + END_OF_MESSAGE);  //format answer for transfer
            int BytesSent = ServerCommunications.Send(SendingMessage);                      //send answer to server

            PlayGame(ServerCommunications);             //start waiting and answering gameplay communication

        }//HandleConnection(Socket)

        /// <summary>
        /// Handles communications between server specifically for gameplay.
        /// </summary>
        /// <param name="ServerCommunications">Socket connection is made through.</param>
        private static void PlayGame(Socket ServerCommunications)
        {
            int BytesReceived,index;
            string data = "";
            // Game loop
            do
            {
                BytesReceived = ServerCommunications.Receive(_incomingBuffer);          //wait for a message
                data = Encoding.ASCII.GetString(_incomingBuffer, 0, BytesReceived);     //unpack

                var messages = data.Split("<END>");                                     //in case multiple messages are in the buffer
                for(int ToProcess = 0;ToProcess < messages.Count() - 1; ++ToProcess)    //Split = 2 for single messages, so count -1. Process each message.
                {
                    var message = messages[ToProcess];
                    var length = message.Length;
                    var TypeTag = message.Substring(0, 3);                                 //get the message type tag
                    message = message.Substring(3, length - 3);                             //get the message without tags

                    if (TypeTag == "<T>")   //if the message is a <T>urn prompt message
                    {
                        bool valid = false;
                        int input = -1;
                        do              //until valid number chosen (0,1,2)
                        {
                            Console.Write(message);
                            valid = Int32.TryParse(Console.ReadLine(), out input);
                        } while (!valid && input >= 0 && input < 3);
                        ServerCommunications.Send(Encoding.ASCII.GetBytes(input.ToString()));
                    }//if
                    else        //if the message is a <B>oard display message or <E>rror
                    {
                        Console.WriteLine(message);
                    }//else
                }
            } while (data != "END OF GAME");
            EndGame(ServerCommunications);
        }//PlayGame(Socket)

        /// <summary>
        /// Wraps up server communications, then disconnects and closes socket.
        /// </summary>
        /// <param name="ServerCommunications">Socket connection is made through.</param>
        private static void EndGame(Socket ServerCommunications)//Socket ServerCommunications)
        {
            var BytesReceived = ServerCommunications.Receive(_incomingBuffer);
            var data = Encoding.ASCII.GetString(_incomingBuffer, 0, BytesReceived);
            var index = data.IndexOf(END_OF_MESSAGE);
            Console.WriteLine(data.Substring(3, index - 3));

            ServerCommunications.Shutdown(SocketShutdown.Both);
            ServerCommunications.Close();
        }//EndGame()

        /// <summary>
        /// Displays prompt sent from server to choose X or O, then accepts and validates user input.
        /// </summary>
        /// <param name="question">Prompt from server.</param>
        /// <returns>User choice of X or O</returns>
        private static string ChooseXO(string question)
        {
            string XO = "";
            var index = question.IndexOf("Would");
            var prompt = question.Substring(index, question.Length-index);
            do
            {
                Console.Write($"{question} ");
                XO = Console.ReadLine().ToLower();
            } while (XO != "x" && XO.ToLower() != "o");
            return XO;
        }//GetUserInput()
    }//ClientProgram
}//namespace Client
