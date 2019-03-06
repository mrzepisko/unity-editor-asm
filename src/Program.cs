using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEditorAssemblyCreator {
    class Program {
        private const string REPLACE_NAME = "%asmname";
        private const string REPLACE_REFERENCES = "%references";
        private const string EDITOR_DIR = "EDITOR";
        private const string FILE_SUFFIX = ".asmdef";
        private const string REFERENCE_ALLOWED_CHARACTERS_REGEX = @"[^\w\d\._]+";
        static void Main(string[] args) {
            if (args == null || args.Length == 0) {
                Console.WriteLine(Resources.USAGE);
            }

            var rootPath = args[0];
            if (!Directory.Exists(rootPath)) {
                Console.WriteLine(Resources.INVALID_PATH);
            }

            var references = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++) {
                references[i - 1] = args[i];
            }

            string[] editorPaths = Directory.GetDirectories(rootPath, EDITOR_DIR, SearchOption.AllDirectories)
                .Where(d => !d.Contains("/.") && !d.Contains(@"\.")).ToArray();
            string refContent = CreateReferences(ref references);
            string content = Resources.AssemblyContents.Replace(REPLACE_REFERENCES, refContent);

            Console.WriteLine(Resources.DIRECTORIES_FOUND, editorPaths.Length);
            foreach (string dir in editorPaths) {
                string name = GetAssemblyName(dir, rootPath);
                var filename = $"{dir}{Path.DirectorySeparatorChar}{name}{FILE_SUFFIX}";
                var contents = content.Replace(REPLACE_NAME, name);
                File.WriteAllText(filename, contents);
                Console.WriteLine(name);
            }
            Console.WriteLine(Resources.DONE);
        }
        
        static string CreateReferences(ref string[] references) {
            StringBuilder sb = new StringBuilder();
            foreach (string reference in references) {
                sb.Append("\"")
                    .Append(reference)
                    .Append("\", ");
            }

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        
        /// <summary>
        /// so703281
        /// </summary>
        /// <returns>relative path</returns>
        static string GetRelativePath(string target, string root)
        {
            Uri pathUri = new Uri(target);
            // Folders must end in a slash
            if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                root += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(root);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        static string GetAssemblyName(string dir, string root) {
            var relative = GetRelativePath(dir, root).Replace(" ", string.Empty);
            return Regex.Replace(relative, REFERENCE_ALLOWED_CHARACTERS_REGEX, ".");
        }
    }
}