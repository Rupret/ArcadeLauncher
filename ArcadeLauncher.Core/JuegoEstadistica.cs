using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeLauncher.Core
{
    public class JuegoEstadistica
    {
        public int CantidadDeEjecuciones { get; set; }
        public DateTime FechaUltimaEjecucion { get; set; }
        public string JuegoArchivo { get; set; }

        public JuegoEstadistica()
        {
            this.CantidadDeEjecuciones = 0;
        }
    }
}
