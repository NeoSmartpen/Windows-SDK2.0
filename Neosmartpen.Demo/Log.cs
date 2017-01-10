using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PenDemo
{
    class Log
    {
        public static void FileLog(){

            String path = Application.StartupPath;
            Stream debugFile = File.Create(path + "\\PEN_debug.log.txt");
            TextWriterTraceListener debugWriter = new TextWriterTraceListener(debugFile, "file");

            Debug.Listeners.Add(debugWriter);
            Debug.Listeners["file"].TraceOutputOptions |= TraceOptions.Callstack;
            Debug.AutoFlush = true;

            Stream logFile = File.Create(path + "\\PEN_consol.log.txt");
            StreamWriter logWriter = new StreamWriter(logFile);
            logWriter.AutoFlush = true;
            Console.SetOut(logWriter);

        }
    }
}
