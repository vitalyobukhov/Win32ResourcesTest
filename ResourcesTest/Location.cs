using System.IO;
using System.Reflection;

namespace ResourcesTest
{
    static class Location
    {
        private const string tempFilenamePrefix = "Temp";


        private static readonly object getCurrentLocationLock = new object();
        private static readonly object getOriginalLocationLock = new object();
        private static readonly object getTempLocationLock = new object();
        private static readonly object isCurrentTempLocationLock = new object();


        private static string currentLocation;
        public static string GetCurrentLocation()
        {
            lock (getCurrentLocationLock)
            {
                return currentLocation ?? (currentLocation = Assembly.GetExecutingAssembly().Location);
            }
        }

        private static string originalLocation;
        public static string GetOriginalLocation()
        {
            lock (getOriginalLocationLock)
            {
                if (originalLocation == null)
                {
                    var location = Assembly.GetExecutingAssembly().Location;
                    var directory = Path.GetDirectoryName(location);
                    var filename = Path.GetFileName(location);
                    var originalFilename = filename.StartsWith(tempFilenamePrefix) ? 
                        filename.Substring(tempFilenamePrefix.Length,
                            filename.Length - tempFilenamePrefix.Length) : 
                        filename;
                    originalLocation = Path.Combine(directory, originalFilename);
                }
            }

            return originalLocation;
        }

        private static string tempLocation;
        public static string GetTempLocation()
        {
            lock (getTempLocationLock)
            {
                if (tempLocation == null)
                {
                    var location = Assembly.GetExecutingAssembly().Location;
                    var directory = Path.GetDirectoryName(location);
                    var filename = Path.GetFileName(location);
                    var tempFilename = filename.StartsWith(tempFilenamePrefix) ?
                        filename :
                        tempFilenamePrefix + filename;
                    tempLocation = Path.Combine(directory, tempFilename);
                }
            }

            return tempLocation;
        }

        private static bool? isCurrentTempLocation;
        public static bool IsCurrentTempLocation()
        {
            lock (isCurrentTempLocationLock)
            {
                if (!isCurrentTempLocation.HasValue)
                {
                    var location = Assembly.GetExecutingAssembly().Location;
                    var filename = Path.GetFileName(location);
                    isCurrentTempLocation = filename.StartsWith(tempFilenamePrefix);
                }
            }

            return isCurrentTempLocation.Value;
        }
    }
}
