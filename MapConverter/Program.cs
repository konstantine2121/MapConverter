using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MapConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var infile = "input.txt";
            var result = "result.txt";

            var converter = new MapConverter();

            converter.ConvertMap(infile, result);
        }
    }

    class MapConverter
    {
        private Size _mapSize;
        private bool[,] _map;

        private char _space = ' ';
        private char _food = (char)254;
        private char _gate = (char)196;

        public void ConvertMap(string inputFilePath, string outputFile)
        {
            if (LoadMap(inputFilePath))
            {
                char emptyCell = ' ';
                var newMap = new char[_mapSize.Height, _mapSize.Width];

                StringBuilder mapBuilder = new StringBuilder();

                for (int y = 0; y < _mapSize.Height; y++) 
                {
                    for (int x = 0; x < _mapSize.Width; x++)
                    {
                        if (_map[y, x])
                        {
                            newMap[y, x] = GetWallChar(x,y);
                        }
                        else
                        {
                            newMap[y, x] = emptyCell;
                        }
                        
                        mapBuilder.Append(newMap[y, x]);
                    }

                    mapBuilder.AppendLine();
                }

                File.WriteAllText(outputFile, mapBuilder.ToString());
            }
        }

        private bool LoadMap(string path)
        {
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);

                InitializeMap(lines);
                return true;
            }

            return false;
        }

        private Size GetMapSize(string[] lines)
        {
            int width = 0;
            int height = 0;

            for (int i = 0; i< lines.Length;i++)
            {
                var line = lines[i];

                if (line.Length > width)
                {
                    width = line.Length;
                }

                if (!string.IsNullOrEmpty(line))
                {
                    height = i + 1;
                }
            }

            return new Size(width, height);
        }

        private void InitializeMap(string [] lines)
        {
            _mapSize = GetMapSize(lines);

            _map = new bool[_mapSize.Height, _mapSize.Width];

            var ignoredChars = new char[] { _space, _food, _gate };

            for (int y = 0; y < _mapSize.Height; y++)
            {
                var line = lines[y];

                for (int x = 0; x < _mapSize.Width && x < line.Length; x++)
                {
                    var ch = line[x];

                    if (ignoredChars.Contains(ch))
                    {
                        continue;
                    }

                    _map[y, x] = true;
                }
            }
        }

        private bool HasTop(int x, int y)
        {
            if (y <=0)
            {
                return false;
            }

            return _map[y - 1, x];
        }

        private bool HasDown(int x, int y)
        {
            if (y >= _mapSize.Height -1)
            {
                return false;
            }

            return _map[y + 1, x];
        }

        private bool HasLeft(int x, int y)
        {
            if (x <= 0)
            {
                return false;
            }

            return _map[y, x-1];
        }

        private bool HasRight(int x, int y)
        {
            if (x >= _mapSize.Width -1)
            {
                return false;
            }

            return _map[y, x + 1];
        }

        private char GetWallChar(int x, int y)
        {  
            //top rigth down left

            int key = (ConvertToInt(HasTop(x, y)) << 3) + (ConvertToInt(HasRight(x, y)) << 2) + (ConvertToInt(HasDown(x, y)) << 1) + ConvertToInt(HasLeft(x, y));

            var chars = new Dictionary<int, char>()
            {
                [0b1011] = '╣',
                [0b1010] = '║',
                [0b0011] = '╗',
                [0b1001] = '╝',
                [0b1100] = '╚',
                [0b0110] = '╔',
                [0b1101] = '╩',
                [0b0111] = '╦',
                [0b1110] = '╠',
                [0b0101] = '═',
                [0b1111] = '╬',
                [0b0000] = '■'
            };

            return chars.ContainsKey(key) ?
                chars[key] :
                '■';
        }

        private int ConvertToInt(bool value)
        {
            return value ? 1 : 0;
        }
    }
}
