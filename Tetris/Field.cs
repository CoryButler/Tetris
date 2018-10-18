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
        public char[,] PlayingField => _playingField;

        private Dictionary<char, ConsoleColor> _colorsForeground = new Dictionary<char, ConsoleColor>
        {
            { 'A',  ConsoleColor.Black},
            { 'B',  ConsoleColor.DarkGray},
            { 'C',  ConsoleColor.Gray},
            { '0',  ConsoleColor.Red},
            { '1',  ConsoleColor.DarkCyan},
            { '2',  ConsoleColor.DarkGreen},
            { '3',  ConsoleColor.Yellow},
            { '4',  ConsoleColor.Green},
            { '5',  ConsoleColor.Blue},
            { '6',  ConsoleColor.DarkYellow},
            { 'D',  ConsoleColor.White}
        };

        private Dictionary<char, char> _sprites = new Dictionary<char, char>
        {
            { 'A',  '▓' },
            { 'B',  '▓' },
            { 'C',  '▓' },
            { '0',  '▓' },
            { '1',  '▓' },
            { '2',  '▓' },
            { '3',  '▓' },
            { '4',  '▓' },
            { '5',  '▓' },
            { '6',  '▓' },
            { 'D',  '*' }
        };

        private readonly char[,] _map;
        private readonly char[,] _playingField;
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

            fieldRows.Add("CBBBBBBBBBBCCCCCCCCCCCCCCC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBNext Up:BBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBAAAAAABBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBAAAAAABBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBAAAAAABBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBAAAAAABBBBC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCCCCCCCCCCCCCCC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCBScore:BBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCBAAAAAAAAAAABC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CBBBBBBBBBBCBHigh Score:BC");
            fieldRows.Add("CBBBBBBBBBBCBAAAAAAAAAAABC");
            fieldRows.Add("CBBBBBBBBBBCBBBBBBBBBBBBBC");
            fieldRows.Add("CCCCCCCCCCCCCCCCCCCCCCCCCC");

            _map = new char[fieldRows[0].Length, fieldRows.Count];
            _playingField = new char[12, Height];
            _mapColor = new ConsoleColor[fieldRows[0].Length, fieldRows.Count];

            for (var y = 0; y < fieldRows.Count; y++)
            {
                for (var x = 0; x < fieldRows[y].Length; x++)
                {
                    _map[x, y] = fieldRows[y][x];
                    _mapColor[x, y] = _map[x, y] != 'B' ? ConsoleColor.White : ConsoleColor.Gray;
                    if (x < 12) _playingField[x, y] = _map[x, y];
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
                                _map[(tetromino.X + px), (tetromino.Y + py)] = tetromino.Sprite;
                                _mapColor[(tetromino.X + px), (tetromino.Y + py)] = tetromino.Color;
                            }
        }

        public async void UpdateFieldAsync(Tetromino tetromino)
        {
            for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                            if (tetromino.Shape[tetromino.Rotate(px, py, tetromino.Rotation)] != '.')
                            {
                                _playingField[(tetromino.X + px), (tetromino.Y + py)] = tetromino.Sprite;
                                _mapColor[(tetromino.X + px), (tetromino.Y + py)] = tetromino.Color;
                            }
        }

        public void ResetField(Tetromino tetromino)
        {
            for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                            if (tetromino.Shape[tetromino.Rotate(px, py, tetromino.Rotation)] != '.')
                            {
                                _map[(tetromino.X + px), (tetromino.Y + py)] = 'B';
                                _mapColor[(tetromino.X + px), (tetromino.Y + py)] = ConsoleColor.Gray;
                            }
        }

        public void DrawField()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    Console.SetCursorPosition(x + 2, y + 2);
                    Console.ForegroundColor = _mapColor[x, y];// != 'B' ? ConsoleColor.White : ConsoleColor.Gray;
                    Console.Write(_map[x, y]);
                }
        }

        public void DrawField2(Tetromino tetromino)
        {
            for (int x = 0; x < _playingField.GetLength(0); x++)
                for (int y = 0; y < _playingField.GetLength(1); y++)
                {
                                Console.SetCursorPosition(x + 2, y + 2);
                                Console.ForegroundColor = _colorsForeground[_playingField[x, y]];
                                Console.Write(_sprites[_playingField[x, y]]);
                    for (var tx = 0; tx < 4; tx++)
                        for (var ty = 0; ty < 4; ty++)
                            if (tetromino.X + tx == x && tetromino.Y + ty == y
                                && tetromino.Shape[tetromino.Rotate(tx, ty, tetromino.Rotation)] != '.')
                            {
                                Console.SetCursorPosition(x + 2, y + 2);
                                Console.ForegroundColor = tetromino.Color;
                                Console.Write(_sprites[tetromino.Sprite]);
                            }
                }
        }
    }
}
