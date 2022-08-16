using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ET;

namespace YuoTools.Extend.Helper
{
    public static class ProcessHelper
    {
        public static Process Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false)
        {
            //Log.Debug($"Process Run exe:{exe} ,arguments:{arguments} ,workingDirectory:{workingDirectory}");
            try
            {
                var redirectStandardOutput = true;
                var redirectStandardError = true;
                var useShellExecute = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }

                if (waitExit)
                {
                    redirectStandardOutput = true;
                    redirectStandardError = true;
                    useShellExecute = false;
                }

                //Log.Debug($"1111111111111111111111111aaaa: {redirectStandardError} {redirectStandardOutput} {useShellExecute}");

                var info = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError
                };

                var process = Process.Start(info);

                if (waitExit) WaitExitAsync(process).Coroutine();

                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }

        private static async ETTask WaitExitAsync(Process process)
        {
            await process.WaitForExitAsync();
#if UNITY
            Log.Info($"process exit, exitcode: {process.ExitCode} {process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
#endif
        }

        private static async Task WaitForExitAsync(this Process self)
        {
            if (!self.HasExited) return;

            try
            {
                self.EnableRaisingEvents = true;
            }
            catch (InvalidOperationException)
            {
                if (self.HasExited) return;
                throw;
            }

            var tcs = new TaskCompletionSource<bool>();

            void Handler(object s, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            self.Exited += Handler;

            try
            {
                if (self.HasExited) return;
                await tcs.Task;
            }
            finally
            {
                self.Exited -= Handler;
            }
        }
    }
}