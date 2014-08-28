using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ArcadeLauncher.Core.Enumerables;

namespace ArcadeLauncher.Core
{
    public class ItemAccionBotonJoystick
    {
        public string IdBoton { get; set; }
        public EnumAcciones Accion { get; set; }
        public EnumAccionesBuscador AccionBuscador { get; set; }
    }
}
