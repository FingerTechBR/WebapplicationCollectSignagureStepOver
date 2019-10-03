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
    public delegate void TOnSignFinishedHandler();
    //public delegate void OnLCDSignButtonType();
    // public delegate void OnDeviceButtonType(uint Button, uint lParam);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate bool TOnCheckTerminateDialogHandler(uint DialogHandle, uint lParam);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnDeviceFrameArrivedHandler();

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnDeviceRemovedHandler(uint PadType, string Identifier, uint IdentLen);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnDeviceErrorHandler(uint ErrorType, string Identifier, uint IdentLen);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnSignFinishedHandlerEx(uint LParam);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnDeviceButtonHandlerEx(uint Button, uint lParam);

    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
    public delegate void TOnDevicePenEventHandler(uint Mode, uint SubMode, uint PenState, uint XPos, uint YPos, uint lParam);

    [Flags]
    public enum deviceStatus
    {
        wrongPadType = -1,
        noDevice = 0,
        colourPad = 1,
        flawless_comfort = 2,
        brilliance = 3,
        tenInchPad = 4,
    }

    [Flags]
    public enum deviceTHREADSTATE
    {
        Idle = 0,
        Poll_PreviewImage = 1,
        Wait_Doc_hash_confirm = 2,
        Handle_SignFinish_Event = 3,
        Handle_Button_Pressed_Event = 4,
        Handle_Device_Removed_Event = 5,
        Handle_Device_Error_Event = 6,
        Handle_Device_Dialog_Terminated = 7,
        Handle_Device_PenEvent = 8,
    }

    /// <summary>
    /// a static class to halt runtime events and states
    /// </summary>
    static class sopadEventCach
    {
        public static uint DeviceButtonCode = 0;
        public static uint DeviceButtonLParam = 0;

        public static uint DeviceEventMode = 0;
        public static uint DeviceEventSubMode = 0;

        public static uint DevicePenstate = 0;
        public static uint DevicePenXPos = 0;
        public static uint DevicePenYPos = 0;

        public static bool PoolPreviewImage = false;

        public static uint ErrorType = 0;
        public static string ErrorIdentifier = "";

        public static uint RemovedPadType = 0;
        public static string RemovedPadIdentifier = "";

        public static uint DialogHandle = 0;
        public static uint DialogLParam = 0;


        public static deviceTHREADSTATE ThreadState = deviceTHREADSTATE.Idle;

    }





    class sopadDEF
    {
        //PAD mode (only the necessary ones)
        public const uint HidMode_hmSign = 0;
        public const uint HidMode_hmStandBy = 1;
        public const uint HidMode_hmViewDoc = 5;
        public const uint HidMode_hmViewDocSign = 6;
        public const uint HidMode_hmConfiguration = 7;

        //BUTTON TYPE - naturaSign Colour LCD buttons. P = Plus, M = Minus
        public const uint HID_BUTTON_ZOOM_P = 200;
        public const uint HID_BUTTON_ZOOM_M = 201;
        public const uint HID_BUTTON_NEXT = 202;
        public const uint HID_BUTTON_PREV = 203;
        public const uint HID_BUTTON_START_SIGN = 204;
        public const uint HID_BUTTON_ROTATE = 205;
        public const uint HID_BUTTON_OK = 206;
        public const uint HID_BUTTON_REPEAT = 207;
        public const uint HID_BUTTON_CANCEL = 208;
        public const uint HID_BUTTON_SCRIPT_FINISHED = unchecked((uint)0xFFFFFFFF);
        public const uint HID_SOPAD_EVENT_CANCEL_SIG = 2000;
        public const uint HID_SOPAD_EVENT_REPEAT_SIG = 2001;


        //button events for documentVIEW-mode
        public const uint DOCVIEW_HID_BUTTON_ZOOM_P = (HID_BUTTON_ZOOM_P << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_ZOOM_M = (HID_BUTTON_ZOOM_M << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_NEXT = (HID_BUTTON_NEXT << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_PREV = (HID_BUTTON_PREV << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_START_SIGN = (HID_BUTTON_START_SIGN << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_ROTATE = (HID_BUTTON_ROTATE << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_OK = (HID_BUTTON_OK << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_REPEAT = (HID_BUTTON_REPEAT << 16) | HidMode_hmViewDoc;
        public const uint DOCVIEW_HID_BUTTON_CANCEL = (HID_BUTTON_CANCEL << 16) | HidMode_hmViewDoc;

        //button events for documentSIGN-mode
        public const uint DOCSIGN_HID_BUTTON_ZOOM_P = (HID_BUTTON_ZOOM_P << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_ZOOM_M = (HID_BUTTON_ZOOM_M << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_NEXT = (HID_BUTTON_NEXT << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_PREV = (HID_BUTTON_PREV << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_START_SIGN = (HID_BUTTON_START_SIGN << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_ROTATE = (HID_BUTTON_ROTATE << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_OK = (HID_BUTTON_OK << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_REPEAT = (HID_BUTTON_REPEAT << 16) | HidMode_hmViewDocSign;
        public const uint DOCSIGN_HID_BUTTON_CANCEL = (HID_BUTTON_CANCEL << 16) | HidMode_hmViewDocSign;

        //button events for standardSIGN-mode
        public const uint STDSIGN_HID_BUTTON_ZOOM_P = (HID_BUTTON_ZOOM_P << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_ZOOM_M = (HID_BUTTON_ZOOM_M << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_NEXT = (HID_BUTTON_NEXT << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_PREV = (HID_BUTTON_PREV << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_START_SIGN = (HID_BUTTON_START_SIGN << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_ROTATE = (HID_BUTTON_ROTATE << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_OK = (HID_BUTTON_OK << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_REPEAT = (HID_BUTTON_REPEAT << 16) | HidMode_hmSign;
        public const uint STDSIGN_HID_BUTTON_CANCEL = (HID_BUTTON_CANCEL << 16) | HidMode_hmSign;
    }
    /*
    // Search for configured device on all USB prolific adapters
    // on startCapture and checkConnectedPad functions
    // (not used currently)
    DRIVER_OPTION1_FAST_SEARCH_PROLIFIC_ON_OPEN = $00000001;

	// Don't check if pad is connected on startCapture.
	// Performs a bit faster.
	DRIVER_OPTION1_DONT_CHECK_PAD_ON_STARTCAPTURE = $00000002;

	// Disables notification message box after retrieving pad id
	// with the help of 5-sec. pad button press.
	DRIVER_OPTION1_DISABLE_PADID_MESSAGEBOX = $00000004;

	// Disables drawing of lines on LCD while user moves the pen over the sensor.
	DRIVER_OPTION1_DISABLE_LCD_PEN_DRAWING = $00000008;

	// Truns transparent drawing of bitmaps on
	DRIVER_OPTION1_TRANSPARENT_DRAW_MODE = $00000010;

	// Don't show device search dialog in checkConnectedPad
	DRIVER_OPTION1_BLIND_SEARCH_IN_CHECKPAD = $00000020;

	// Truns frame filter off
	DRIVER_OPTION1_FRAME_FILTER_OFF = $00000040;

	// Enables emulation of naturaSign V3 for older devices
	DRIVER_OPTION1_ENABLE_EMULATION = $00000080;

	// Disables automated computation of signature preview image
	DRIVER_OPTION1_DISABLE_PREVIEW = $00000100;

	// ReadHighResBitmap waits until final image is received completely
	DRIVER_OPTION1_WAIT_FINAL_IMAGE = $00000200;

	// User-defined password is required in NSV3 emulation mode
	DRIVER_OPTION1_USE_PASSWORD_FOR_EMULATION = $00000400;

	// Use emulation even for devices that support internal encryption (naturaSignV3, ColourPad)
	DRIVER_OPTION1_DIGITAL_SIGNATURE_EMULATION_FOR_NEW_DEVICES = $00000800;

	// Don't download BG image, don't do alpha-blending for ColourPad
	DRIVER_OPTION1_DISABLE_CP_BACKGROUND_IMAGE = $00001000;

	// Prohibits using of devices with 'opened' flag set
	DRIVER_OPTION1_ENABLE_PADOPEN_CHECK = $00002000;

	// Disables writing of generated keys on disk in emulation mode (see KeyPairStorage*.pas)
	DRIVER_OPTION1_STORE_EMULATION_KEYS_IN_MEMORY = $00004000;

	// Stretch signature to fill whole signature image (keep dimensions of signature image)
	DRIVER_OPTION1_STRETCH_SIGNATURE = $00008000;

	// Crop white borders around signature (make signature image smaller)
	DRIVER_OPTION1_CROP_SIGNATURE = $00010000;
    */

    /// <summary>
    /// /handling the sopad.dll  events 
    /// </summary>
    class sopadCALLBACK
    {


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_registerOnDeviceRemovedHandler(IntPtr lpHandler);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_registerOnDeviceErrorHandler(IntPtr lpHandler);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_registerOnDeviceFrameArrived(IntPtr lpHandler);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_registerOnDeviceButtonEx(IntPtr lpHandler);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_RegisterOnDeviceSignFinishedEx(IntPtr lpHandler);

    }

    /// <summary>
    /// handling the signature Pad
    /// </summary>
    class sopadDLL
    {
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_initialize();

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_uninitialize();


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_isPadAvailable(IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_EnumeratePadsFirst(IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_EnumeratePadsNext(IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_configurePad(IntPtr certificate, bool bShowSimpleDialog,
            bool bAutostartIfSimpleDialog, bool bSaveToRegistry, IntPtr padSettings);


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_startCapture(IntPtr certificate, bool CheckPad,
            bool AutostartSerching, bool showConnectionWinIfAutostart, bool ReadAndSaveInRegistry,
            IntPtr padSettings);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_stopCapture(IntPtr padID, IntPtr timeStamp, int LParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_SetDriverString(int Key, String Value);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_SetDriverLong(int Key, int Value);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_GetDriverLong(int Key);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_GetDriverString(int Key, IntPtr Value);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetBioDataString();


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_ReadPreviewImage(int nTypeOfPic, int nWidth, int nHeight, ref int pnLenOfImage);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_LCDImageExR(int ImageKind, int x, int y, IntPtr hImage);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean SOPAD_CreateSignatureRectangle(int x, int y, int Width, int Height, int Color);

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

        //overloaded function  
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_SetPreliminaryDocumentHash(byte[] pHash, int nSize);


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetSignedDocHash(ref int status, ref int outLen);


        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_setDriverOptions(uint Options1, uint Options2);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_setDriverOptions1Flag(uint flagID, bool state);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_setDriverOptions2Flag(uint flagID, uint state);



        //gen12-support
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetLcdButtonOptions(uint Button, uint Mode, uint Enabled, uint Visible, uint Color);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSignatureLineColor(uint ModeType, uint Color);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SignPadRefreshOptions(uint ModeType);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SignPadResetConfiguration(uint SoftResetStrategy);


    }



    /// <summary>
    /// handling the dialog (from NsUiDriver.dll)
    /// </summary>
    class sopadDIALOG
    {


        /**
        * Creates the dialog driver from a XML Buffer and name
        * note:  use IntPtr instead of [MarshalAs(UnmanagedType.LPWStr)]String
        * @return the unsigned int ( a dialog handle)
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SOPAD_UID_CreateFromStream(/*string*/IntPtr dialogDriverConfigurationXml, /*string*/IntPtr dialogName);

        /**
        * Creates the dialog driver from a File ane name
        * note:  use IntPtr instead of [MarshalAs(UnmanagedType.LPWStr)]String
        * @return the unsigned int (dialog handle)
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SOPAD_UID_CreateFromFile(/*string*/IntPtr FileName, /*string*/IntPtr dialogName);



        /**
            * release dialog 
            *
            * @param dialogDriver the dialog driver
            * @return the void
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SOPAD_UID_Release(uint DialogHandle);

        /**
        * RUN dialog 
        *
        * @param dialogDriver the dialog driver
        * @CheckHandler pointer to callback function (OnCheckTerminateDialogHandler)
        * @return the bool
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_Run(uint DialogHandle, IntPtr /*void**/ CheckHandler, uint lParam);


        /**
            * SetName the current dialog
            * note:  use IntPtr instead of [MarshalAs(UnmanagedType.LPWStr)]String
            * @param dialogDriver the dialog driver
            * @return the bool
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_SetName(uint DialogHandle, /*string*/IntPtr DialogName);

        /**
        * Dialog set current state (SOPAD_UID_DialogSetCurrentState)
        *
        * @param dialogDriver the dialog driver
        * @param dialogId the dialog id
        * @param stateId the state id
        * @return the int
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_SetState(uint DialogHandle, uint StateId); //SOPAD_UID_DialogSetCurrentState

        /**
            * State get variable id.
            * note:  use IntPtr instead of [MarshalAs(UnmanagedType.LPWStr)]String
            * @param dialogDriver the dialog driver
            * @param dialogId the dialog id
            * @param stateId the state id
            * @param variableName the variable name
            * @param variableId the variable id
            * @return the int
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_StateGetVariableId(uint dialogId, uint stateId,/*string*/IntPtr variableName, ref uint variableId);


        /**
        * Dialog get state id by name. (SOPAD_UID_DialogGetStateIdByName)
        *
        * @param dialogDriver the dialog driver
        * @param dialogId the dialog id
        * @param stateName the state name
        * @param stateId the state id
        * @return the int
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_GetStateIdByName(uint DialogHandle, /*string*/IntPtr State, ref uint StateId);

        /**
            * Dialog get state id by type (SOPAD_UID_GetStateIdByType)
            *
            * @param dialogDriver the dialog driver
            * @param dialogId the dialog id
            * @param stateType the state type
            * @param stateId the state id
            * @return the int
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //ORG:: public static extern int SOPAD_UID_DialogGetStateIdByType(uint dialogDriver, uint dialogId, uint stateType, ref uint stateId);
        public static extern bool SOPAD_UID_GetStateIdByType(uint DialogHandle, uint State, ref uint StateId);

        /**
            * Dialog get current state.
            *
            * @param dialogDriver the dialog driver
            * @param dialogId the dialog id
            * @param stateId the state id
            * @return the int
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_GetState(uint DialogHandle, ref uint StateId);  //SOPAD_UID_DialogGetCurrentState


        /**
        * state:  Get variable count
        *
        * @param dialogDriver the dialog driver
        * @param dialogId the dialog id
        * @param stateId the state id
        * @return the int
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_StateGetVariableCount(uint DialogHandle, uint stateId, ref uint variableCount);


        /**
        * state:  Get variableId by index
        *
        * @param dialogDriver the dialog driver
        * @param dialogId the dialog id
        * @param stateId the state id
        * @return the bool
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_StateGetVariableIdByIndex(uint DialogHandle, uint StateId, uint AIndex, ref uint VariableId);



        /**
            * Variable get value.
            *
            * @param dialogDriver the dialog driver
            * @param variableId the variable id
            * @param resultText the result text
            * @param resultTextSize the result text size
            * @param reply the reply
            * @return the int
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_VariableGetValue(uint DialogHandle, uint VariableId, IntPtr resultText, uint resultTextSize);


        /**
            * Variable set value.
            *
            * @param dialogDriver the dialog driver
            * @param variableId the variable id
            * @param value the value
            * @return the bool
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_VariableSetValue(uint DialogHandle, uint VariableId, IntPtr VariableValue);



        /**
            * Variable get name .
            *
            * @param dialogDriver the dialog driver
            * @param variableId the variable id
            * @param value the value
            * @return the bool
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_VariableGetName(uint DialogHandle, uint VariableId, /*string*/IntPtr resultText, uint resultTextSize);


        /**
            * Get Variable id .
            *
            * @param dialogDriver the dialog driver
            * @param variableName the variable name
            * @param variableID the variable id
            * @return the bool
            */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_DialogGetVariableId(uint DialogHandle, /*string*/IntPtr VariableName, ref uint VariableId);


        /**
        * Get Variable id .
        *
        * @param dialogDriver the dialog driver
        * @param variableCount the amunt of variable
        * @return the bool
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_DialogGetVariableCount(uint DialogHandle, ref uint variableCount);


        /**
        * Get Dialog id by  index .
        *
        * @param dialogDriver the dialog driver
        * @param ResDialogID  ref to  ResDialogID
        * @return the bool
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_DialogGetDialogIdByIndex(uint DialogHandle, uint dialogIndex, ref uint ResDialogID);

        /**
        * Get amount of dialogs .
        *
        * @param dialogDriver the dialog driver
        * @param dialogCount  ref to  dialogCount
        * @return the bool
        */
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SOPAD_UID_DriverGetDialogCount(uint DialogHandle, ref uint dialogCount);

    }

    /// <summary>
    /// promoscreen  & configuration file handling
    /// handling the PAD resources   !!! (not finished)
    /// </summary>
    class sopadRESOURCES
    {

        /// <summary>
        /// promoscreen  & configuration file handling
        /// </summary>

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_DownloadPromoImage(int ImageNumber, IntPtr ImageFilePath);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_UploadPromoImage(int ImageNumber, IntPtr ImageFilePath);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_RemovePromoImage(int ImageNumber);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_PromoImageSetOptions(int ImageNumber, int Orientation, int Delay, int Effect, int WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_PromoImageOptionsDoAction(IntPtr ConfigFileOrData, int Action, int WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SOPAD_GetPadResourceFileList(int WPARAM);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_GetPadResourceFileInfoById(int ResourceID, IntPtr ResourceName, ref int ResourceSize);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_GetPadResourceFileInfoByName(IntPtr ResourceName, ref int ResourceID, ref int ResourceSize);


        /// <summary>
        /// handling the PAD resources   !!! (not finished)
        /// </summary>
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_UploadPadResourceFileByName(IntPtr ResourceName, IntPtr FilePath, Int32 WParam, Int32 LParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_DownloadPadResourceFileByName(IntPtr ResourceName, IntPtr FilePath, Int32 WParam, Int32 LParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_RemovePadResourceFileByName(IntPtr ResourceName, Int32 WParam, Int32 LParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_RenamePadResourceFileName(IntPtr ResourceName, IntPtr NewResourceName, Int32 WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_ConvertPadResourceImage(IntPtr SourceFile, IntPtr Destination, Int32 WParam);

    }

    /// <summary>
    /// handling the simple dialog
    /// </summary>
    class sopadSimpleDIALOG
    {
        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSimpleDialogResourceScript(IntPtr ResourceName, IntPtr ScriptFilePath, Int32 WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSimpleDialogResourceFont(IntPtr ResourceName, IntPtr ResourceFilePath, Int32 WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSimpleDialogResourceImage(IntPtr ResourceName, IntPtr ResourceFilePath, Int32 XPos, Int32 YPos, Int32 WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSimpleDialogResourceText(IntPtr ResourceText, Int32 XPos, Int32 YPos, Int32 Color, Int32 Background, Int32 WParam);

        [DllImport("sopadd2c.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SOPAD_SetSimpleDialogDisplayOption(Int32 Option, Int32 WParam);
    }

}
