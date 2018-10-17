using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Tetromino
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        private List<string> _shapes;
        private int _shapeIndex;
        private List<ConsoleColor> _colors;
        private const int _speed = 1;

        public string Shape => _shapes[_shapeIndex];
        public ConsoleColor Color => _colors[_shapeIndex];
        public int Rotation { get; set; } = 0;
        public int X { get; set; } = 4;
        public int Y { get; set; } = 0;

        public Tetromino()
        {
            _shapes = InitShapes();
            _colors = InitColors();
            _shapeIndex = new Random().Next() % _shapes.Count;
        }

        public bool CanMoveDown(Field field)
        {
            return WillFitAtDestination(X, Y + 1, field);
        }

        private List<string> InitShapes()
        {
            List<string> shapes = new List<string>(7);

            StringBuilder sb = new StringBuilder();
            sb.Append("..X.");
            sb.Append("..X.");
            sb.Append("..X.");
            sb.Append("..X.");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append("..X.");
            sb.Append(".XX.");
            sb.Append(".X..");
            sb.Append("....");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append(".X..");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("....");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append(".XX.");
            sb.Append("....");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append("..X.");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("....");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("..X.");
            shapes.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append(".X..");
            sb.Append(".X..");
            shapes.Add(sb.ToString());
            sb.Clear();

            return shapes;
        }

        private List<ConsoleColor> InitColors()
        {
            return new List<ConsoleColor>
            {
                ConsoleColor.Red,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkGreen,
                ConsoleColor.Yellow,
                ConsoleColor.Green,
                ConsoleColor.Blue,
                ConsoleColor.DarkYellow
            };
        }

        public int Rotate(int x, int y, int rotation)
        {
            switch(rotation % 4)
            {
                case 0: return y * 4 + x;
                case 1: return 12 + y - (x * 4);
                case 2: return 15 - (y * 4) - x;
                case 3: return 3 - y + (x * 4);
            }
            return 0;
        }

        public void UpdatePositionInField(Field field)
        {
            bool[] keys = new bool[4];
            for (int i = 0; i < 4; i++)				   // R   L   D Z
                    keys[i] = (0x8000 & GetAsyncKeyState((char)("\x27\x25\x28Z"[i]))) != 0;

                X += (keys[0] && WillFitAtDestination(X + _speed, Y, field)) ? _speed : 0;
		        X -= (keys[1] && WillFitAtDestination(X - _speed, Y, field)) ? _speed : 0;		
                Y += (keys[2] && WillFitAtDestination(X, Y + _speed, field)) ? _speed : 0;
        }

        public bool WillFitAtDestination(int destinationX, int destinationY, Field field)
        {
            for (var x = 0; x < 4; x++)
                for (var y = 0; y < 4; y++)
                {
                    int tetrominoIndex = Rotate(x, y, Rotation);
                    int fieldIndexX = destinationX + x;
                    int fieldIndexY = destinationY + y;

                    if (destinationX + x >= 0 && destinationX + x < field.Width)
                        if (destinationY + y >= 0 && destinationY + y < field.Height)
                            if (Shape[tetrominoIndex] == 'X' && field.Bounds[fieldIndexX, fieldIndexY] != '░')
                                return false;
                }

            return true;
        }
    }
}
