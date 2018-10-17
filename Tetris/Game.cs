using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    class Game
    {
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

                tetromino.UpdatePositionInField(field);

                if (forceDown)
                {
                    // Update difficulty every 50 pieces
                    speedCount = 0;
                    pieceCount++;
                    if (pieceCount % 50 == 0)
                        if (speed >= 10) speed--;
                    if (tetromino.WillFitAtDestination(tetromino.X, tetromino.Y + 1, field))
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
                                    field.Bounds[(tetromino.X + coordinateX), (tetromino.Y + coordinateY)] = '▓';
                                    field.Map[(tetromino.X + coordinateX), (tetromino.Y + coordinateY)] = '▓';
                                }

                        // Check for lines
                        for (int coordinateY = 0; coordinateY < 4; coordinateY++)
                            if (tetromino.Y + coordinateY < field.Height - 1)
                            {
                                bool makesLine = true;
                                for (int coordinateX = 1; coordinateX < field.Bounds.GetLength(0) - 1; coordinateX++)
                                {
                                    makesLine &= (field.Map[coordinateX, tetromino.Y + coordinateY]) != 0;
                                }

                                if (makesLine)
                                {
                                    // Remove Line, set to =
                                    for (int coordinateX = 1; coordinateX < field.Bounds.GetLength(0) - 1; coordinateX++)
                                        field.Map[coordinateX, tetromino.Y + coordinateY] = '*';

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
                        _isGameOver = !tetromino.WillFitAtDestination(tetromino.X, tetromino.Y, field);
                    }

                    field.UpdateField(tetromino);
                    field.DrawField();
                    field.ResetField(tetromino);
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
