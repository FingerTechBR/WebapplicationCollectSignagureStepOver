using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using System.Web.Http;
using System.Web.Http.Cors;

namespace FT_stepoverAPI.Controllers
{

 
    [RoutePrefix("api/getimage")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StepOVerController : ApiController
    {

        bool initRes;
        IntPtr pcert;
        IntPtr psettings;
        IntPtr padID;
        IntPtr timestamp;
        Byte[] imagem;
      
        bool pegarimagemcontrolador;
        

       


        public StepOVerController()
        {
           //inicializar();
        }


        [HttpGet]
      
        [Route("imagem")]
        public  Byte[] getimagem()
        {


            if (WebApiConfig.inservice == false) {


                 initRes = sopadDLL.SOPAD_initialize();
                 pcert = Marshal.AllocHGlobal(256);
                 psettings = Marshal.AllocHGlobal(256);
                 padID = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PidType)));
                timestamp = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GMTStamType)));

                OnSignFinishedType myDelegate = new OnSignFinishedType(this.OnSignFinishedHandler);
                sopadDLL.SOPAD_SetDriverLong(5/*autoFinish*/, 3000);
                // Now you can pass that to the function that needs to call you back.
                sopadDLL.SOPAD_SetDriverLong(4/*OnSignFinishedHandler*/, Marshal.GetFunctionPointerForDelegate(myDelegate).ToInt32());
                sopadDLL.SOPAD_SetDriverLong(59, 0);
                pegarimagemcontrolador = false;
                //  S   et OnSignFinish Timer to 3000ms
                try
                  {
                    WebApiConfig.inservice = true;
                    bool x = sopadDLL.SOPAD_startCapture(pcert, false, true, true, false, psettings);                
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

            sopadDLL.SOPAD_stopCapture(padID, timestamp);
            Marshal.FreeHGlobal(pcert);
            Marshal.FreeHGlobal(psettings);
            Marshal.FreeHGlobal(padID);
            Marshal.FreeHGlobal(timestamp);
            WebApiConfig.inservice = false;


        }

        public void inicializar()
        {

            // Don't download BG image, don't do alpha-blending for ColourPad default SignMode 
            OnSignFinishedType myDelegate = new OnSignFinishedType(this.OnSignFinishedHandler);

            sopadDLL.SOPAD_SetDriverLong(5/*autoFinish*/, 3000);

            // Now you can pass that to the function that needs to call you back.
            sopadDLL.SOPAD_SetDriverLong(4/*OnSignFinishedHandler*/, Marshal.GetFunctionPointerForDelegate(myDelegate).ToInt32());
            sopadDLL.SOPAD_SetDriverLong(59, 0);

            //  Start SignMode

           

            

        }



     
        public void OnSignFinishedHandler()
        {
            //do something

            // disable pen drawing
            sopadDLL.SOPAD_SetDriverLong(48, 0);

            // Stops SignMode
            sopadDLL.SOPAD_stopCapture(padID, timestamp);


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
               
                

            }

            sopadDLL.SOPAD_stopCapture(padID, timestamp);
            Marshal.FreeHGlobal(pcert);
            Marshal.FreeHGlobal(psettings);
            Marshal.FreeHGlobal(padID);
            Marshal.FreeHGlobal(timestamp);

            WebApiConfig.inservice = false;
            pegarimagemcontrolador = true;

        }
    }
}
