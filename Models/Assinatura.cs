using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_stepoverAPI.Models
{


    //cria uma assinatura  e estrutura para ser enviado via json
    public class Assinatura
    {

        public Assinatura()
        {

        }

        Byte[] imagem_assinatura;


        public  status st { get; set; }


        string msg;


        public byte[] Imagem_assinatura { get => imagem_assinatura; set => imagem_assinatura = value; }
        public string Msg { get => msg; set => msg = value; }
    }
}