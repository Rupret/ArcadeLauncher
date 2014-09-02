using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcadeLauncher.Core.Enumerables;

namespace ArcadeLauncher.Core
{
    public class MapeoJoystick
    {
        public List<ItemAccionBotonJoystick> AccionesSegunBoton { get; set; }
        public string Id { get; set; }

        public MapeoJoystick()
        {
            this.AccionesSegunBoton = new List<ItemAccionBotonJoystick>();
        }

        public EnumAcciones ObtenerAccionDeBoton( string[] botones )
        {
            var accionSegunBoton = this.AccionesSegunBoton.Find( x => x.Botones.SequenceEqual( botones ) );
            if ( accionSegunBoton == null )
                return EnumAcciones.Nada;
            else
                return accionSegunBoton.Accion;
        }

        public ItemAccionBotonJoystick ObtenerItemAccionesDeBoton( string[] botones )
        {
            ItemAccionBotonJoystick accionSegunBoton = this.AccionesSegunBoton.Find( x => x.Botones.SequenceEqual( botones ) );
            if ( accionSegunBoton == null )
            {
                accionSegunBoton = new ItemAccionBotonJoystick();
                accionSegunBoton.Accion = EnumAcciones.Nada;
                accionSegunBoton.AccionBuscador = EnumAccionesBuscador.NULA;
            }
            return accionSegunBoton;
        }

        public EnumAcciones ObtenerAccionDeBoton( string boton )
        {
            var accionSegunBoton = this.AccionesSegunBoton.Find( x => x.IdBoton.Equals( boton, StringComparison.InvariantCultureIgnoreCase ) );
            if ( accionSegunBoton == null )
            {
                return EnumAcciones.Nada;
            }
            else
            {
                return accionSegunBoton.Accion;
            }
        }

        public ItemAccionBotonJoystick ObtenerItemAccionesDeBoton( string boton )
        {
            var accionSegunBoton = this.AccionesSegunBoton.Find( x => x.IdBoton.Equals( boton, StringComparison.InvariantCultureIgnoreCase ) );
            if ( accionSegunBoton == null )
            {
                accionSegunBoton = new ItemAccionBotonJoystick();
                accionSegunBoton.Accion = EnumAcciones.Nada;
                accionSegunBoton.AccionBuscador = EnumAccionesBuscador.NULA;
            }

            return accionSegunBoton;
        }
    }
}
