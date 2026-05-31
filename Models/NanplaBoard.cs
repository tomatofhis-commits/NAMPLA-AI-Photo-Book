using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanpla.Models
{
    public class NanplaBoard
    {
        public NanplaCell[,] Cells { get; private set; }
        private int[,] _solution;

        public string BackgroundImage { get; set; } = string.Empty;

        public NanplaBoard()
        {
            Cells = new NanplaCell[9, 9];
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    Cells[r, c] = new NanplaCell();
                }
            }
            _solution = new int[9, 9];
        }

        public void GenerateBoard(Difficulty difficulty)
        {
            FillCompleteBoard();

            int cellsToRemove = difficulty switch
            {
                Difficulty.Easy => 30,
                Difficulty.Medium => 45,
                Difficulty.Hard => 55,
                _ => 40
            };

            RemoveNumbers(cellsToRemove);

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Cells[r, c].Value != 0)
                    {
                        Cells[r, c].IsOriginal = true;
                    }
                    else
                    {
                        Cells[r, c].IsOriginal = false;
                    }
                }
            }
        }

        private void FillCompleteBoard()
        {
            Random rand = new Random();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    _solution[r, c] = 0;
                }
            }

            Solve(_solution, rand);

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    Cells[r, c].Value = _solution[r, c];
                }
            }
        }

        private bool Solve(int[,] board, Random rand)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        List<int> numbers = Enumerable.Range(1, 9).OrderBy(x => rand.Next()).ToList();
                        foreach (int num in numbers)
                        {
                            if (IsSafe(board, row, col, num))
                            {
                                board[row, col] = num;
                                if (Solve(board, rand)) return true;
                                board[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsSafe(int[,] board, int row, int col, int num)
        {
            for (int x = 0; x < 9; x++)
                if (board[row, x] == num) return false;

            for (int x = 0; x < 9; x++)
                if (board[x, col] == num) return false;

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i + startRow, j + startCol] == num) return false;

            return true;
        }

        private void RemoveNumbers(int count)
        {
            Random rand = new Random();
            List<(int, int)> availableCells = new List<(int, int)>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Cells[r, c].Value != 0)
                    {
                        availableCells.Add((r, c));
                    }
                }
            }

            var shuffled = availableCells.OrderBy(x => rand.Next()).ToList();
            int removedCount = 0;
            int[,] tempBoard = new int[9, 9];

            for (int i = 0; i < shuffled.Count; i++)
            {
                if (removedCount >= count) break;

                var (r, c) = shuffled[i];
                int backupValue = Cells[r, c].Value;

                // Temporary clear the cell
                Cells[r, c].Value = 0;

                // Copy current board state to tempBoard
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        tempBoard[row, col] = Cells[row, col].Value;
                    }
                }

                // Check number of solutions (uniqueness)
                int solutions = CountSolutions(tempBoard);

                if (solutions == 1)
                {
                    // Uniqueness holds, confirm removal
                    removedCount++;
                }
                else
                {
                    // Multiple solutions or no solution, restore the value
                    Cells[r, c].Value = backupValue;
                }
            }
        }

        public void ValidateBoard()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    Cells[r, c].IsError = false;

            for (int r = 0; r < 9; r++)
            {
                var dict = new Dictionary<int, List<NanplaCell>>();
                for (int c = 0; c < 9; c++)
                {
                    int val = Cells[r, c].Value;
                    if (val != 0)
                    {
                        if (!dict.ContainsKey(val)) dict[val] = new List<NanplaCell>();
                        dict[val].Add(Cells[r, c]);
                    }
                }
                foreach (var pair in dict.Where(p => p.Value.Count > 1))
                    foreach (var cell in pair.Value)
                        cell.IsError = true;
            }

            for (int c = 0; c < 9; c++)
            {
                var dict = new Dictionary<int, List<NanplaCell>>();
                for (int r = 0; r < 9; r++)
                {
                    int val = Cells[r, c].Value;
                    if (val != 0)
                    {
                        if (!dict.ContainsKey(val)) dict[val] = new List<NanplaCell>();
                        dict[val].Add(Cells[r, c]);
                    }
                }
                foreach (var pair in dict.Where(p => p.Value.Count > 1))
                    foreach (var cell in pair.Value)
                        cell.IsError = true;
            }

            for (int br = 0; br < 3; br++)
            {
                for (int bc = 0; bc < 3; bc++)
                {
                    var dict = new Dictionary<int, List<NanplaCell>>();
                    for (int r = br * 3; r < br * 3 + 3; r++)
                    {
                        for (int c = bc * 3; c < bc * 3 + 3; c++)
                        {
                            int val = Cells[r, c].Value;
                            if (val != 0)
                            {
                                if (!dict.ContainsKey(val)) dict[val] = new List<NanplaCell>();
                                dict[val].Add(Cells[r, c]);
                            }
                        }
                    }
                    foreach (var pair in dict.Where(p => p.Value.Count > 1))
                        foreach (var cell in pair.Value)
                            cell.IsError = true;
                }
            }
        }

        private int CountSolutions(int[,] board, int limit = 2)
        {
            int count = 0;
            CountSolutionsHelper(board, ref count, limit);
            return count;
        }

        private bool CountSolutionsHelper(int[,] board, ref int count, int limit)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            if (IsSafe(board, row, col, num))
                            {
                                board[row, col] = num;
                                if (CountSolutionsHelper(board, ref count, limit))
                                {
                                    if (count >= limit)
                                    {
                                        board[row, col] = 0;
                                        return true;
                                    }
                                }
                                board[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            count++;
            return count >= limit;
        }

        public bool CheckWin()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Cells[r, c].Value != _solution[r, c])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
