using GameAI.GamePlaying;
using GameAI.GamePlaying.Core;
using System;
using System.Collections.Generic;

namespace GameAI.GamePlaying
{
    public class StudentAI : Behavior
    {
        public StudentAI()
        {
           //Nothing to add :p
        }

        public List<ComputerMove> generateValidMoves(int _color, Board _board) 
        {
            List<ComputerMove> curList = new List<ComputerMove>();
            for (int i = 0; i < Board.Height; i++)
            {
                for (int j = 0; j < Board.Width; j++)
                {
                    if (_board.IsValidMove(_color, i, j))
                    {
                        ComputerMove temp = new ComputerMove(i, j);
                        curList.Add(temp);
                    }
                }
            }
            return curList;
        }

        public ComputerMove Run(int _color, Board _board, int _lookAheadDepth)
        {
            ComputerMove bestMove = null;
            Board curBoard = new Board();
            List<ComputerMove> moveList = generateValidMoves(_color, _board);

            foreach (ComputerMove move in moveList)
            {
                curBoard.Copy(_board);
                curBoard.MakeMove(_color, move.row, move.column);

                if (curBoard.IsTerminalState() || _lookAheadDepth == 0)
                {
                    move.rank = Evaluate(curBoard);         
                }
                else
                {
                    if (curBoard.HasAnyValidMove(_color * -1))
                    {
                        ComputerMove curRun = Run(_color * -1, curBoard, _lookAheadDepth - 1);
                        move.rank = curRun.rank;
                    }
                    else
                    {
                        ComputerMove curRun = Run(_color, curBoard, _lookAheadDepth - 1);
                        move.rank = curRun.rank;
                    }
                }

                if (bestMove == null || ((_color == Board.White) && (move.rank > bestMove.rank)) || ((_color == Board.Black) && (move.rank < bestMove.rank)))
                {
                    bestMove = move;
                }
            }
            return bestMove;
        }

        private int Evaluate(Board _board)
        {
            int totalValue = 0;
            int totalScore = 0;
            for (int i = 0; i < Board.Height; i++)
            {
                for (int j = 0; j < Board.Width; j++)
                {
                    int curColor = _board.GetTile(i, j);
                    totalScore += curColor;
                    if ((i == 0 || i == Board.Height - 1))
                    {
                        if ((j == 0 || j == Board.Width - 1))
                        {
                            curColor *= 100; //Tile in corner
                        }
                        else
                        {
                            curColor *= 10; //Top or Bottom Row and not in corner
                        }
                    }
                    else if ((j == 0 || j == Board.Width - 1))
                    {
                        curColor *= 10;
                    }
                    totalValue += curColor;
                }
            }
            if (_board.IsTerminalState())
            {
                if (totalScore > 0)
                {
                    totalValue += 10000;
                }
                else if (totalScore < 0)
                {
                    totalValue -= 10000;
                }
            }

            return totalValue;
        }
    }
}

