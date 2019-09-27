using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace FT_stepoverAPI
{
    public struct PidType { public byte b1, b2, bb3, b4, b5; }
    public struct GMTStamType { public byte years, months, days, hours, minutes, seconds; }

    //delegate types (typedef for callbacks)
    public delegate void OnSignFinishedType();
    public delegate void OnLCDSignButtonType();


    class sopadDLL
    {
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_initialize();

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_uninitialize();


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_isPadAvailable(IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_configurePad(IntPtr certificate, bool bShowSimpleDialog,
            bool bAutostartIfSimpleDialog, bool bSaveToRegistry, IntPtr padSettings);


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_startCapture(IntPtr certificate, bool CheckPad,
            bool AutostartSerching, bool showConnectionWinIfAutostart, bool ReadAndSaveInRegistry,
            IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_stopCapture(IntPtr padID, IntPtr timeStamp);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_SetDriverLong(int Key, int Value);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_GetDriverLong(int Key);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern string SOPAD_GetDriverString(int Key);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetBioDataString();


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_ReadPreviewImage(int nTypeOfPic, int nWidth, int nHeight, ref int pnLenOfImage);


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetEncryptedAesKey(ref int keyLen);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_ReadHighResBitmap(int TypeOfPic, ref int pnLenOfImage);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetDeviceCertificate(ref int outLen);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_SetFinalDocumentHash(IntPtr pHash, int nSize);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_SetPreliminaryDocumentHash(IntPtr pHash, int nSize);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetSignedDocHash(ref int status, ref int outLen);


    }
}