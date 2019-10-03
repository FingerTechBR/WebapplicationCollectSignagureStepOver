using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_stepoverAPI.Models
{
    public enum status
    {
        Capturando = 1,
        Reiniciando_captura = 2,
        Capturado_Sucesso = 3,
        Error = 4,
        Fechado = 5

    }
}