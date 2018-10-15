using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            GameLoop();

        }

        private void GameLoop()
        {
            while(!_isGameOver)
            {

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
