using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Field
    {
        public int Width => _map.GetLength(0);
        public int Height => _map.GetLength(1);

        private readonly int[,] _map;
        private readonly int _highScore = 300;
        private readonly Tuple<int, int> _highScoreCoordinate = new Tuple<int, int>(13, 15);
        private readonly Tuple<int, int> _scoreCoordinate = new Tuple<int, int>(13, 12);
        private readonly List<ConsoleColor> _scoreColors = new List<ConsoleColor>()
        {
            ConsoleColor.Yellow,
            ConsoleColor.Green
        };

        public Field()
        {
            List<string> fieldRows = new List<string>();

            fieldRows.Add("▓░░░░░░░░░░▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░Next Up:░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░      ░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░      ░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░      ░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░      ░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░Score:░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░           ░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░High Score:░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░           ░▓");
            fieldRows.Add("▓░░░░░░░░░░▓░░░░░░░░░░░░░▓");
            fieldRows.Add("▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");

            for (var y = 0; y < fieldRows.Count; y++)
                for (var x = 0; x < fieldRows[y].Length; x++)
                    _map[x, y] = fieldRows[y][x];

            WriteScore(0);
        }
        
        // TODO: Move WriteScore(int score) to Game class.
        public void WriteScore(int score)
        {
            Console.SetCursorPosition(_scoreCoordinate.Item1, _scoreCoordinate.Item2);
            Console.ForegroundColor = (score < _highScore) ? _scoreColors[0] : _scoreColors[1];
            Console.Write(score);
            
            Console.SetCursorPosition(_highScoreCoordinate.Item1, _highScoreCoordinate.Item2);
            Console.ForegroundColor = (score > _highScore) ? _scoreColors[0] : _scoreColors[1];
            Console.Write(score);

            Console.ResetColor();
        }
    }
}
