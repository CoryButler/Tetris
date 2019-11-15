using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    class Game
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        private const int _speed = 1;
        private bool _rotateHold;

        private bool _isGameOver = false;
        private const string _respondYes = "Y";
        private const string _respondNo = "N";

        public Game()
        {
            Console.CursorVisible = false;
            GameLoop();

        }

        private void GameLoop()
        {
            Field field = new Field();
            Tetromino tetromino = new Tetromino();
            field.UpdateField(tetromino);

                //tetromino.UpdatePositionInField(field);
            
            bool forceDown = true;
            var speedCount = 0;
            int speed = 20;
	        int pieceCount = 0;
            List<int> lines = new List<int>();

            while (!_isGameOver)
            {
                Thread.Sleep(50);

                speedCount++;
                forceDown = (speedCount == speed);

                bool[] keys = new bool[4];
                for (int i = 0; i < 4; i++)                     // R   L   D Z
                    keys[i] = (0x8000 & GetAsyncKeyState((char)("\x27\x25\x28Z"[i]))) != 0;

                tetromino.X += (keys[0] && tetromino.WillFitAtDestination(tetromino.X + _speed, tetromino.Y, tetromino.Rotation, field)) ? _speed : 0;
                tetromino.X -= (keys[1] && tetromino.WillFitAtDestination(tetromino.X - _speed, tetromino.Y, tetromino.Rotation, field)) ? _speed : 0;
                tetromino.Y += (keys[2] && tetromino.WillFitAtDestination(tetromino.X, tetromino.Y + _speed, tetromino.Rotation, field)) ? _speed : 0;
                if (keys[3])
                {
                    tetromino.Rotation += (_rotateHold && tetromino.WillFitAtDestination(tetromino.X, tetromino.Y, tetromino.Rotation + 1, field)) ? 1 : 0;
                    _rotateHold = false;
                }
                else
                    _rotateHold = true;

                if (forceDown)
                {
                    // Update difficulty every 50 pieces
                    speedCount = 0;
                    pieceCount++;
                    if (pieceCount % 50 == 0)
                        if (speed >= 10) speed--;
                    if (tetromino.WillFitAtDestination(tetromino.X, tetromino.Y + 1, tetromino.Rotation, field))
                    {
                        tetromino.Y++;
                    }
                    else
                    {
                        // It can't! Lock the piece in place
                        for (int coordinateX = 0; coordinateX < 4; coordinateX++)
                            for (int coordinateY = 0; coordinateY < 4; coordinateY++)
                                if (tetromino.Shape[tetromino.Rotate(coordinateX, coordinateY, tetromino.Rotation)] != '.')
                                {
                                    field.PlayingField[(tetromino.X + coordinateX), (tetromino.Y + coordinateY)] = tetromino.Sprite;
                                    field.PlayingField[(tetromino.X + coordinateX), (tetromino.Y + coordinateY)] = tetromino.Sprite;
                                }

                        // Check for lines
                        for (int coordinateY = 0; coordinateY < 4; coordinateY++)
                            if (tetromino.Y + coordinateY < field.Height - 1)
                            {
                                bool makesLine = true;
                                for (int coordinateX = 1; coordinateX < field.PlayingField.GetLength(0) - 1; coordinateX++)
                                {
                                    makesLine &= (field.PlayingField[coordinateX, tetromino.Y + coordinateY]) != 'B';
                                }

                                if (makesLine)
                                {
                                    // Remove Line, set to =
                                    for (int coordinateX = 1; coordinateX < field.PlayingField.GetLength(0) - 1; coordinateX++)
                                        field.PlayingField[coordinateX, tetromino.Y + coordinateY] = 'D';

                                    lines.Add(tetromino.Y + coordinateY);
                                }
                            }

                        //score += 25;
                        //if(lines.Count > 0)	score += (lines.Count) * 100;

                        //// Pick New Piece
                        //currentX = _fieldWidth / 2;
                        //currentY = 0;
                        //currentRotation = 0;
                        //currentPiece = new Random().Next() % 7;
                        tetromino = new Tetromino();

                        // If piece does not fit straight away, game over!
                        _isGameOver = !tetromino.WillFitAtDestination(tetromino.X, tetromino.Y, tetromino.Rotation, field);
                    }
                }

                if (forceDown || keys.Any(k => k == true))
                {
                    field.UpdateField(tetromino);
                    field.DrawField2(tetromino);
                    field.ResetField(tetromino);

                    // Animate Line Completion
                    if (lines.Count > 0)
                    {
                        // Display Frame (cheekily to draw lines)
                        Thread.Sleep(400); // Delay a bit

                        foreach (var line in lines)
                            for (int px = 1; px < field.PlayingField.GetLength(0) - 1; px++)
                            {
                                for (int py = line; py > 0; py--)
                                    field.PlayingField[px, py] = field.PlayingField[px, py - 1];

                                field.PlayingField[px, 0] = 'B';
                            }

                        lines.Clear();
                    }

                }
            }
        }

        private bool ResultsScreen()
        {
            ClearInputBuffer();

            var response = string.Empty;
            while (!IsValidResponse(response))
            {
                Console.Write("Play again?");
                response = Console.ReadKey(false).KeyChar.ToString();
            }

            // TODO: This switch statement is dumb.
            switch(response)
            {
                case _respondYes:
                    return true;
                case _respondNo:
                    return false;
                default:
                    return false;
            }
        }

        private void ClearInputBuffer()
        {
            while(Console.KeyAvailable) Console.ReadKey(false);
        }

        private bool IsValidResponse(string response)
        {
            return string.Equals(response, _respondYes, StringComparison.OrdinalIgnoreCase)
                || string.Equals(response, _respondNo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
