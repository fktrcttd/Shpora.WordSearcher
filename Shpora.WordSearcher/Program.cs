using System;
using System.Text;

namespace Shpora.WordSearcher
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var searcher = new WordDetecter();
            searcher.StartTheGame();
            Console.ReadKey();
        }
    }
}
