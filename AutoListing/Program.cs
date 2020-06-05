using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AutoListing
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();
        const int SW_HIDE = 0;

        [STAThread]
        static void Main(string[] args)
        {
            IntPtr hwnd;
            hwnd = GetConsoleWindow();
            ShowWindow(hwnd, SW_HIDE);

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            List<string> folder_path = new List<string>();
            while (true) 
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    if (folder_path.Count != 0)
                        dialog.SelectedPath = folder_path.Last();
                    if (dialog.ShowDialog() == DialogResult.OK)
                        folder_path.Add(dialog.SelectedPath);
                    else
                        break;
                }
            }
            if (folder_path.Count == 0)
                return;

            string result_file = string.Empty;

            using (var save_dialog = new SaveFileDialog())
            {
                save_dialog.InitialDirectory = programFiles;
                save_dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                save_dialog.FilterIndex = 1;
                save_dialog.RestoreDirectory = true;

                if (save_dialog.ShowDialog() == DialogResult.OK)
                    result_file = save_dialog.FileName;
                else
                    return;
            }

            foreach (var el in folder_path)
            {
                DirectoryInfo info = new DirectoryInfo(el);
                FileInfo[] files = info.GetFiles("*.cs");

                foreach (FileInfo file in files)
                {
                    using (StreamWriter writer = new StreamWriter(result_file, true))
                    {
                        writer.WriteLine($"[{file.Name}]{Environment.NewLine}");
                        using (StreamReader reader = new StreamReader(file.FullName))
                            writer.WriteLine(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
