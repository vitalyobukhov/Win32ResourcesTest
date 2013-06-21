using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ResourcesTest
{
    static class PInvokeWrapper
    {
        private const int loadTextDefaultBufferSize = 65536;


        [StructLayout(LayoutKind.Sequential)]
        private struct GetResourceNamesParameter
        {
            public const int MaxResultCount = StringTable.StringsPerGroup * (ushort.MaxValue + 1);


            public int ResultLength;
            public uint[] Result;


            public GetResourceNamesParameter(int @resultCount = MaxResultCount)
            {
                ResultLength = 0;
                Result = new uint[@resultCount];
            }
        }


        private static bool GetResourceNamesCallback(IntPtr module, PInvoke.ResourceType type, uint id, IntPtr parameterPtr)
        {
            var parameter = (GetResourceNamesParameter)Marshal.PtrToStructure(parameterPtr, typeof(GetResourceNamesParameter));
            parameter.Result[parameter.ResultLength++] = id;
            Marshal.StructureToPtr(parameter, parameterPtr, true);
            return true;
        }

        private static ushort GetLanguage(PInvoke.PrimaryLanguage primary, PInvoke.SecondaryLanguage secondary)
        {
            return (ushort)((((ushort)((ushort)secondary >> 10)) << 10) | ((ushort)(((ushort)primary) & 0x3ff)));
        }

        private static Exception GetLastException()
        {
            return new Win32Exception(Marshal.GetLastWin32Error());
        }


        public static uint[] GetResourceIds(IntPtr module, PInvoke.ResourceType resourceType,
            PInvoke.PrimaryLanguage primaryResourceLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL,
            PInvoke.SecondaryLanguage secondaryResourceLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL,
            PInvoke.EnumResourceNamesFlags flags = PInvoke.EnumResourceNamesFlags.RESOURCE_ENUM_LN)
        {
            var parameter = new GetResourceNamesParameter(GetResourceNamesParameter.MaxResultCount);
            IntPtr parameterPtr = IntPtr.Zero;

            try
            {
                parameterPtr = Marshal.AllocHGlobal(Marshal.SizeOf(parameter));
                Marshal.StructureToPtr(parameter, parameterPtr, false);

                var language = GetLanguage(primaryResourceLanguage, secondaryResourceLanguage);
                if (!PInvoke.EnumResourceNamesEx(module, resourceType, GetResourceNamesCallback,
                    parameterPtr, flags, language))
                {
                    if (Marshal.GetLastWin32Error() != (int)PInvoke.SystemErrorCode.ERROR_RESOURCE_TYPE_NOT_FOUND)
                        throw GetLastException();
                }

                parameter = (GetResourceNamesParameter)Marshal.
                    PtrToStructure(parameterPtr, typeof(GetResourceNamesParameter));
                return parameter.Result.Take(parameter.ResultLength).ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to get resource names", ex);
            }
            finally
            {
                try
                {
                    if (parameterPtr != IntPtr.Zero)
                        Marshal.DestroyStructure(parameterPtr, typeof(GetResourceNamesParameter));
                }
                catch { }
            }
        }

        public static uint[] GetResourceIds(string filename, PInvoke.ResourceType type,
            PInvoke.PrimaryLanguage primaryLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL,
            PInvoke.SecondaryLanguage secondaryLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL,
            PInvoke.EnumResourceNamesFlags flags = PInvoke.EnumResourceNamesFlags.RESOURCE_ENUM_LN)
        {
            var module = PInvoke.LoadLibraryEx(filename, IntPtr.Zero, PInvoke.LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
            if (module == IntPtr.Zero)
            {
                var message = string.Format("Unable to load module from {0}", filename);
                throw new InvalidOperationException(message, GetLastException());
            }

            try
            {
                return GetResourceIds(module, type,
                    primaryLanguage, secondaryLanguage, flags);
            }
            finally
            {
                PInvoke.FreeLibrary(module);
            }
        }

        public static uint[] GetResourceIds(PInvoke.ResourceType type,
            PInvoke.PrimaryLanguage primaryLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL,
            PInvoke.SecondaryLanguage secondaryLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL,
            PInvoke.EnumResourceNamesFlags flags = PInvoke.EnumResourceNamesFlags.RESOURCE_ENUM_LN)
        {
            return GetResourceIds(IntPtr.Zero, type, primaryLanguage, secondaryLanguage, flags);
        }


        public static IntPtr BeginUpdateResources(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            var handle = PInvoke.BeginUpdateResource(filename, false);
            if (handle == IntPtr.Zero)
            {
                var message = string.Format("Unable to start update resources to {0}", filename);
                throw new IOException(message, GetLastException());
            }

            return handle;
        }

        public static void UpdateResource(IntPtr updateHandle,
            PInvoke.ResourceType type, uint id, byte[] data,
            PInvoke.PrimaryLanguage primaryLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL,
            PInvoke.SecondaryLanguage secondaryLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL,
            bool discardOnException = false)
        {
            if (updateHandle == IntPtr.Zero)
                throw new ArgumentNullException("updateHandle");

            var language = GetLanguage(primaryLanguage, secondaryLanguage);
            var size = data == null ? 0 : (uint)data.Length;
            if (!PInvoke.UpdateResource(updateHandle, type, id, language, data, size))
            {
                if (discardOnException)
                    PInvoke.EndUpdateResource(updateHandle, true);

                var message = string.Format("Unable to update resource {0}", id);
                throw new IOException(message, GetLastException());
            }
        }

        public static void RemoveResource(IntPtr updateHandle,
            PInvoke.ResourceType type, uint id,
            PInvoke.PrimaryLanguage primaryLanguage = PInvoke.PrimaryLanguage.LANG_NEUTRAL,
            PInvoke.SecondaryLanguage secondaryLanguage = PInvoke.SecondaryLanguage.SUBLANG_NEUTRAL,
            bool discardOnException = false)
        {
            UpdateResource(updateHandle, type, id, null, primaryLanguage, secondaryLanguage, discardOnException);
        }

        public static void EndUpdateResources(IntPtr updateHandle)
        {
            if (updateHandle == IntPtr.Zero)
                throw new ArgumentNullException("updateHandle");

            if (!PInvoke.EndUpdateResource(updateHandle, false))
            {
                throw new IOException("Unable to finish update resources", GetLastException());
            }
        }


        public static string LoadText(IntPtr module, uint id, int bufferSize = loadTextDefaultBufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");

            var buffer = new StringBuilder(bufferSize);
            var length = PInvoke.LoadString(module, id, buffer, buffer.Capacity);
            var text = length == 0 ? null : buffer.ToString();

            return text;
        }

        public static string LoadText(uint id, int bufferSize = loadTextDefaultBufferSize)
        {
            return LoadText(IntPtr.Zero, id, bufferSize);
        }
    }
}
