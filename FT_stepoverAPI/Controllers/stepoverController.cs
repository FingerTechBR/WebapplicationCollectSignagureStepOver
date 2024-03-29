﻿using FT_stepoverAPI.Models;
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
        public Assinatura getimagem()
        {          



            if (WebApiConfig.inservice == false)
            {
                String drivercertificateXML = null;
                  
                try
                {
                    StreamReader streamReader = new StreamReader("C:\\FingertechWeb PAD\\FT_stepoverAPI.xml");
                     drivercertificateXML = streamReader.ReadToEnd();

                }
                catch (FileNotFoundException e)
                {
                    drivercertificateXML = "Certificado não encontrado";
                }
              
               
                initRes = sopadDLL.SOPAD_initialize();
                pcert = Marshal.AllocHGlobal(256);
                psettings = Marshal.AllocHGlobal(256);
                padID = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PidType)));
                timestamp = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GMTStamType)));
                onSignFinishedHandlerEx = new TOnSignFinishedHandlerEx(this.OnDeviceSignFinishedEx);              
                bool dispositivo_conectado = sopadDLL.SOPAD_EnumeratePadsFirst(psettings);                                            
                sopadDLL.SOPAD_SetDriverLong(5/*Auto finalizar assinatura time - >*/, 3000);
                
                sopadDLL.SOPAD_SetDriverLong(4, Marshal.GetFunctionPointerForDelegate(onSignFinishedHandlerEx).ToInt32());
                sopadDLL.SOPAD_SetDriverLong(59, 0);
                if (!dispositivo_conectado)
                {
                    assinatura.Msg = "Não foi Possível iniciar o Dispositivo";
                    assinatura.st = status.Error;
                    return assinatura;
                }
                pegarimagemcontrolador = false;
               
                try
                {
                   

                    sopadDLL.SOPAD_SetDriverString(3, drivercertificateXML);
                    WebApiConfig.inservice = true;
                    bool x = sopadDLL.SOPAD_startCapture(pcert, false, false, true, true, psettings);
                    assinatura.Msg = "Dispositivo encontrado, captura Iniciada.";
                    assinatura.st = status.Capturando;
                }
                catch
                {
                    fechar();
                    assinatura.Msg = "Não foi Possível iniciar o Dispositivo";
                    assinatura.st = status.Error;
                    return assinatura;
                        
                }
                // Console.WriteLine("passei");

                while (pegarimagemcontrolador == false)
                {

                }

                assinatura.Imagem_assinatura = imagem;
                assinatura.Msg = "Captura Finalizada com Sucesso";
                assinatura.st = status.Capturado_Sucesso;
                Debug.WriteLine(assinatura.st);
                return assinatura;

            }
            else
            {

                assinatura.Msg = "Dispositivo encontrado, dispositivo em Captura";
                assinatura.st = status.Reiniciando_captura;
                fechar();
                return assinatura;
            }


        }



        [HttpGet]
        [Route("fechar")]
        public Assinatura fechar()
        {

            assinatura.Msg = "Captura Fechada";
            assinatura.st = status.Fechado;
            sopadDLL.SOPAD_stopCapture(padID, timestamp, 0);
            Marshal.FreeHGlobal(pcert);
            Marshal.FreeHGlobal(psettings);
            Marshal.FreeHGlobal(padID);
            Marshal.FreeHGlobal(timestamp);
            WebApiConfig.inservice = false;
            return assinatura;


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




    }
}