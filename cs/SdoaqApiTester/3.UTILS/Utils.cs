using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static SDOWSIO.WSIO;

namespace SDOAQ_App_CS
{
	public static class Utils
    {
        #region Log
        public static void WriteLog(string str)
        {
            WriteLog(str, null);
        }

        public static void WriteDataLog(string str)
        {
            WriteLog(str, @".\Data\");
        }

        public static void WriteLog(string str,
                                    string path = null)
        {
            string logPath = path;

            if (path != null && path != string.Empty && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (path == null || path == string.Empty || !Directory.Exists(path))
                logPath = @".\Log\";

            string FilePath = Path.Combine(logPath, "Log_" + DateTime.Now.ToString("yyyyMMddHH") + ".log");
            string DirPath = logPath;
            if (DirPath.First() == '.')
            {
                DirPath = Environment.CurrentDirectory + logPath.Substring(1, logPath.Length - 1);
            }
            string temp = str;

            if (!Directory.Exists(DirPath))
                Directory.CreateDirectory(DirPath);

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            Console.WriteLine(str);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                str = str.Replace("\n", "");
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        //temp = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        //temp = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
		
		public static void print_wsio_last_error()
		{
			byte[] bytesLog = new byte[4 * 1024];
			GCHandle bufHandle = GCHandle.Alloc(bytesLog, GCHandleType.Pinned);
			IntPtr log = bufHandle.AddrOfPinnedObject();

			WSIO_LastErrorString(log, bytesLog.Length);

			string str = Encoding.Default.GetString(bytesLog);
			Utils.WriteLog(string.Format("[WSIO ERROR] {0}", str));
			if (bufHandle.IsAllocated) { bufHandle.Free(); }
		}
		#endregion

		#region RGB
		public static uint RGB(byte r, byte g, byte b)
        {
            return ((uint)(((uint)(r) | ((uint)((g)) << 8)) | (((uint)(b)) << 16)));
        }

        #endregion
    }
}
