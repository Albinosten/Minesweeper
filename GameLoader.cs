using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Minesweeper
{
    public class GameLoader
    {
        private static string path => "temp.txt";

        public IList<int> LoadResult()
        {
            var result = new List<int>();

            if(File.Exists(path))
            {
                var lines = File.ReadAllLines(path, Encoding.UTF8);
                foreach(var line in lines)
                {
                    result.Add(int.Parse(line));
                }
            }

            return result;
        }

        public void SaveResult(IList<int> results)
        {
            var file = File.Create(path);
            var streamWriter = new StreamWriter(file);

            foreach(var result in results)
            {
                streamWriter.WriteLine(result);
            }
            streamWriter.Close();
            file.Close();
        }
    }
}