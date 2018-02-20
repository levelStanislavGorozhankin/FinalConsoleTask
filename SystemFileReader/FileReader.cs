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
    public class FileReader
    {
        private string ViewPath = @"d:\tmp1\";
        private string TreePath = @"D:\tmp\folderTree.txt";
        private string LogPath = @"D:\tmp\log.txt";
        private ConsoleKeyInfo _key;

        public void Start()
        {
            if (!Directory.GetParent(TreePath).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(TreePath).ToString());
            }

            if (File.Exists(TreePath))
            {
                File.Delete(TreePath);
            }

            FileReaderLogic root = new FileReaderLogic(ViewPath, TreePath, LogPath);

            Thread[] ThreadArray = new Thread[3];
            //     Parallel.Invoke(()=> { root.Start(); }, () => { root.Start(); }, () => { root.Start(); });
            for (var i = 0; i < ThreadArray.Length; i++)
            {
                ThreadArray[i] = new Thread(root.Start);
                ThreadArray[i].Start();
                ThreadArray[i].Name = (i + 1).ToString();
            }

            bool stop = false;
            while (true)
            {
                if (stop)
                {
                    break;
                } 
                _key = Console.ReadKey(true);

                if (!ThreadArray[0].IsAlive && !ThreadArray[0].IsAlive && !ThreadArray[0].IsAlive) //if(!ThreadArray.All(x => x.IsAlive))
                {
                    Console.WriteLine("All thread are stopped");
                }   

                switch (_key.Key)
                {

                    case ConsoleKey.Spacebar:
                        {
                            if (ThreadArray.Any(x => x.ThreadState == ThreadState.Running))
                            {
                                root.WaitHandler.Reset();
                            }
                            else
                            {
                                root.WaitHandler.Set();
                            }
                            break;
                        }

                    case ConsoleKey.F1:
                        {
                            foreach (Thread thr in ThreadArray)
                            {
                                Console.WriteLine(thr.Name + " " + thr.ThreadState + " " + thr.IsAlive);
                            }
                            break;
                        }

                    case ConsoleKey.Escape:
                        {
                            foreach (Thread thr in ThreadArray)
                            {
                                thr.Abort();
                            }
                            stop = true;
                            break;
                        }
                }
            }

            Console.WriteLine("--------------------------");
            File.WriteAllLines(TreePath, root.FolderList);
            File.AppendAllLines(TreePath, root.FileList);

            Console.WriteLine("Done");
        }
    }
}
