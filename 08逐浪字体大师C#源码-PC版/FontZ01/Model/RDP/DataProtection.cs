using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace FontZ01.Model.RDP
{
    [Serializable()]
    public sealed class DataProtection
    {
        [Flags()]
        public enum CryptProtectPromptFlags
        {
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x01,
            CRYPTPROTECT_PROMPT_ON_PROTECT = 0x02,
            CRYPTPROTECT_PROMPT_RESERVED = 0x04,
            CRYPTPROTECT_PROMPT_STRONG = 0x08,
            CRYPTPROTECT_PROMPT_REQUIRE_STRONG = 0x10
        }

        [Flags()]
        public enum CryptProtectDataFlags
        {
            CRYPTPROTECT_UI_FORBIDDEN = 0x01,
            CRYPTPROTECT_LOCAL_MACHINE = 0x04,
            CRYPTPROTECT_CRED_SYNC = 0x08,
            CRYPTPROTECT_AUDIT = 0x10,
            CRYPTPROTECT_NO_RECOVERY = 0x20,
            CRYPTPROTECT_VERIFY_PROTECTION = 0x40,
            CRYPTPROTECT_CRED_REGENERATE = 0x80
        }

        #region 加密数据
        public static string ProtectData(string data, string name)
        {
            return ProtectData(data, name,
                CryptProtectDataFlags.CRYPTPROTECT_UI_FORBIDDEN | CryptProtectDataFlags.CRYPTPROTECT_LOCAL_MACHINE);
        }

        public static byte[] ProtectData(byte[] data, string name)
        {
            return ProtectData(data, name,
                CryptProtectDataFlags.CRYPTPROTECT_UI_FORBIDDEN | CryptProtectDataFlags.CRYPTPROTECT_LOCAL_MACHINE);
        }

        public static string ProtectData(string data, string name, CryptProtectDataFlags flags)
        {
            byte[] dataIn = Encoding.Unicode.GetBytes(data);
            byte[] dataOut = ProtectData(dataIn, name, flags);

            if (dataOut != null)
                return (Convert.ToBase64String(dataOut));
            else
                return null;
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data">要加密的明文数据</param>
        /// <param name="name">有意义的描述，此描述会加到加密后的数据中</param>
        /// <param name="dwFlags">flags的位标志</param>
        /// <returns></returns>
        private static byte[] ProtectData(byte[] data, string name, CryptProtectDataFlags dwFlags)
        {
            byte[] cipherText = null;

            // copy data into unmanaged memory
            //DATA_BLOB结构，用于CryptProtectData参数
            DPAPI.DATA_BLOB din = new DPAPI.DATA_BLOB();
            din.cbData = data.Length;

            //Marshal类的作用：提供了一个方法集，这些方法用于分配非托管内存、复制非托管内存块、将托管类型转换为非托管类型，
            //此外还提供了在与非托管代码交互时使用的其他杂项方法。 
            //为din.pbData分配内存
            din.pbData = Marshal.AllocHGlobal(din.cbData);

            //InPtr结构:用于表示指针或句柄的平台特定类型
            //分配内存错误，抛出内存不足异常
            //IntPtr.Zero:一个只读字段，代表已初始化为零的指针或句柄
            if (din.pbData.Equals(IntPtr.Zero))
                throw new OutOfMemoryException("Unable to allocate memory for buffer.");

            //将data数组中的数据复制到pbData内存指针中
            Marshal.Copy(data, 0, din.pbData, din.cbData);

            //声明DPAPI类的DATA_BLOB公共结构类型
            DPAPI.DATA_BLOB dout = new DPAPI.DATA_BLOB();

            try
            {
                //加密数据
                bool cryptoRetval = DPAPI.CryptProtectData(ref din, name, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, dwFlags, ref dout);

                //判断加密是否成功
                if (cryptoRetval) // 成功
                {
                    int startIndex = 0;
                    //分配cipherText数据元素大小为dout.cbData
                    cipherText = new byte[dout.cbData];
                    //从dout.pbData内存指针指向的内容拷贝到byte数组cipherText中
                    Marshal.Copy(dout.pbData, cipherText, startIndex, dout.cbData);
                    //从内存中释放指针指向的数据I
                    DPAPI.LocalFree(dout.pbData);
                }
                else
                {
                    //加密失败，获得错误信息
                    int errCode = Marshal.GetLastWin32Error();
                    StringBuilder buffer = new StringBuilder(256);
                    //显示错误信息
                    Win32Error.FormatMessage(Win32Error.FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, errCode, 0, buffer, buffer.Capacity, IntPtr.Zero);
                }
            }
            finally
            {
                // 如果din.pbData非空，则释放din.pbData使用的内存
                if (!din.pbData.Equals(IntPtr.Zero))
                    Marshal.FreeHGlobal(din.pbData);
            }

            //返回加密后的数据
            return cipherText;
        }
        #endregion


        //解密数据

        internal static void InitPromptstruct(ref DPAPI.CRYPTPROTECT_PROMPTSTRUCT ps)
        {
            ps.cbSize = Marshal.SizeOf(typeof(DPAPI.CRYPTPROTECT_PROMPTSTRUCT));
            ps.dwPromptFlags = 0;
            ps.hwndApp = IntPtr.Zero;
            ps.szPrompt = null;
        }
    }

    //允许托管代码不经过堆栈步即调入非托管代码
    [SuppressUnmanagedCodeSecurityAttribute()]
    internal class DPAPI
    {
        [DllImport("crypt32")]
        public static extern bool CryptProtectData(ref DATA_BLOB dataIn, string szDataDescr, IntPtr optionalEntropy, IntPtr pvReserved,
            IntPtr pPromptStruct, DataProtection.CryptProtectDataFlags dwFlags, ref DATA_BLOB pDataOut);

        [DllImport("crypt32")]
        public static extern bool CryptUnprotectData(ref DATA_BLOB dataIn, StringBuilder ppszDataDescr, IntPtr optionalEntropy,
            IntPtr pvReserved, IntPtr pPromptStruct, DataProtection.CryptProtectDataFlags dwFlags, ref DATA_BLOB pDataOut);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [StructLayout(LayoutKind.Sequential)]
        public struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize; // = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT))
            public int dwPromptFlags; // = 0
            public IntPtr hwndApp; // = IntPtr.Zero
            public string szPrompt; // = null
        }
    }


    internal class Win32Error
    {
        [Flags()]
        public enum FormatMessageFlags : int
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x0100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x0200,
            FORMAT_MESSAGE_FROM_STRING = 0x0400,
            FORMAT_MESSAGE_FROM_HMODULE = 0x0800,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x1000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000,
            FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xFF,
        }

        [DllImport("Kernel32.dll")]
        public static extern int FormatMessage(FormatMessageFlags flags, IntPtr source, int messageId, int languageId,
            StringBuilder buffer, int size, IntPtr arguments);
    }
}
