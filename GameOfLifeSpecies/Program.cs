using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        private int?[,] board;
        private int cases = 0;
        private int species = 2;
        private int xDim = 85;
        private int yDim = 155;
        private bool eyeStrain = false;

        public Program()
        {
            board = new int?[xDim, yDim];
            bool validInput = false;
            Console.Write("Please enter an integer number of at least 1 (Over 1 breaks the board into sections) :");
            while (!validInput)
            {
                var input = Console.ReadLine();
                int.TryParse(input, out cases);

                if (cases > 0)
                    validInput = true;
                else
                    Console.Write("Please enter an integer number of at least 1:");

            }

            // validInput = false;
            // Console.Write("Enable worse eye strain? (Not recommended) y/n: ");
            // while (!validInput)
            // {
            //     var input = Console.ReadLine();
            //
            //     if (input.ToLower() == "y")
            //     {
            //         validInput = true;
            //         eyeStrain = true;
            //     }
            //     else if (input.ToLower() == "n")
            //     {
            //         validInput = true;
            //     }
            //     else
            //         Console.Write("Please enter y or n:");
            //
            // }

            var rand = new Random();
            var xSize = xDim / cases;
            var ySize = yDim / cases;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (cases > 1 && ((i + 1) % xSize == 0 || (j + 1) % ySize == 0))
                        board[i, j] = null;
                    else if (rand.Next(100) > 95)
                        board[i, j] = 1;
                    else if (rand.Next(100) > 93)
                        board[i, j] = 2;
                    else
                        board[i, j] = 0;
                }
            }

        }

        public void NextState()
        {
            int?[,] newBoard = new int?[board.GetLength(0), board.GetLength(1)];
            Array.Copy(board, newBoard, board.Length);

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] != null)
                        newBoard[i, j] = GetNewState(i, j, board[i, j]);
                }
            }

            board = newBoard;
        }

        private int? GetNewState(int x, int y, int? currentState)
        {
            if (currentState == null)
            {
                return null;
            }

            var neighborSpecies = GetNumNeighbors(x, y);
            int otherSpecies = neighborSpecies.Sum() - neighborSpecies[0];

            if (currentState != 0)
            {
                otherSpecies -= neighborSpecies[(int)currentState];

                if (eyeStrain)
                {
                    if ((neighborSpecies[(int) currentState] % 3 == 2 && neighborSpecies[(int)currentState] < 3)||
                        otherSpecies > 0)
                        return 0;

                    return currentState;
                }
                else if (neighborSpecies[(int)currentState] < 3 ||
                    otherSpecies > 0)
                    return 0;
                else
                    // else condition implicitly satisfies a live cell count == 2 || count == 3
                    return currentState;
            }
            else
            {
                int maxIndex = 0;
                int maxValue = 0;
                for (int i = 1; i < species + 1; i++)
                {
                    if (maxValue < neighborSpecies[i])
                    {
                        maxValue = neighborSpecies[i];
                        maxIndex = i;
                    }
                }
                
                if (maxValue % 3 == 2 && maxValue > 1)
                    return maxIndex;
                else
                    return 0;
            }
        }

        private int[] GetNumNeighbors(int x, int y)
        {
            int[] neighborSpecies = new int[species + 1];

            if (x > 0)
            {
                if (board[x - 1, y] > 0)
                    neighborSpecies[(int)board[x - 1, y]]++;
            }
            if (x > 2)
            {
                if (board[x - 2, y] > 0 &&
                    board[x - 1, y] != null)
                    neighborSpecies[(int)board[x - 2, y]]++;
            }
            if (x > 0 && y > 0)
            {
                if (board[x - 1, y - 1] > 0)
                    neighborSpecies[(int)board[x - 1, y - 1]]++;
            }
            if (x > 1 && y > 0)
            {
                if (board[x - 2, y - 1] > 0 &&
                    board[x - 1, y - 1] != null)
                    neighborSpecies[(int)board[x - 2, y - 1]]++;
            }
            if (x > 0 && y > 1)
            {
                if (board[x - 1, y - 2] > 0 &&
                    board[x - 1, y - 1] != null)
                    neighborSpecies[(int)board[x - 1, y - 2]]++;
            }
            if (x > 2 && y > 2)
            {
                if (board[x - 2, y - 2] > 0 &&
                    board[x - 1, y - 1] != null &&
                    board[x - 1, y - 2] != null &&
                    board[x - 2, y - 1] != null)
                    neighborSpecies[(int)board[x - 2, y - 2]]++;
            }
            if (y > 0)
            {
                if (board[x, y - 1] > 0)
                    neighborSpecies[(int)board[x, y - 1]]++;
            }
            if (y > 1)
            {
                if (board[x, y - 2] > 0 &&
                    board[x, y - 1] != null)
                    neighborSpecies[(int)board[x, y - 2]]++;
            }
            if (x < board.GetLength(0) - 1 && y > 0)
            {
                if (board[x + 1, y - 1] > 0)
                    neighborSpecies[(int)board[x + 1, y - 1]]++;
            }
            if (x < board.GetLength(0) - 1 && y > 1)
            {
                if (board[x + 1, y - 2] > 0 &&
                    board[x + 1, y - 1] != null)
                    neighborSpecies[(int)board[x + 1, y - 2]]++;
            }
            if (x < board.GetLength(0) - 2 && y > 0)
            {
                if (board[x + 2, y - 1] > 0 &&
                    board[x + 1, y - 1] != null)
                    neighborSpecies[(int)board[x + 2, y - 1]]++;
            }
            if (x < board.GetLength(0) - 2 && y > 1)
            {
                if (board[x + 2, y - 2] > 0 &&
                    board[x + 1, y - 1] != null &&
                    board[x + 2, y - 1] != null &&
                    board[x + 1, y - 2] != null)
                    neighborSpecies[(int)board[x + 2, y - 2]]++;
            }
            if (x < board.GetLength(0) - 1)
            {
                if (board[x + 1, y] > 0)
                    neighborSpecies[(int)board[x + 1, y]]++;
            }
            if (x < board.GetLength(0) - 2)
            {
                if (board[x + 2, y] > 0 &&
                    board[x + 1, y] != null)
                    neighborSpecies[(int)board[x + 2, y]]++;
            }
            if (x < board.GetLength(0) - 1 && y < board.GetLength(1) - 1)
            {
                if (board[x + 1, y + 1] > 0)
                    neighborSpecies[(int)board[x + 1, y + 1]]++;
            }
            if (x < board.GetLength(0) - 2 && y < board.GetLength(1) - 1)
            {
                if (board[x + 2, y + 1] > 0 &&
                    board[x + 1, y + 1] != null)
                    neighborSpecies[(int)board[x + 2, y + 1]]++;
            }
            if (x < board.GetLength(0) - 1 && y < board.GetLength(1) - 2)
            {
                if (board[x + 1, y + 2] > 0 &&
                    board[x + 1, y + 1] != null)
                    neighborSpecies[(int)board[x + 1, y + 2]]++;
            }
            if (x < board.GetLength(0) - 2 && y < board.GetLength(1) - 2)
            {
                if (board[x + 2, y + 2] > 0 &&
                    board[x + 1, y + 1] != null &&
                    board[x + 2, y + 1] != null &&
                    board[x + 1, y + 2] != null)
                    neighborSpecies[(int)board[x + 2, y + 2]]++;
            }
            if (y < board.GetLength(1) - 1)
            {
                if (board[x, y + 1] > 0)
                    neighborSpecies[(int)board[x, y + 1]]++;
            }
            if (y < board.GetLength(1) - 2)
            {
                if (board[x, y + 2] > 0 &&
                    board[x, y + 1] != null)
                    neighborSpecies[(int)board[x, y + 2]]++;
            }
            if (x > 0 && y < board.GetLength(1) - 1)
            {
                if (board[x - 1, y + 1] > 0)
                    neighborSpecies[(int)board[x - 1, y + 1]]++;
            }
            if (x > 0 && y < board.GetLength(1) - 2)
            {
                if (board[x - 1, y + 2] > 0 &&
                    board[x - 1, y + 1] != null)
                    neighborSpecies[(int)board[x - 1, y + 2]]++;
            }

            return neighborSpecies;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] != 0 && board[i, j] != null)
                        sb.Append($"{board[i, j]} ");
                    else if (board[i, j] == null)
                        sb.Append("+ ");
                    else
                        sb.Append("  ");
                }
                sb.Append("\n\r");
            }

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            Console.Title = "Modified Version of Nick's Version of Conway's Game of Life";
            var game = new Program();

            while (true)
            {
                Console.Clear();
                Console.WriteLine(game.ToString());
                game.NextState();
                Thread.Sleep(50);
            }
        }
    }
}
