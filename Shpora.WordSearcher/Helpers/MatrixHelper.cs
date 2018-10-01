using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shpora.WordSearcher.Helpers
{
    static class MatrixHelper
    {
        public static string FromBoolMAtrixToString(bool[,] map, char empty, char full)
        {
            var sb = new StringBuilder();
            for (var row = 0; row < map.GetLength(0); row++)
            {
                for (var column = 0; column < map.GetLength(1); column++)
                {
                    sb.Append(map[row, column] ? full : empty);
                }
                sb.Append("\n");
            }

            //удаляем ненужный перенос строки
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static int CountUnitsInMatrix(string matrix)
        {
            if (String.IsNullOrWhiteSpace(matrix))
                return 0;
            if (!matrix.Contains('1'))
                return 0;
            return matrix.Split('1').Count() - 1;
        }

        public static int CountUnitsAtBottomLine(string matrix)
        {
            if (String.IsNullOrWhiteSpace(matrix))
                return 0;
            if (!matrix.Contains('1'))
                return 0;

            return matrix.Split('\n').Last().Split('1').Count() - 1;
        }

        public static int CountUnitsAtRightSide(string matrix)
        {
            if (String.IsNullOrWhiteSpace(matrix))
                return 0;
            if (!matrix.Contains('1'))
                return 0;
            var splittedMatrix = matrix.Replace("\r", String.Empty).Split('\n');
            var rightColumn = new StringBuilder();
            foreach (var s in splittedMatrix)
                rightColumn.Append(s.Last());

            return rightColumn.ToString().Split('1').Count() - 1;
        }


        public static int CountZeroColumnsAtTheBeginning(List<string> matrixToList)
        {
            //считаем количество столбцов, которые необходимо убрать
            var countOfColumns = matrixToList[0].Length;
            foreach (var line in matrixToList)
            {
                int counter = 0;
                if (line.StartsWith("1"))
                {
                    countOfColumns = 0;
                    continue;
                }


                foreach (var symbol in line)
                {
                    if (symbol == '0')
                        counter++;
                    else
                        break;
                }
                if (counter < countOfColumns)
                    countOfColumns = counter;
            }
            return countOfColumns;
        }

        public static bool MatrixContainsWordBegin(List<string> widedMatrix)
        {
            var leftColumns = new StringBuilder();
            foreach (var s in widedMatrix)
            {
                leftColumns.Append(s[0]);
                leftColumns.Append(s[1]);
                leftColumns.Append("\n");
            }
            var columnsToString = leftColumns.ToString();
            var result = !columnsToString.Contains("1");

            return result;
        }

        public static bool MatrixContainsWordEnd(List<string> widedMatrix)
        {
            var columnsAfterLetter = new StringBuilder();

            foreach (var s in widedMatrix)
            {
                columnsAfterLetter.Append(s[7]);
                columnsAfterLetter.Append(s[8]);
                columnsAfterLetter.Append("\n");
            }
            var columnsToString = columnsAfterLetter.ToString();
            var result = !columnsToString.Contains("1");

            return result;
        }

    }
}
