using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MiniJob.Dapr
{
    public static class MiniJobActorExtensions
    {
        /// <summary>
        /// 运行 Dapr sidecar
        /// </summary>
        /// <param name="options"></param>
        /// <param name="address"></param>
        /// <exception cref="Exception"></exception>
        public static void RunDaprSidecar(this MiniJobDaprOptions options, string address)
        {
            if (string.IsNullOrEmpty(address))
                return;

            if (!options.RunSidecar)
                return;

            string argumentPrefix;
            string fileName;
            var uri = new Uri(address);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                argumentPrefix = "-c";
                fileName = "/bin/bash";
            }
            else
            {
                argumentPrefix = "/C";
                fileName = "cmd.exe";
            }

            var arguments = $"{argumentPrefix} {options.GenerateScript()} -app-port={uri.Port}";

            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                arguments += " -app-ssl=true";
            }

            var procStartInfo = new ProcessStartInfo(fileName, arguments)
            {
                WorkingDirectory = GetDaprPath()
            };

            try
            {
                Process.Start(procStartInfo);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't run dapr...");
            }
        }

        /// <summary>
        /// 获取 dapr 文件夹路径(存放配置文件)
        /// <para>调试模式工作目录为解决方案路径，发布后为应用当前路径</para>
        /// </summary>
        /// <returns></returns>
        private static string GetDaprPath()
        {
            var slnDirectoryPath = GetSolutionDirectoryPath();

            if (slnDirectoryPath != null)
            {
                return slnDirectoryPath;
            }

            return Directory.GetCurrentDirectory();
        }

        private static string GetSolutionDirectoryPath()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (Directory.GetParent(currentDirectory.FullName) != null)
            {
                currentDirectory = Directory.GetParent(currentDirectory.FullName);

                if (Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
                {
                    return currentDirectory.FullName;
                }
            }

            return null;
        }
    }
}
