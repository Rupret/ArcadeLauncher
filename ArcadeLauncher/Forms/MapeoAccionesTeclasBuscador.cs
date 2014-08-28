using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcadeLauncher.Core.Enumerables;

namespace ArcadeLauncher.Forms
{
    public class MapeoAccionesTeclasBuscador
    {
        public EnumAccionesBuscador ObtenerAccion( Keys keys )
        {
            switch ( keys )
            {
                case Keys.Left:
                    return EnumAccionesBuscador.Anterior;
                case Keys.Right:
                    return EnumAccionesBuscador.Siguiente;
                case Keys.End:
                    return EnumAccionesBuscador.BuscarYAvanzar;
                case Keys.Back:
                    return EnumAccionesBuscador.Borrar;

                default:
                    return EnumAccionesBuscador.NULA;
            }
        }
    }
}
