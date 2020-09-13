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
            string board = $" {_board[0,0]} | {_board[0, 1]} | {_board[0, 2]} {Environment.NewLine}" +
                           $"---+---+---{Environment.NewLine}" +
                           $" {_board[1, 0]} | {_board[1, 1]} | {_board[1, 2]} {Environment.NewLine}" +
                           $"---+---+---{Environment.NewLine}" +
                           $" {_board[2,0]} | {_board[2,1]} | {_board[2,2]} {Environment.NewLine}";
            return board;
        }//

        public bool InsertX(int row, int col)
        {
            if (row <= _board.GetUpperBound(0) && row>=0 &&
                col <= _board.GetUpperBound(1) && col>=0 &&
                this[row,col] == ' ')
            {
                _board[row, col] = 'X';
                return true;
            }
            else
                return false;
        }//insertx

        public bool InsertO(int row, int col)
        {
            if (row <= _board.GetUpperBound(0) && row >= 0 &&
                col <= _board.GetUpperBound(1) && col >= 0 &&
                this[row, col] == ' ')
            {
                _board[row, col] = 'O';
                return true;
            }
            else
                return false;
        }

        public char ReportResult()
        {
            char[] units = { 'X', 'O' };
            bool XWin = false;
            bool OWin = false;

            foreach(var unit in units)
            {
                if (CheckColumnWin(unit) || CheckRowWin(unit) || CheckDiagonalWin(unit))
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

            var NoResult = CheckNoResult();
            if (!OWin && !XWin && NoResult)
                return 'N';
            else if (OWin && XWin || !NoResult)
                return 'D';
            else if (OWin && !XWin)
                return 'O';
            else
                return 'X';
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

        private bool CheckNoResult()
        {
            for(int row = 0; row <= _board.GetUpperBound(0); ++row)
            {
                for(int col = 0; col <= _board.GetUpperBound(1); ++col)
                {
                    if (this[row, col] == ' ')
                        return true; ;
                }
            }
            return false; ;
        }
    }//class
}

