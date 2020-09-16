using System;

namespace TicTacToeApp.Logic
{
    public class TicTacToeBoard
    {
        private char[,] _board = new char[3, 3];
        public char this[int row, int col]
        {
            get
            {
                return _board[row, col];
            }
        }

        public TicTacToeBoard()
        {
            for(int i = 0; i<=_board.GetUpperBound(0);++i)
            {
                for(int j = 0; j<=_board.GetUpperBound(1);++j)
                {
                    _board[i, j] = ' ';
                }//for columns
            }//for rows
        }//ctor

        public override string ToString()
        {
            string board = $"    0   1   2 \n" +
                           $"0   {_board[0,0]} | {_board[0, 1]} | {_board[0, 2]} {Environment.NewLine}" +
                           $"   ---+---+---{Environment.NewLine}" +
                           $"1   {_board[1, 0]} | {_board[1, 1]} | {_board[1, 2]} {Environment.NewLine}" +
                           $"   ---+---+---{Environment.NewLine}" +
                           $"2   {_board[2,0]} | {_board[2,1]} | {_board[2,2]} {Environment.NewLine}";
            return board;
        }//

        public bool CheckPosition(int row, int col)
        {
            return (row <= _board.GetUpperBound(0) && row >= 0 &&
                col <= _board.GetUpperBound(1) && col >= 0 &&
                this[row, col] == ' ');
        }

        public bool Insert(int row, int col, char player)
        {
            if (CheckPosition(row, col))
            {
                _board[row, col] = player;
                return true;
            }
            else
                return false;
        }//insert

        public char ReportResult()
        {
            char[] units = { 'X', 'O' };
            bool XWin = false,
                 OWin = false,
                 ColWin = false,
                 RowWin = false,
                 DiagWin = false;


            foreach (var unit in units)
            {
                ColWin = CheckColumnWin(unit);
                RowWin = CheckRowWin(unit);
                DiagWin = CheckDiagonalWin(unit);

                if (ColWin || RowWin || DiagWin)
                {
                    switch (unit)
                    {
                        case 'X':
                            XWin = true;
                            break;

                        case 'O':
                            OWin = true;
                            break;
                    }
                }
            }

            var NoResult = CheckNoResult(XWin, OWin);
            if (!OWin && !XWin && NoResult)
                return 'N';
            else if (OWin && !XWin)
                return 'O';
            else if (!OWin && XWin)
                return 'X';
            else //if (OWin && XWin || !NoResult)
                return 'D';
        }

        private bool CheckColumnWin(char unit)
        {
            for(int i = 0; i < 3; ++i)
            {
                if (_board[0, i] == unit)
                    if (_board[1, i] == unit)
                        if (_board[2, i] == unit)
                            return true;
            }
            return false;
        }

        private bool CheckRowWin(char unit)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (_board[i, 0] == unit)
                    if (_board[i, 1] == unit)
                        if (_board[i, 2] == unit)
                            return true;
            }
            return false;
        }

        private bool CheckDiagonalWin(char unit)
        {
            if (_board[0, 0] == unit)
                if (_board[1, 1] == unit)
                    if (_board[2, 2] == unit)
                        return true;
            if (_board[0, 2] == unit)
                if (_board[1, 1] == unit)
                    if (_board[2, 0] == unit)
                        return true;
            return false;
        }

        private bool CheckNoResult(bool XWin, bool OWin)
        {
            if (XWin || OWin) return false;

            for(int row = 0; row <= _board.GetUpperBound(0); ++row)
            {
                for(int col = 0; col <= _board.GetUpperBound(1); ++col)
                {
                    if (this[row, col] == ' ')
                        return true;
                }
            }
            return false;
        }
    }//class
}

