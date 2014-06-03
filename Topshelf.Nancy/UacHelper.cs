using System.Diagnostics;

namespace Topshelf.Nancy
{
    public static class UacHelper
    {
        public static bool RunElevated(string file, string args, out string output)
        {
            var process = CreateProcess(args, file);

            process.Start();
            process.WaitForExit();

            output = process.StandardOutput.ReadToEnd();

            return process.ExitCode == 0;
        }

        private static Process CreateProcess(string args, string file)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    Arguments = args,
                    FileName = file,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
        }
    }
}
