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
        string ViewPath = @"d:\photo\";
        string TreePath = @"D:\tmp\folderTree.txt";
        string LogPath = @"D:\tmp\log.txt";
        ConsoleKeyInfo _key;

        public void DoSomeThing()
        {
            if (File.Exists(TreePath))
                File.Delete(TreePath);

            FileReaderLogic root = new FileReaderLogic(ViewPath, TreePath, LogPath);

            Thread[] ThreadArray = new Thread[3];
            //     Parallel.Invoke(()=> { root.Start(); }, () => { root.Start(); }, () => { root.Start(); });
            for (var i = 0; i < ThreadArray.Length; i++)
            {
                ThreadArray[i] = new Thread(root.Start);
                ThreadArray[i].Start();
                ThreadArray[i].Name = (i + 1).ToString();
            }

            while (ThreadArray.Any(x => x.IsAlive))
            {
                _key = Console.ReadKey();

                if (_key.Key == ConsoleKey.Spacebar)
                {
                    if (!ThreadArray.Any(x => x.IsAlive))
                        break;

                    if (ThreadArray[0].ThreadState == ThreadState.Running || ThreadArray[1].ThreadState == ThreadState.Running || ThreadArray[2].ThreadState == ThreadState.Running )
                    {
                        foreach (Thread thr in ThreadArray)
                        {
                            if (thr.IsAlive)
                            {
                                thr.Suspend();
                                Console.WriteLine("Thread {0} suspended", thr.Name);
                            }
                            Console.WriteLine(thr.Name + " "+ thr.ThreadState + " " + thr.IsAlive);
                        }
                    }
                    else
                    {
                        foreach (Thread thr in ThreadArray)
                        {
                            if (thr.IsAlive)
                            {
                                thr.Resume();
                            }
                        }
                    }
                }
            }
            Console.WriteLine("--------------------------");
        }
    }
}
