﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemFileReader
{
    class FileReaderLogic
    {
        public volatile List<string> FolderList;
        public volatile List<string> FileList;
        //  public List<string> Log;
        private static object Locker = new object();
        private DirectoryInfo RootDir;
        private FileInfo[] Files;
        private DirectoryInfo[] SubDirs;
        private string TreePath;
        private string LogPath;
        public EventWaitHandle WaitHandler = new ManualResetEvent(true);

        public FileReaderLogic(string path, string treePath, string logPath)
        {
            RootDir = Directory.GetParent(path);
            FolderList = new List<string>();
            FileList = new List<string>();
            //  Log = new List<string>();
            TreePath = treePath;
            LogPath = logPath;
        }

        public void Start()
        {
            WalkDirectoryTree(RootDir);
        }

        private void WalkDirectoryTree(DirectoryInfo root)
        {

            try
            {
                Files = root.GetFiles("*.*");
                SubDirs = root.GetDirectories();
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

            if (Files != null)
            {

                foreach (FileInfo fi in Files)
                {
                    WaitHandler.WaitOne();
                    lock (Locker)
                    {
                        if (FileList.Any(x => x.Contains(fi.FullName)))
                        {
                            continue;
                        }
                        FileList.Add(fi.FullName + " Поток " + Thread.CurrentThread.Name);
                        Console.WriteLine(fi.FullName + " Поток " + Thread.CurrentThread.Name);
                    }
                }

                foreach (DirectoryInfo dirInfo in SubDirs)
                {
                    lock (Locker)
                    {
                        if (!FolderList.Any(x => x.Contains(dirInfo.FullName)))
                        {
                            FolderList.Add(dirInfo.FullName + " Поток " + Thread.CurrentThread.Name);
                            Console.WriteLine(dirInfo.FullName + " Поток " + Thread.CurrentThread.Name);
                        }
                    }
                    WaitHandler.WaitOne();
                    WalkDirectoryTree(dirInfo);
                }
            }
        }
    }
}
