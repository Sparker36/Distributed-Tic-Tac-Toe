using System;
using TicTacToeApp.Logic;

namespace TicTacToeApp
{
    public class TicTacToeProgram
    {
        static void Main(string[] args)
        {
            TicTacToeBoard sut = new TicTacToeBoard();
            int row, col;
            Console.WriteLine(sut.ToString());
            var result = sut.ReportResult();
            do
            {
                Console.WriteLine("Please insert the row to place an X: ");
                row = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Please insert the col to place an X: ");
                col = Int32.Parse(Console.ReadLine());
                sut.InsertX(row, col);
                Console.WriteLine(sut.ToString());

                result = sut.ReportResult();
                if (result != 'N') { break; }

                Console.WriteLine("Please insert the row to place an O: ");
                row = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Please insert the col to place an O: ");
                col = Int32.Parse(Console.ReadLine());
                sut.InsertO(row, col);
                Console.WriteLine(sut.ToString()); ;
                result = sut.ReportResult();
            } while (result == 'N');

            switch (result)
            {
                case 'X':
                    Console.WriteLine("X wins the game!");
                    break;
                case 'O':
                    Console.WriteLine("O wins the game!");
                    break;
                case 'D':
                    Console.WriteLine("The game is a draw.");
                    break;
            }//switch

            Console.WriteLine(sut.ToString());
            Console.ReadKey();
        }//main
    }
}
