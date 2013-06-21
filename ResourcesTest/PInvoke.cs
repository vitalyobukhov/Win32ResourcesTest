using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ResourcesTest
{
    static class PInvoke
    {
        public delegate bool EnumResourceNamesProcedure(IntPtr module, ResourceType type, uint id, IntPtr parameter);

// ReSharper disable InconsistentNaming
        public enum SystemErrorCode : int
        {
            ERROR_RESOURCE_TYPE_NOT_FOUND = 1813
        }

        [Flags]
        public enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }

        [Flags]
        public enum EnumResourceNamesFlags : uint
        {
            RESOURCE_ENUM_LN = 0x1,
            RESOURCE_ENUM_MUI = 0x2,
            RESOURCE_ENUM_VALIDATE = 0x8
        }

        public enum ResourceType : uint
        {
            CURSOR = 1,
            BITMAP = 2,
            ICON = 3,
            MENU = 4,
            DIALOG = 5,
            STRING = 6,
            FONTDIR = 7,
            FONT = 8,
            ACCELERATOR = 9,
            RCDATA = 10,
            MESSAGETABLE = 11,
            GROUP_CURSOR = 12,
            GROUP_ICON = 14,
            VERSION = 16,
            DLGINCLUDE = 17,
            PLUGPLAY = 19,
            VXD = 20,
            ANICURSOR = 21,
            ANIICON = 22,
            HTML = 23,
            MANIFEST = 24
        }

        public enum PrimaryLanguage : ushort
        {
            LANG_NEUTRAL = 0x00,
            LANG_AFRIKAANS = 0x36,
            LANG_ALBANIAN = 0x1c,
            LANG_ARABIC = 0x01,
            LANG_ARMENIAN = 0x2b,
            LANG_ASSAMESE = 0x4d,
            LANG_AZERI = 0x2c,
            LANG_BASQUE = 0x2d,
            LANG_BELARUSIAN = 0x23,
            LANG_BENGALI = 0x45,
            LANG_BULGARIAN = 0x02,
            LANG_CATALAN = 0x03,
            LANG_CHINESE = 0x04,
            LANG_CROATIAN = 0x1a,
            LANG_CZECH = 0x05,
            LANG_DANISH = 0x06,
            LANG_DUTCH = 0x13,
            LANG_ENGLISH = 0x09,
            LANG_ESTONIAN = 0x25,
            LANG_FAEROESE = 0x38,
            LANG_FARSI = 0x29,
            LANG_FINNISH = 0x0b,
            LANG_FRENCH = 0x0c,
            LANG_GEORGIAN = 0x37,
            LANG_GERMAN = 0x07,
            LANG_GREEK = 0x08,
            LANG_GUJARATI = 0x47,
            LANG_HEBREW = 0x0d,
            LANG_HINDI = 0x39,
            LANG_HUNGARIAN = 0x0e,
            LANG_ICELANDIC = 0x0f,
            LANG_INDONESIAN = 0x21,
            LANG_ITALIAN = 0x10,
            LANG_JAPANESE = 0x11,
            LANG_KANNADA = 0x4b,
            LANG_KASHMIRI = 0x60,
            LANG_KAZAK = 0x3f,
            LANG_KONKANI = 0x57,
            LANG_KOREAN = 0x12,
            LANG_LATVIAN = 0x26,
            LANG_LITHUANIAN = 0x27,
            LANG_MACEDONIAN = 0x2f,
            LANG_MALAY = 0x3e,
            LANG_MALAYALAM = 0x4c,
            LANG_MANIPURI = 0x58,
            LANG_MARATHI = 0x4e,
            LANG_NEPALI = 0x61,
            LANG_NORWEGIAN = 0x14,
            LANG_ORIYA = 0x48,
            LANG_POLISH = 0x15,
            LANG_PORTUGUESE = 0x16,
            LANG_PUNJABI = 0x46,
            LANG_ROMANIAN = 0x18,
            LANG_RUSSIAN = 0x19,
            LANG_SANSKRIT = 0x4f,
            LANG_SERBIAN = 0x1a,
            LANG_SINDHI = 0x59,
            LANG_SLOVAK = 0x1b,
            LANG_SLOVENIAN = 0x24,
            LANG_SPANISH = 0x0a,
            LANG_SWAHILI = 0x41,
            LANG_SWEDISH = 0x1d,
            LANG_TAMIL = 0x49,
            LANG_TATAR = 0x44,
            LANG_TELUGU = 0x4a,
            LANG_THAI = 0x1e,
            LANG_TURKISH = 0x1f,
            LANG_UKRAINIAN = 0x22,
            LANG_URDU = 0x20,
            LANG_UZBEK = 0x43,
            LANG_VIETNAMESE = 0x2a
        }

        public enum SecondaryLanguage : ushort
        {
            SUBLANG_NEUTRAL = 0x00,
            SUBLANG_DEFAULT = 0x01,
            SUBLANG_SYS_DEFAULT = 0x02,
            SUBLANG_ARABIC_SAUDI_ARABIA = 0x01,
            SUBLANG_ARABIC_IRAQ = 0x02,
            SUBLANG_ARABIC_EGYPT = 0x03,
            SUBLANG_ARABIC_LIBYA = 0x04,
            SUBLANG_ARABIC_ALGERIA = 0x05,
            SUBLANG_ARABIC_MOROCCO = 0x06,
            SUBLANG_ARABIC_TUNISIA = 0x07,
            SUBLANG_ARABIC_OMAN = 0x08,
            SUBLANG_ARABIC_YEMEN = 0x09,
            SUBLANG_ARABIC_SYRIA = 0x0a,
            SUBLANG_ARABIC_JORDAN = 0x0b,
            SUBLANG_ARABIC_LEBANON = 0x0c,
            SUBLANG_ARABIC_KUWAIT = 0x0d,
            SUBLANG_ARABIC_UAE = 0x0e,
            SUBLANG_ARABIC_BAHRAIN = 0x0f,
            SUBLANG_ARABIC_QATAR = 0x10,
            SUBLANG_AZERI_LATIN = 0x01,
            SUBLANG_AZERI_CYRILLIC = 0x02,
            SUBLANG_CHINESE_TRADITIONAL = 0x01,
            SUBLANG_CHINESE_SIMPLIFIED = 0x02,
            SUBLANG_CHINESE_HONGKONG = 0x03,
            SUBLANG_CHINESE_SINGAPORE = 0x04,
            SUBLANG_CHINESE_MACAU = 0x05,
            SUBLANG_DUTCH = 0x01,
            SUBLANG_DUTCH_BELGIAN = 0x02,
            SUBLANG_ENGLISH_US = 0x01,
            SUBLANG_ENGLISH_UK = 0x02,
            SUBLANG_ENGLISH_AUS = 0x03,
            SUBLANG_ENGLISH_CAN = 0x04,
            SUBLANG_ENGLISH_NZ = 0x05,
            SUBLANG_ENGLISH_EIRE = 0x06,
            SUBLANG_ENGLISH_SOUTH_AFRICA = 0x07,
            SUBLANG_ENGLISH_JAMAICA = 0x08,
            SUBLANG_ENGLISH_CARIBBEAN = 0x09,
            SUBLANG_ENGLISH_BELIZE = 0x0a,
            SUBLANG_ENGLISH_TRINIDAD = 0x0b,
            SUBLANG_ENGLISH_ZIMBABWE = 0x0c,
            SUBLANG_ENGLISH_PHILIPPINES = 0x0d,
            SUBLANG_FRENCH = 0x01,
            SUBLANG_FRENCH_BELGIAN = 0x02,
            SUBLANG_FRENCH_CANADIAN = 0x03,
            SUBLANG_FRENCH_SWISS = 0x04,
            SUBLANG_FRENCH_LUXEMBOURG = 0x05,
            SUBLANG_FRENCH_MONACO = 0x06,
            SUBLANG_GERMAN = 0x01,
            SUBLANG_GERMAN_SWISS = 0x02,
            SUBLANG_GERMAN_AUSTRIAN = 0x03,
            SUBLANG_GERMAN_LUXEMBOURG = 0x04,
            SUBLANG_GERMAN_LIECHTENSTEIN = 0x05,
            SUBLANG_ITALIAN = 0x01,
            SUBLANG_ITALIAN_SWISS = 0x02,
            SUBLANG_KASHMIRI_INDIA = 0x02,
            SUBLANG_KOREAN = 0x01,
            SUBLANG_LITHUANIAN = 0x01,
            SUBLANG_LITHUANIAN_CLASSIC = 0x02,
            SUBLANG_MALAY_MALAYSIA = 0x01,
            SUBLANG_MALAY_BRUNEI_DARUSSALAM = 0x02,
            SUBLANG_NEPALI_INDIA = 0x02,
            SUBLANG_NORWEGIAN_BOKMAL = 0x01,
            SUBLANG_NORWEGIAN_NYNORSK = 0x02,
            SUBLANG_PORTUGUESE = 0x02,
            SUBLANG_PORTUGUESE_BRAZILIAN = 0x01,
            SUBLANG_SERBIAN_LATIN = 0x02,
            SUBLANG_SERBIAN_CYRILLIC = 0x03,
            SUBLANG_SPANISH = 0x01,
            SUBLANG_SPANISH_MEXICAN = 0x02,
            SUBLANG_SPANISH_MODERN = 0x03,
            SUBLANG_SPANISH_GUATEMALA = 0x04,
            SUBLANG_SPANISH_COSTA_RICA = 0x05,
            SUBLANG_SPANISH_PANAMA = 0x06,
            SUBLANG_SPANISH_DOMINICAN_REPUBLIC = 0x07,
            SUBLANG_SPANISH_VENEZUELA = 0x08,
            SUBLANG_SPANISH_COLOMBIA = 0x09,
            SUBLANG_SPANISH_PERU = 0x0a,
            SUBLANG_SPANISH_ARGENTINA = 0x0b,
            SUBLANG_SPANISH_ECUADOR = 0x0c,
            SUBLANG_SPANISH_CHILE = 0x0d,
            SUBLANG_SPANISH_URUGUAY = 0x0e,
            SUBLANG_SPANISH_PARAGUAY = 0x0f,
            SUBLANG_SPANISH_BOLIVIA = 0x10,
            SUBLANG_SPANISH_EL_SALVADOR = 0x11,
            SUBLANG_SPANISH_HONDURAS = 0x12,
            SUBLANG_SPANISH_NICARAGUA = 0x13,
            SUBLANG_SPANISH_PUERTO_RICO = 0x14,
            SUBLANG_SWEDISH = 0x01,
            SUBLANG_SWEDISH_FINLAND = 0x02,
            SUBLANG_URDU_PAKISTAN = 0x01,
            SUBLANG_URDU_INDIA = 0x02,
            SUBLANG_UZBEK_LATIN = 0x01,
            SUBLANG_UZBEK_CYRILLIC = 0x02
        }
// ReSharper restore InconsistentNaming

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string filename, IntPtr reserved, LoadLibraryFlags flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr module);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumResourceNamesEx(IntPtr module, ResourceType type,
            EnumResourceNamesProcedure procedure, IntPtr parameter, EnumResourceNamesFlags flags, uint language);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr BeginUpdateResource(string filename, bool deleteExisting);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UpdateResource(IntPtr updateHandle, ResourceType type,
            uint id, ushort language, byte[] data, uint size);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndUpdateResource(IntPtr updateHandle, bool discard);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr module, uint id, StringBuilder buffer, int size);
    }
}
