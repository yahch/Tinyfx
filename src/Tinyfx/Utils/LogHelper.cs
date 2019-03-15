﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tinyfx.Cores;

namespace Tinyfx.Utils
{
    public class LogHelper
    {

        public enum LogType { ERROR, WARN, DEBUG, INFO };

        //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();

        public static void WriteLog(LogType logType, string message, Exception ex)
        {

            if (string.IsNullOrEmpty(message)) return;

            try
            {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                string logDir = System.IO.Path.Combine(TinyfxCore.Root, "Logs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);


                var logFilePath = System.IO.Path.Combine(logDir, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
                var now = DateTime.Now;
                var logContent = string.Format("{0} | {1} | {2} | {3}\r\n", now.ToString(), logType.ToString(), message, ex == null ? "-" : ex.ToString());

                File.AppendAllText(logFilePath, logContent);
            }
            catch (Exception)
            {

            }
            finally
            {
                //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}
