using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemFileReader
{
    public class FileReaderLogic
    {
        private List<string> FolderList;
        public List<string> Log;
        private static object locker = new object();
        private DirectoryInfo rootDir;
        private FileInfo[] files;
        private DirectoryInfo[] subDirs;
        private string TreePath;
        private string LogPath;

        public FileReaderLogic(string path, string treePath, string logPath)
        {
            rootDir = Directory.GetParent(path);
            FolderList = new List<string>();
            Log = new List<string>();
            TreePath = treePath;
            LogPath = logPath;
        }

        public void Start()
        {
            WalkDirectoryTree(rootDir);
        }

        public void WalkDirectoryTree(DirectoryInfo root)
        {
            try
            {
                files = root.GetFiles("*.*");
                subDirs = root.GetDirectories();
            }
            catch (UnauthorizedAccessException e)
            {
                //Log.Add(e.Message);
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(e.Message);
                }
            }

            catch (DirectoryNotFoundException e)
            {
                //Log.Add(e.Message);
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(e.Message);
                }
            }

            if (files != null)
            {

                foreach (FileInfo fi in files)
                {
                    lock (locker)
                    {
                        if (FolderList.Any(x => x.Equals(fi.FullName)))
                            continue;

                        using (StreamWriter sw = File.AppendText(TreePath))
                        {
                            sw.WriteLine(fi.FullName + " Поток " + Thread.CurrentThread.Name); //Thread.CurrentThread.Name);
                        }

                        FolderList.Add(fi.FullName);
                        Console.WriteLine(fi.FullName + " Поток " + Thread.CurrentThread.Name);//Thread.CurrentThread.Name);
                    }
                }

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    lock (locker)
                    {
                        if (FolderList.Any(x => x.Equals(dirInfo.FullName)))
                            continue;

                        using (StreamWriter sw = File.AppendText(TreePath))
                        {
                            sw.WriteLine(dirInfo.FullName + " Поток " + Thread.CurrentThread.Name); //Thread.CurrentThread.Name);
                        }

                        FolderList.Add(dirInfo.FullName);
                        Console.WriteLine(dirInfo.FullName + " Поток " + Thread.CurrentThread.Name);//Thread.CurrentThread.Name);
                    }
                    WalkDirectoryTree(dirInfo);
                }
            }
        }
    }
}
