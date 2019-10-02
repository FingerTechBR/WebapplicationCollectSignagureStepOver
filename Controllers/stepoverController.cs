using FT_stepoverAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;

namespace FT_stepoverAPI.Controllers
{
    [RoutePrefix("api/getimage")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class stepoverController : ApiController
    {

        int initRes;
        IntPtr pcert;
        IntPtr psettings;
        IntPtr padID;
        IntPtr timestamp;
        Byte[] imagem;
        TOnSignFinishedHandlerEx onSignFinishedHandlerEx = null;

        bool pegarimagemcontrolador;


        Assinatura assinatura = new Assinatura();
       
      
        
        [HttpGet]
        [Route("imagem")]
        public Byte[] getimagem()
        {



            assinatura.Msg = "ok";
            assinatura.st = status.Recebido;   



            if (WebApiConfig.inservice == false)
            {
                StreamReader streamReader = new StreamReader(@"C:\\Users\\Lincoln\\source\\repos\\FT_stepoverAPI\\FT_stepoverAPI\\bin\\FT_stepoverAPI.xml");
                String drivercertificateXML = streamReader.ReadToEnd();

                initRes = sopadDLL.SOPAD_initialize();
                pcert = Marshal.AllocHGlobal(256);

                psettings = Marshal.AllocHGlobal(256);
                padID = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PidType)));
                timestamp = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GMTStamType)));

                onSignFinishedHandlerEx = new TOnSignFinishedHandlerEx(this.OnDeviceSignFinishedEx);
                // visibel Device Search
                //sopadDLL.SOPAD_configurePad(pcert, true, true, true, psettings);

                // hidden Device search
                sopadDLL.SOPAD_EnumeratePadsFirst(psettings);

                string settings = Marshal.PtrToStringAnsi(psettings);

                //sopadDLL.SOPAD_isPadAvailable(psettings);
                sopadDLL.SOPAD_SetDriverLong(5/*autoFinish*/, 3000);
                // Now you can pass that to the function that needs to call you back.
                sopadDLL.SOPAD_SetDriverLong(4, Marshal.GetFunctionPointerForDelegate(onSignFinishedHandlerEx).ToInt32());

                sopadDLL.SOPAD_SetDriverLong(59, 0);




                //string dialogdriverconfigurationxml;
                //dialogdriverconfigurationxml = streamreader.readtoend();

                //sopaddll.sopad_setdriverstring(3, dialogdriverconfigurationxml);



                pegarimagemcontrolador = false;
                //  S   et OnSignFinish Timer to 3000ms
                try
                {
                   
                    sopadDLL.SOPAD_SetDriverString(3, drivercertificateXML);
                    WebApiConfig.inservice = true;
                    bool x = sopadDLL.SOPAD_startCapture(pcert, false, false, true, true, psettings);
                }
                catch
                {
                    fechar();
                    return null;
                        
                }
                // Console.WriteLine("passei");
                while (pegarimagemcontrolador == false)
                {

                }


                return imagem;

            }
            else
            {
                fechar();
                return imagem;
            }


        }



        [HttpGet]
        [Route("fechar")]
        public void fechar()
        {

            sopadDLL.SOPAD_stopCapture(padID, timestamp, 0);
            Marshal.FreeHGlobal(pcert);
            Marshal.FreeHGlobal(psettings);
            Marshal.FreeHGlobal(padID);
            Marshal.FreeHGlobal(timestamp);
            WebApiConfig.inservice = false;


        }

        public void OnDeviceSignFinishedEx(uint LParam)
        {
            try
            {
                sopadEventCach.PoolPreviewImage = false;
                sopadEventCach.ThreadState = deviceTHREADSTATE.Handle_SignFinish_Event;

                // disable pen drawing
                sopadDLL.SOPAD_SetDriverLong(48, 0);

                // Stops SignMode
                sopadDLL.SOPAD_stopCapture(padID, timestamp, 0);

                // get encrypted biodata as string
                IntPtr biodata = sopadDLL.SOPAD_GetBioDataString();

                // Convert the characters inside the buffer into a managed string.
                string strBio = Marshal.PtrToStringAnsi(biodata);


                // get final image
                int picsize = 0;
                IntPtr picture = sopadDLL.SOPAD_ReadHighResBitmap(0, ref picsize);
                if (picsize > 0)
                {
                    byte[] managedArray = new byte[picsize];
                    imagem = managedArray;
                    Marshal.Copy(picture, managedArray, 0, picsize);
                    MemoryStream ms = new MemoryStream();
                    ms.Write(managedArray, 0, Convert.ToInt32(managedArray.Length));


                }

                fechar();

                WebApiConfig.inservice = false;
                pegarimagemcontrolador = true;

            }
            catch (Exception ex)
            {
                // log errors
                fechar();
            }

        }





        public void OnSignFinishedHandler()
        {
            //do something

            // disable pen drawing
            sopadDLL.SOPAD_SetDriverLong(48, 0);

            // Stops SignMode
            sopadDLL.SOPAD_stopCapture(padID, timestamp, 0);


            // get encrypted biodata as string
            IntPtr biodata = sopadDLL.SOPAD_GetBioDataString();

            // Convert the characters inside the buffer into a managed string.
            string strBio = Marshal.PtrToStringAnsi(biodata);


            // get final image
            int picsize = 0;
            IntPtr picture = sopadDLL.SOPAD_ReadHighResBitmap(0, ref picsize);
            if (picsize > 0)
            {
                byte[] managedArray = new byte[picsize];
                imagem = managedArray;
                Marshal.Copy(picture, managedArray, 0, picsize);
                MemoryStream ms = new MemoryStream();
                ms.Write(managedArray, 0, Convert.ToInt32(managedArray.Length));


            }

            sopadDLL.SOPAD_stopCapture(padID, timestamp, 0);
            Marshal.FreeHGlobal(pcert);
            Marshal.FreeHGlobal(psettings);
            Marshal.FreeHGlobal(padID);
            Marshal.FreeHGlobal(timestamp);

            WebApiConfig.inservice = false;
            pegarimagemcontrolador = true;

        }
    }
}