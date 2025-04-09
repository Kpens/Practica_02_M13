using Gestio_Botiga_Calcat.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat
{
    public static class Global
    {
        public static Service mdbService = new Service("Botiga");
        public static UsuariMDB Usuari { get; set; }
        public static CistellManager cistellManager { get; set; }

        public static void Init()
        {
            if(cistellManager == null)
                cistellManager = new CistellManager();

            Usuari = null;
        }

    }
}
