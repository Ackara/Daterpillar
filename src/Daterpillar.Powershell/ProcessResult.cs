using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar
{
    public struct ProcessResult
    {
        internal ProcessResult(int exitcode, string output, string error, long excutionTime)
        {
            ExitCode = exitcode;
            ErrorOutput = error;
            StandardOutput = output;
            ExecutionTime = excutionTime;
        }

        public int ExitCode { get; }

        public long ExecutionTime { get; }

        public string ErrorOutput { get; }

        public string StandardOutput { get; }

        public IEnumerable<string> GetOutput()
        {
            foreach (string line in GetLines(ErrorOutput))
                yield return line;

            foreach (string line in GetLines(StandardOutput))
                yield return line;
        }

        internal static IEnumerable<string> GetLines(string text)
        {
            if (!string.IsNullOrEmpty(text))
                foreach (string line in text.Split(new string[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.None))
                    yield return line;
        }
    }
}