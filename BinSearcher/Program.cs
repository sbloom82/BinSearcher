using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinSearcher
{
    class Program
    {
        static HashSet<string> extensions = new HashSet<string>();

        static void Main(string[] args)
        {
            string directory = args[0];
            string hex = args[1];
            for (int i = 2; i < args.Length; ++i)
            {
                extensions.Add(args[i]);
            }

            byte[] pattern = Helper.StringToByteArray(hex);

            List<FileInfo> filesToSearch = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            GetFiles(dir, ref filesToSearch);

            HashSet<FileInfo> matches = new HashSet<FileInfo>();

            Console.WriteLine($"Searching {filesToSearch.Count} Files");

            var result = Parallel.ForEach(filesToSearch,
                (file) =>
                {
                    byte[] source = File.ReadAllBytes(file.FullName);
                    if (Helper.HasMatch(source, pattern))
                    {
                        lock (matches)
                        {
                            matches.Add(file);
                        }
                    }
                });

            Console.WriteLine($"{matches.Count} Matches.");
            foreach (FileInfo match in matches)
            {
                Console.WriteLine($"{match.FullName}");
            }
            Console.ReadKey();
        }

        static private void GetFiles(DirectoryInfo directory, ref List<FileInfo> filesToSearch)
        {
            foreach (DirectoryInfo sub in directory.GetDirectories())
            {
                GetFiles(sub, ref filesToSearch);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                foreach (string ext in extensions)
                {
                    if (file.FullName.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        filesToSearch.Add(file);
                        break;
                    }
                }
            }
        }
    }
}
