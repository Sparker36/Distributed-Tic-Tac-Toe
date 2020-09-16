using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TicTacToeApp.Logic;

namespace Server
{
    class ServerProgram
    {
        private const string END_OF_MESSAGE = "<END>";
        static void Main(string[] args)
        {
            #region setup
            //  Allocate a buffer to store incoming data
            byte[] IncomingBuffer = new byte[1024];
            var app = new ServerProgram();

            //  Establish a local endpoint for the socket
            IPHostEntry HostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress IPAddress = HostInfo.AddressList[0];
            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress, 1200);

            //  Create the socket
            Socket listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Log server ip
            var BorderLine = String.Concat(Enumerable.Repeat("-", Console.WindowWidth));
            Console.WriteLine($"Server IP : {LocalEndPoint}\n{BorderLine}");
            #endregion

            try
            {
                //  Bind the socket to the local endpoint
                listener.Bind(LocalEndPoint);

                //  Listen for incoming connections
                listener.Listen(1);

                //  Enter a loop
                while (true)
                {
                    //  Listen for a connection (the program is blocked)
                    Console.WriteLine("Waiting for connection...");
                    Socket handler = listener.Accept();
                    Console.WriteLine($"Connection accepted!");

                    HandleConnection(handler);
                }//while

            }//try
            catch (Exception e)
            {
                Console.WriteLine($"EXCEPTION : {e}");
            }//catch
        }//main

        /// <summary>
        /// Displays welcome message, accepts player choice of X or O, and launches game.
        /// </summary>
        /// <param name="handler">Socket connection is made through.</param>
        private static void HandleConnection(Socket handler)
        {
            byte[] IncomingBuffer = new byte[1024];
            int index;
            // Greet client and prompt for X/O
            SendMessage(handler, "Welcome to the Tic-Tac-Server!\nWould you like to play as [X]'s or [O]'s?");

            string data = "";
            //  Process the connection to read the incoming data
            while (true)
            {
                int BytesReceived = handler.Receive(IncomingBuffer);
                data += Encoding.ASCII.GetString(IncomingBuffer, 0, BytesReceived);
                index = data.IndexOf("<END>");
                if (index > -1)
                {
                    break;
                }//if
            }//while

            //  Process the incoming data
            var player = data.Substring(0, index);
            Console.WriteLine($"RESPONSE : User chose to play as {player}'s");      //log user choice to server console

            PlayTicTacToe(handler, player.ToUpper());     //play the game
            EndConnection(handler);
        }//handlerequest(socket)

        /// <summary>
        /// Wraps up communication with client and closes socket.
        /// </summary>
        /// <param name="handler">Socket connection is made through.</param>
        private static void EndConnection(Socket handler)
        {
            Console.WriteLine("Closing the connection...");
            SendMessage(handler, "<M>Thank you for playing!");

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }//EndConnection(Socket)

        /// <summary>
        /// Encodes a given string to a byte buffer, adds protocol-defined end-of-message tag, and sends through socket.
        /// </summary>
        /// <param name="handler">Socket connection is made through.</param>
        /// <param name="msg">Given string to send to client.</param>
        private static void SendMessage(Socket handler, string msg)
        {
            byte[] message = Encoding.ASCII.GetBytes(msg+END_OF_MESSAGE);
            handler.Send(message);
        }//SendMessage(Socket, string)

        /// <summary>
        /// Provides game functionality. Prompts user for location and generates AI turns.
        /// </summary>
        /// <param name="handler">Socket connection is made through.</param>
        /// <param name="player">Player's choice of X or O</param>
        private static void PlayTicTacToe(Socket handler, string player)
        {
            var board = new TicTacToeBoard();
            int row, col;
            char result;
            Random rng = new Random();

            //Take turns
            if(player == "X")       //If the player chose to play X
            {
                SendMessage(handler, $"<B>{board}");    //Display original (empty) board
                result = board.ReportResult();

                do          //Turn loop. Start with X (player), check for victory conditions, then continue to O and repeat.
                {
                    Turn(handler, board, player);
                    SendMessage(handler, $"<B>{board}\n");
                    result = board.ReportResult();
                    if (result != 'N') { break; }

                    Turn(handler, board, "O", rng);
                    SendMessage(handler, $"<B>{board}\n");
                    result = board.ReportResult();
                    if (result != 'N') { break; }
                } while (result == 'N');
            }//if
            else                    //If the player chose to play O
            {
                SendMessage(handler, $"<B>{board}");
                result = board.ReportResult();

                do
                {
                    Turn(handler, board, "X", rng);
                    SendMessage(handler, $"<B>{board}\n");
                    result = board.ReportResult();
                    if (result != 'N') { break; }

                    Turn(handler, board, player);
                    SendMessage(handler, $"<B>{board}\n");
                    result = board.ReportResult();
                    if (result != 'N') { break; }
                } while (result == 'N');
            }//else

            // Generate and display winner messages
            switch (result)
            {
                case 'X':
                    SendMessage(handler, "<M>X wins the game!\n");
                    Console.WriteLine("GAME EVENT : X wins the game!");
                    break;
                case 'O':
                    SendMessage(handler, "<M>O wins the game!\n");
                    Console.WriteLine("GAME EVENT : O wins the game!");
                    break;
                case 'D':
                    SendMessage(handler, "<M>The game is a draw.\n");
                    Console.WriteLine("GAME EVENT : The game is a draw.");
                    break;
            }//switch

            SendMessage(handler, $"<B>{board}");     //show final board
            SendMessage(handler, "<M>END OF GAME");  //send end of game message to trigger connection closing
        }//PlayTicTacToe()

        /// <summary>
        /// Turn functionality.
        /// If user turn, prompts for row, then column, then attempts to insert at chosen location. 
        /// If AI turn, generates 2 random integers from 0 to 3 for row and column, then attempts to insert at that location.
        /// </summary>
        /// <param name="handler">Socket connection is made through.</param>
        /// <param name="board">Existing TicTacToeBoard</param>
        /// <param name="player">X or O to place</param>
        /// <param name="rng">Randum number generator. If null, it is user turn. If it exists, AI turn.</param>
        private static void Turn(Socket handler, TicTacToeBoard board, string player, Random rng = null)
        {
            int row, col, BytesReceived;
            byte[] IncomingBuffer = new byte[1024];
            bool valid = false;

            do                  //attempt to place token, repeat until valid location chosen
            {
                if(rng == null) //if rng is null, we're processing player actions
                {
                    //Get row
                    SendMessage(handler, $"<T>Please enter the row to place an {player}: ");
                    BytesReceived = handler.Receive(IncomingBuffer);
                    var x = Encoding.ASCII.GetString(IncomingBuffer, 0, BytesReceived);
                    row = Int32.Parse(x);

                    //Get column
                    SendMessage(handler, $"<T>Please enter the column to place an {player}: ");
                    BytesReceived = handler.Receive(IncomingBuffer);
                    col = Int32.Parse(Encoding.ASCII.GetString(IncomingBuffer, 0, BytesReceived));

                    if (!board.CheckPosition(row, col)) HandleInvalid(handler, row, col, player, true);
                }//if
                else            //if rng is not null, it's ai move
                {
                    row = rng.Next(0, 3);
                    col = rng.Next(0, 3);

                    //Log move to server, whether valid or invalid.
                    if (!board.CheckPosition(row, col))
                    {
                        HandleInvalid(handler, row, col, player, false);
                    }
                    else
                    {
                        SendMessage(handler, $"<M>Computer played at ({row},{col})");
                    }
                }//else

                valid = board.Insert(row, col, player[0]);
            } while (!valid);
            Console.WriteLine($"GAME EVENT : {player} at ({row},{col})");
        }//Turn(Socket, TicTacToeBoard, string, Random)

        /// <summary>
        /// If invalid position is chosen, sends the row, column, and player marker to the server for logging.
        /// </summary>
        /// <param name="handler">Socket connection is made through</param>
        /// <param name="row">Row of board chosen</param>
        /// <param name="col">Column of board chosen</param>
        /// <param name="player">X or O attempted to be played</param>
        /// <param name="isPlayer">Boolean flag to display invalid location message to player or not</param>
        private static void HandleInvalid(Socket handler, int row, int col, string player, bool isPlayer)
        {
            if(isPlayer) SendMessage(handler, "<E>\nInvalid location. Please pick a valid location.\n");
            Console.WriteLine($"GAME EVENT : Invalid placement. {player} at ({row},{col})");
        }//HandleInvalid
    }//serverprogram
}//namespace
