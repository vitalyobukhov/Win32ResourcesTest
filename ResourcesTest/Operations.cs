using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ResourcesTest
{
    static class Operations
    {
        private const PInvoke.ResourceType operationsResourceType = PInvoke.ResourceType.STRING;
        private const PInvoke.PrimaryLanguage operationsPrimaryLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL;
        private const PInvoke.SecondaryLanguage operationsSecondaryLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL;


        private static void CheckCurrentLocationIsTemp()
        {
            if (!Location.IsCurrentTempLocation())
                throw new InvalidOperationException("Executable is not temporary one");
        }

        private static void CheckCurrentLocationIsNotTemp()
        {
            if (Location.IsCurrentTempLocation())
                throw new InvalidOperationException("Executable is temporary one");
        }

        private static void CopyToTempLocation()
        {
            var originalLocation = Location.GetOriginalLocation();
            var tempLocation = Location.GetTempLocation();

            try
            {
                File.Copy(originalLocation, tempLocation, true);
            }
            catch (Exception ex)
            {
                var message = string.Format("Unable to copy file from {0} to {1}",
                    originalLocation, tempLocation);
                throw new IOException(message, ex);
            }
        }

        private static void PrintText(uint id, string text)
        {
            const string format = @"
STRING[ID={0:00000000}]
--------------------------------------------------------------------------------{1}
--------------------------------------------------------------------------------";
            const string empty = "[EMPTY]";
            Console.Write(format, id, text ?? empty);
        }

        private static Process GetProcessByLocation(string location)
        {
            return Process.GetProcesses().FirstOrDefault(p =>
            {
                try { return p.MainModule.FileName == location; }
                catch { return false; }
            });
        }

        private static void InvokeReplace()
        {
            var originalLocation = Location.GetOriginalLocation();
            var tempLocation = Location.GetTempLocation();

            try
            {
                var replaceArgs = new ReplaceArgs
                {
                    OriginalLocation = originalLocation,
                    TempLocation = tempLocation
                };
                Process.Start(tempLocation, replaceArgs.ToString());
            }
            catch (Exception ex)
            {
                var message = string.Format("Unable to start executable {0}", originalLocation);
                try { File.Delete(tempLocation); }
                catch { }
                throw new InvalidOperationException(message, ex);
            }
        }


        public static void Insert(InsertArgs args)
        {
            CheckCurrentLocationIsNotTemp();

            var tempLocation = Location.GetTempLocation();

            CopyToTempLocation();

            try
            {
                var updateHandle = PInvokeWrapper.BeginUpdateResources(tempLocation);

                var groups = StringTable.GetGroups(args.Strings);
                foreach (var data in groups)
                {
                    PInvokeWrapper.UpdateResource(updateHandle, operationsResourceType, 
                        data.Key, data.Value, operationsPrimaryLanguage, operationsSecondaryLanguage, true);
                }

                PInvokeWrapper.EndUpdateResources(updateHandle);
            }
            catch
            {
                try { File.Delete(tempLocation); }
                catch { }
                throw;
            }

            InvokeReplace();
        }

        public static void Extract(ExtractArgs args)
        {
            CheckCurrentLocationIsNotTemp();

            foreach (var id in args.Ids)
            {
                var text = PInvokeWrapper.LoadText(id);
                PrintText(id, text);
            }
        }

        public static void Clean(CleanArgs args)
        {
            CheckCurrentLocationIsNotTemp();

            var tempLocation = Location.GetTempLocation();

            CopyToTempLocation();

            try
            {
                var resourceIds = PInvokeWrapper.GetResourceIds(tempLocation,
                    operationsResourceType);

                var updateHandle = PInvokeWrapper.BeginUpdateResources(tempLocation);

                foreach (var id in resourceIds)
                {
                    PInvokeWrapper.RemoveResource(updateHandle, operationsResourceType, 
                        id, operationsPrimaryLanguage, operationsSecondaryLanguage, true);
                }

                PInvokeWrapper.EndUpdateResources(updateHandle);
            }
            catch
            {
                try { File.Delete(tempLocation); }
                catch { }
                throw;
            }

            InvokeReplace();
        }

        public static void Replace(ReplaceArgs args)
        {
            CheckCurrentLocationIsTemp();

            using (var parentProcess = GetProcessByLocation(args.OriginalLocation))
            {
                if (parentProcess != null)
                {
                    const int waitTimeout = 5000;
                    if (!parentProcess.WaitForExit(waitTimeout))
                        throw new TimeoutException("Parent process is still running");
                }
            }

            try
            {
                if (File.Exists(args.OriginalLocation))
                    File.Delete(args.OriginalLocation);
            }
            catch (Exception ex)
            {
                var message = string.Format("Unable to delete file {0}", 
                    args.OriginalLocation);
                throw new IOException(message, ex);
            }

            try
            {
                File.Move(args.TempLocation, args.OriginalLocation);
            }
            catch (Exception ex)
            {
                var message = string.Format("Unable to move file from {0} to {1}",
                    args.TempLocation, args.OriginalLocation);
                throw new IOException(message, ex);
            }
        }
    }
}
