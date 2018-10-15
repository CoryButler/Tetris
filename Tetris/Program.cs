using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
        }
    }

    class Game_orig
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        List<ConsoleColor> _colors = new List<ConsoleColor>()
        {
            ConsoleColor.DarkGray,
            ConsoleColor.Red,
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkGreen,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.DarkYellow,
            ConsoleColor.White,
            ConsoleColor.Gray
        };

        List<string> _tetrominos = new List<string>(7);

        private const int _screenWidth = 32;
        private const int _screenHeight = 32;

        private const int _fieldWidth = 12;
        private const int _fieldHeight = 18;
        private int[,] _field2D;

        public Game_orig()
        {

            // Create Screen Buffer
            char[,] _screen2D = new char[_screenWidth, _screenHeight];
	        ConsoleColor[,] _screenColor2D = new ConsoleColor[_screenWidth, _screenHeight];

            for (int x = 0; x < _screenWidth; x++)
                for (int y = 0; y < _screenHeight; y++)
                    _screen2D[x, y] = ' ';

            Console.SetWindowSize((int)_screenWidth, (int)_screenHeight);
            Console.SetBufferSize((int)_screenWidth, (int)_screenHeight);

            StringBuilder sb = new StringBuilder();
            sb.Append("..X.");
            sb.Append("..X.");
            sb.Append("..X.");
            sb.Append("..X.");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append("..X.");
            sb.Append(".XX.");
            sb.Append(".X..");
            sb.Append("....");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append(".X..");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("....");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append(".XX.");
            sb.Append("....");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append("..X.");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("....");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append("..X.");
            sb.Append("..X.");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            sb.Append("....");
            sb.Append(".XX.");
            sb.Append(".X..");
            sb.Append(".X..");
            _tetrominos.Add(sb.ToString());
            sb.Clear();

            _field2D = new int[_fieldWidth, _fieldHeight];
            for (var x = 0; x < _fieldWidth; x++)
                for (var y = 0; y < _fieldHeight; y++)
                {
                    _field2D[x, y] = (x == 0 || x == _fieldWidth - 1 || y == _fieldHeight - 1) ? 9 : 0;
                }


            bool isGameOver = false;
            bool forceDown = true;

            bool[] keys = { false, false, false, false };
            bool rotateHold = true;
            int currentPiece = new Random().Next() % 7;
            int currentRotation = 0;
            int currentX = _fieldWidth / 2;
            int currentY = 0;
            var speedCount = 0;
            int speed = 20;
	        int pieceCount = 0;
	        int score = 0;
            List<int> lines = new List<int>();

            Console.CursorVisible = false;
            while (isGameOver == false)
            {
                Thread.Sleep(50);

                speedCount++;
                forceDown = (speedCount == speed);

                for (int i = 0; i < 4; i++)				   // R   L   D Z
                    keys[i] = (0x8000 & GetAsyncKeyState((char)("\x27\x25\x28Z"[i]))) != 0;

                currentX += (keys[0] && DoesPieceFit(currentPiece, currentRotation, currentX + 1, currentY)) ? 1 : 0;
		        currentX -= (keys[1] && DoesPieceFit(currentPiece, currentRotation, currentX - 1, currentY)) ? 1 : 0;		
                currentY += (keys[2] && DoesPieceFit(currentPiece, currentRotation, currentX, currentY + 1)) ? 1 : 0;

                if (keys[3])
		        {
			        currentRotation += (rotateHold && DoesPieceFit(currentPiece, currentRotation + 1, currentX, currentY)) ? 1 : 0;
			        rotateHold = false;
		        }
		        else
                    rotateHold = true;

                // Force the piece down the playfield if it's time
		        if (forceDown)
		        {
			        // Update difficulty every 50 pieces
			        speedCount = 0;
			        pieceCount++;
			        if (pieceCount % 50 == 0)
				        if (speed >= 10) speed--;
                    			
			        // Test if piece can be moved down
			        if (DoesPieceFit(currentPiece, currentRotation, currentX, currentY + 1))
				        currentY++; // It can, so do it!
			        else
			        {
                        // It can't! Lock the piece in place
                        for (int coordinateX = 0; coordinateX < 4; coordinateX++)
                            for (int coordinateY = 0; coordinateY < 4; coordinateY++)
                                if (_tetrominos[currentPiece][Rotate(coordinateX, coordinateY, currentRotation)] != '.')
                                {
                                    _field2D[(currentX + coordinateX), (currentY + coordinateY)] = currentPiece + 1;
                                }

				        // Check for lines
				        for (int coordinateY = 0; coordinateY < 4; coordinateY++)
					        if(currentY + coordinateY < _fieldHeight - 1)
					        {
						        bool makesLine = true;
                                for (int coordinateX = 1; coordinateX < _fieldWidth - 1; coordinateX++)
                                {
                                    makesLine &= (_field2D[coordinateX, currentY + coordinateY]) != 0;
                                }

						        if (makesLine)
						        {
                                    // Remove Line, set to =
                                    for (int coordinateX = 1; coordinateX < _fieldWidth - 1; coordinateX++)
                                        _field2D[coordinateX, currentY + coordinateY] = 8;

							        lines.Add(currentY + coordinateY);
						        }						
					        }
                        
				        score += 25;
				        if(lines.Count > 0)	score += (lines.Count) * 100;

				        // Pick New Piece
				        currentX = _fieldWidth / 2;
				        currentY = 0;
				        currentRotation = 0;
				        currentPiece = new Random().Next() % 7;

				        // If piece does not fit straight away, game over!
				        isGameOver = !DoesPieceFit(currentPiece, currentRotation, currentX, currentY);
			        }
                }

                // Draw Screen
                if (forceDown || keys.Any(k => k == true))
                {
                    // Draw Field
                    for (int x = 0; x < _fieldWidth; x++)
                        for (int y = 0; y < _fieldHeight; y++)
                        {
                            _screen2D[x, y] = "░▓▓▓▓▓▓▓×▓"[_field2D[x, y]];
                            _screenColor2D[x, y] = _colors[_field2D[x, y]];
                        }

                    // Draw Current Piece
                    for (int px = 0; px < 4; px++)
                        for (int py = 0; py < 4; py++)
                            if (_tetrominos[currentPiece][Rotate(px, py, currentRotation)] != '.')
                            {
                                _screen2D[(currentX + px), (currentY + py)] = '▓';
                                _screenColor2D[(currentX + px), (currentY + py)] = _colors[currentPiece + 1];
                            }

                    for (int x = 0; x < _fieldWidth; x++)
                        for (int y = 0; y < _fieldHeight; y++)
                        {
                            Console.ForegroundColor = _screenColor2D[x, y];
                            Console.SetCursorPosition(x + 2, y + 2);
                            Console.Write(_screen2D[x, y]);
                        }

                    Console.ResetColor();

                    // Animate Line Completion
                    if (lines.Count > 0)
                    {
                        // Display Frame (cheekily to draw lines)
                        Thread.Sleep(400); // Delay a bit

                        foreach (var line in lines)
                            for (int px = 1; px < _fieldWidth - 1; px++)
                            {
                                for (int py = line; py > 0; py--)
                                    _field2D[px, py] = _field2D[px, py - 1];

                                _field2D[px, 0] = 0;
                            }

                        lines.Clear();
                    }

                    // Draw Score
                    Console.SetCursorPosition(0, 0);
                    Console.Write("Score: {0}", score);
                }
            }
            
            // TODO: Aspect Ratio
            // TODO: Refactor all

            Console.Clear();
            Console.SetCursorPosition(2, 2);
            Console.Write("Game Over!     Score: {0}", score);
            Console.SetCursorPosition(2, 4);
            Console.Write("Press ENTER to quit.");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ReadLine();
        }

        int Rotate(int tetrominoX, int tetrominoY, int rotation)
        {
            switch(rotation % 4)
            {
                case 0: return tetrominoY * 4 + tetrominoX;
                case 1: return 12 + tetrominoY - (tetrominoX * 4);
                case 2: return 15 - (tetrominoY * 4) - tetrominoX;
                case 3: return 3 - tetrominoY + (tetrominoX * 4);
            }
            return 0;
        }

        bool DoesPieceFit(int tetromino, int rotation, int tetrominoX, int tetrominoY)
        {
            for (var coordinateX = 0; coordinateX < 4; coordinateX++)
                for (var coordinateY = 0; coordinateY < 4; coordinateY++)
                {
                    int tetrominoIndex = Rotate(coordinateX, coordinateY, rotation);
                    int fieldIndexX = tetrominoX + coordinateX;
                    int fieldIndexY = tetrominoY + coordinateY;

                    if (tetrominoX + coordinateX >= 0 && tetrominoX + coordinateX < _fieldWidth)
                        if (tetrominoY + coordinateY >= 0 && tetrominoY + coordinateY < _fieldHeight)
                            if (_tetrominos[tetromino][tetrominoIndex] == 'X' && _field2D[fieldIndexX, fieldIndexY] != 0)
                                return false;
                }

            return true;
        }
    }
}
