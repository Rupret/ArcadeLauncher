using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ArcadeLauncher.Core
{
    public class Juego
    {
        public string NombreArchivo { get; set; }
        public string Nombre { get; set; }
        public string EsClonDe { get; set; }
        public string Fabricante { get; set; }
        public int Anio { get; set; }
        public string Genero { get; set; }
        public JuegoEstadistica Estadisticas { get; set; }

        public Juego()
        {
            this.Estadisticas = new JuegoEstadistica();
        }
    }
}
