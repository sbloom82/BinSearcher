#define GUIMode

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BinSearcher
{
    class Program
    {
        static HashSet<string> extensions = new HashSet<string>();

        [STAThread]
        static void Main(string[] args)
        {
#if GUIMode
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            string directory = fbd.SelectedPath;
            string hex = "";

            if (InputBox(Application.ProductName, "Enter Search Pattern:", ref hex) != DialogResult.OK || string.IsNullOrWhiteSpace(hex))
                return;

            string extsCSV = "";
            if (InputBox(Application.ProductName, "Enter Extensions To Search Separated By Commas(,):", ref extsCSV, "bin") != DialogResult.OK || string.IsNullOrWhiteSpace(extsCSV))
                return;

            string[] sArr = extsCSV.Split(',');
            foreach(string s in sArr)
            {
                if (!string.IsNullOrWhiteSpace(s) && !s.Contains(","))
                    extensions.Add(s);
            }
#else
            string directory = args[0];
            string hex = args[1];
            for (int i = 2; i < args.Length; ++i)
            {
                extensions.Add(args[i]);
            }
#endif

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

        public static DialogResult InputBox(string title, string promptText, ref string value, string defaultValue = "")
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.TopMost = true;

            form.Shown += (o, i) =>
            {
                form.Activate();
            };

            textBox.KeyDown += (o, i) =>
            {
                if (i.KeyCode == Keys.A && i.Control)
                {
                    textBox.SelectAll();
                }
            };

            textBox.Text = defaultValue;

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
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
