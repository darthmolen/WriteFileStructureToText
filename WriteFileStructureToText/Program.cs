using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WriteFileStructureToText
{
    class Program
    {
        static Parms parms = new Parms();
        static void Main(string[] args)
        {
            
            if (args.Length > 0)
                ProcessArgs(args, ref parms);

            List<PathMember> fullPlusRel = new List<PathMember>();

            //var currDir = Directory.GetCurrentDirectory();
            Console.WriteLine("Current Directory: {0}", parms.CurrentDirectory);
            var sub = Path.Combine(parms.CurrentDirectory, parms.SubDirectoryName);
            Console.WriteLine("SubDirectory: {0}", sub);
            foreach(var file in Directory.EnumerateFiles(sub, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("Full Path: {0}", file);
                var relative = IOHelper.RelativePath(file, parms.CurrentDirectory);
                Console.WriteLine("Relative Path: {0}", relative);
                fullPlusRel.Add(new PathMember() { Full = file, Relative  = relative });
            }

            WriteClearTextRelative(fullPlusRel, parms);
            WriteClearTextFull(fullPlusRel, parms);
            WriteJson(fullPlusRel, parms);
            Console.ReadLine();
        }

        private static void WriteJson(List<PathMember> fullPlusRel, Parms parms)
        {
            var fileName = Path.Combine(parms.CurrentDirectory, parms.FileNameForCombined);
            var data = PathMember.SerializeJSon<List<PathMember>>(fullPlusRel);
            File.WriteAllText(fileName, data);
        }

        private static void WriteClearTextFull(List<PathMember> fullPlusRel, Parms parms)
        {
            var fileName = Path.Combine(parms.CurrentDirectory, parms.FileNameForFull);
            File.WriteAllLines(fileName, fullPlusRel.Select(s => s.Full).ToArray());
        }

        private static void WriteClearTextRelative(List<PathMember> fullPlusRel, Parms parms)
        {
            var fileName = Path.Combine(parms.CurrentDirectory, parms.FileNameForRelative);
            File.WriteAllLines(fileName, fullPlusRel.Select(s => s.Relative).ToArray());
        }

        private static Parms ProcessArgs(string[] args, ref Parms parms)
        {

            for(int i = 0; i < args.Length; i++)
            {
                try
                {
                    var split = args[i].Split('=');
                    if (split == null || split.Length <= 1)
                        split = args[i].Split(':');

                    if (split == null || split.Length <= 1)
                        continue;

                    var k = split[0];
                    var s = split[1];

                    switch (k.ToLower().TrimStart('/'))
                    {
                        case "subdir":
                            parms.SubDirectoryName = s;
                            break;
                        case "relativefile":
                        case "rf":
                            parms.FileNameForRelative = s;
                            break;
                        case "fullfile":
                        case "ff":
                            parms.FileNameForFull = s;
                            break;
                        case "combinedfile":
                        case "cf":
                            parms.FileNameForCombined = s;
                            break;
                    }
                }
                catch
                { return parms; }
            }

            return parms;
        }
    }

    public class Parms
    {
        public string CurrentDirectory { get; }
        public string SubDirectoryName { get; set; }
        public string FileNameForRelative { get; set; }
        public string FileNameForFull { get; set; }
        public string FileNameForCombined { get; set; }

        public Parms()
        {
            SubDirectoryName = "";
            FileNameForRelative = "relative.txt";
            FileNameForFull = "full.txt";
            FileNameForCombined = "combined.json";
            CurrentDirectory = Directory.GetCurrentDirectory();
        }
    }

    [DataContract]
    public class PathMember
    {
        [DataMember]
        public string Full { get; set; }
        [DataMember]
        public string Relative { get; set; }

        public static string SerializeJSon<T>(T t)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings();
            ds.WriteObject(stream, t);
            string jsonString = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return jsonString;
        }
    }

    public static class IOHelper
    {
        public static string RelativePath(string absPath, string relTo)
        {
            Uri full = new Uri(absPath, UriKind.Absolute);
            Uri root = new Uri(relTo, UriKind.Absolute);

            return root.MakeRelative(full);
        }

    }
}
