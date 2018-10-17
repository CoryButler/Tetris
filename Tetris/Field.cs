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
        public char[,] Map => _map;
        public char[,] Bounds => _bounds;

        private readonly char[,] _map;
        private readonly char[,] _bounds;
        private readonly ConsoleColor[,] _mapColor;
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

            _map = new char[fieldRows[0].Length, fieldRows.Count];
            _bounds = new char[12, Height];
            _mapColor = new ConsoleColor[fieldRows[0].Length, fieldRows.Count];

            for (var y = 0; y < fieldRows.Count; y++)
            {
                for (var x = 0; x < fieldRows[y].Length; x++)
                {
                    _map[x, y] = fieldRows[y][x];
                    _mapColor[x, y] = _map[x, y] != '░' ? ConsoleColor.White : ConsoleColor.Gray;
                    if (x < 12) _bounds[x, y] = _map[x, y];
                }
            }

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

        public void UpdateField(Tetromino tetromino)
        {
            for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                            if (tetromino.Shape[tetromino.Rotate(px, py, tetromino.Rotation)] != '.')
                            {
                                _map[(tetromino.X + px), (tetromino.Y + py)] = '▓';
                                _mapColor[(tetromino.X + px), (tetromino.Y + py)] = tetromino.Color;
                            }
        }

        public void ResetField(Tetromino tetromino)
        {
            for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                            if (tetromino.Shape[tetromino.Rotate(px, py, tetromino.Rotation)] != '.')
                            {
                                _map[(tetromino.X + px), (tetromino.Y + py)] = '░';
                                _mapColor[(tetromino.X + px), (tetromino.Y + py)] = ConsoleColor.Gray;
                            }
        }

        public void DrawField()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    Console.SetCursorPosition(x + 2, y + 2);
                    Console.ForegroundColor = _mapColor[x, y];// != '░' ? ConsoleColor.White : ConsoleColor.Gray;
                    Console.Write(_map[x, y]);
                }
        }
    }
}
