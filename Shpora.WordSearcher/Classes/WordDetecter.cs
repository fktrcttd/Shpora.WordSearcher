using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shpora.WordSearcher.Helpers;

namespace Shpora.WordSearcher
{
    public class WordDetecter
    {
        public String Host { get; set; }
        public String Token { get; set; }

        private bool[,] GameField { get; set; }
        private String GameFieldToString => MatrixHelper.FromBoolMAtrixToString(this.GameField, '0','1');
        private string[] GameFieldToLines => this.GameFieldToString.Replace("\r", String.Empty).Split('\n');
        
        private DateTime GameStartedIn { get; set; }
        private DateTime GameEndsIn { get; set; }

        private string FirstWordWasDetected { get; set; }
        public string FirstWordInRow { get; set; }
        public Int32 ToroidsLongtitude { get; set; } = 100;
        public Int32 ToroidsLongtitudeCounter { get; set; }

        public bool NeedCountToroidsLongtitude { get; set; }
        
        public List<string> Words { get; set; } = new List<string>();


        public Boolean GameIsClosed { get; set; }
        


        public WordDetecter(string host = "http://shpora.skbkontur.ru:81/", string token = "42a20533-a0f6-4a83-a81b-719ca0de7c5c")
        {
           
            this.Host = host;
            this.Token = token;
            using (var client = new GameClient(Host, Token))
            {
                client.FinishSession();
                var info = client.InitSession();
                if (info.Status == Status.Conflict)
                    client.InitSession();

                var sessionInfo = info.Value;
                this.GameEndsIn = DateTime.Now + sessionInfo.Expires;
                this.GameStartedIn = sessionInfo.Created;
                this.GameField = client.MakeMove(Direction.Down).Value;
            }
        }

        public void StartTheGame()
        {
            int i = 0;
            while (true)
            {
                var countUnitsAtTheBottom = MatrixHelper.CountUnitsAtBottomLine(this.GameFieldToString);
                if (countUnitsAtTheBottom > 0)
                    break;

                if (i != 0 && i % 100 == 0)
                    MoveToRight(7);
                i++;
                MoveToDown();
            }

            MoveToDown();
            var countUnitsAtBottomLine = MatrixHelper.CountUnitsAtBottomLine(GameFieldToString);
            if (countUnitsAtBottomLine > 0)
                DetectWordFromUnder();
            else
                DetectWordFromUpper();

            GoToFirstLetter();
            ReadWord(true);
            RunHorizontalLooping();
        }


        public void RunHorizontalLooping()
        {
            int counter = 0;
            string word = string.Empty;
            while (true)
            {
                MoveToRight();
                if (MatrixHelper.CountUnitsAtRightSide(GameFieldToString) > 0)
                {
                    MoveToRight(6);

                    char letter = LetterHelper.GetLetterFromMatrix(GameFieldToLines.ToList());
                    if (letter != 'E')
                    {
                        word = ReadWord();
                        
                    }
                    else
                    {
                        MoveToDown();
                        var countUnitsOnBottom = MatrixHelper.CountUnitsAtBottomLine(GameFieldToString);
                        if (countUnitsOnBottom > 0)
                            DetectWordFromUnder();
                        else
                            DetectWordFromUpper();

                        GoToFirstLetter();
                        word = ReadWord();

                        FirstWordInRow = word;
                        counter = 0;
                    }

                    if (counter!= 0 && counter % 100 == 0)
                    {
                        MoveToDown(7);
                    }
                    

                    //if (word == FirstWordWasDetected || (GameEndsIn <= DateTime.Now.AddSeconds(-10)))
                    //{
                    //    CloseSession();
                    //    break;
                    //}



                }
                counter++;
            }
        }
        

        private string ReadWord(bool isFirstWord = false)
        {
            var i = 0;
            string word = string.Empty;
            
            //while (!MatrixHelper.MatrixContainsWordEnd(GameFieldToLines.ToList()))
            while (true)
            {
                var letter = LetterHelper.GetLetterFromMatrix(GameFieldToLines.ToList());
                word += letter;
                if (MatrixHelper.MatrixContainsWordEnd(GetWidedMatrixToRight()))
                {
                    MoveToRight(2);
                    break;
                }
                MoveToRight(8);
            }
            if (isFirstWord)
                FirstWordWasDetected = word;

            if (String.IsNullOrEmpty(FirstWordInRow))
            {
                FirstWordInRow = word;
            }
            if (Words.Take(5).ToList().Contains(word))
            {
                FirstWordInRow = String.Empty;
                MoveToDown(10);
                return word;
            }
            Words.Add(word);
            return word;
        }

        #region Вспомогательные методы

        private List<string> GetWidedMatrixToRight()
        {
            var columnsAfterLetter = GameFieldToLines.ToList();

            MoveToRight(2);
            for (var index = 0; index < GameFieldToLines.ToList().Count; index++)
            {
                var s = GameFieldToLines.ToList()[index];

                columnsAfterLetter[index] = columnsAfterLetter[index].Replace("\n", string.Empty);
                columnsAfterLetter[index] += s[5];
                columnsAfterLetter[index] += s[6];
            }
            MoveToLeft(2);
            return columnsAfterLetter;
        }

        private void DetectWordFromUnder()
        {
            var firstLine = GameFieldToLines[0];
            int i = 0;
            while (!firstLine.Contains("1"))
            {
                MoveToDown();
                firstLine = GameFieldToLines[0];
                i++;
            }
        }

        private void DetectWordFromUpper()
        {
            var firstLine = GameFieldToLines[0];
            while (firstLine.Contains("1"))
            {
                MoveToUp();
                firstLine = GameFieldToLines[0];
            }
            MoveToDown();
        }

        private void GoToFirstLetter()
        {
            while (!MatrixHelper.MatrixContainsWordBegin(GameFieldToLines.ToList()))
                MoveToLeft();

            MoveToRight(2);
        }


        private void CloseSession()
        {
            GameIsClosed = true;
            Words = Words.Distinct().OrderBy(c => c.Length).ToList();
            
            using (var client = new GameClient(Host, Token))
            {
                client.SendWords(Words);
                Console.WriteLine($"Нашли {client.GetStatistics().Value.Words} слов. Сделали {client.GetStatistics().Value.Moves} ходов");
                Console.WriteLine($"Получили {client.FinishSession().Value.Points} очков");
            }
        }

        #endregion

        #region Moves

        private void MoveToUp(int times = 1)
        {
            using (var client = new GameClient(Host, Token))
            {
                var direction = Direction.Up;
                for (int i = 0; i < times - 1; i++)
                    client.MakeMove(direction);
                this.GameField = client.MakeMove(direction).Value;
            }
        }
        private void MoveToDown(int times = 1)
        {
            using (var client = new GameClient(Host, Token))
            {
                var direction = Direction.Down;
                for (int i = 0; i < times - 1; i++)
                    client.MakeMove(direction);
                this.GameField = client.MakeMove(direction).Value;
            }
        }

        private void MoveToLeft(int times = 1)
        {
            using (var client = new GameClient(Host, Token))
            {
                var direction = Direction.Left;
                for (int i = 0; i < times - 1; i++)
                    client.MakeMove(direction);
                this.GameField = client.MakeMove(direction).Value;
            }
        }
        private void MoveToRight(int times = 1)
        {
            using (var client = new GameClient(Host, Token))
            {
                var direction = Direction.Right;
                for (int i = 0; i < times - 1; i++)
                    client.MakeMove(direction);
                this.GameField = client.MakeMove(direction).Value;
            }
        }

        #endregion
    }


}
