using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcadeLauncher.Core;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher
{
    public class Controladores
    {
        public Controlador Controlador1 { get; set; }
        public Controlador Controlador2 { get; set; }

        public MapeoJoystick MapeoJoystick1 { get; set; }
        public MapeoJoystick MapeoJoystick2 { get; set; }

        private Dictionary<string, Controlador> controladoresPorId = new Dictionary<string,Controlador>();
        private Dictionary<string, MapeoJoystick> mapeosPorId = new Dictionary<string, MapeoJoystick>();

        public Controladores( MapeoJoystick mapeoJoystick1, MapeoJoystick mapeoJoystick2 )
        {
            this.MapeoJoystick1 = mapeoJoystick1;
            this.MapeoJoystick2 = mapeoJoystick2;
            List<Controlador> controladores = ControladorFactory.ObtenerControladoresDisponibles();
            if ( mapeoJoystick1 != null )
            {
                this.Controlador1 = controladores.FirstOrDefault( x => x.Id.ToString().Equals( mapeoJoystick1.Id ) );
                if ( this.Controlador1 != null )
                {
                    this.Controlador1.Inicializar();
                    this.controladoresPorId.Add( this.Controlador1.Id.ToString(), this.Controlador1 );
                    this.mapeosPorId.Add( this.Controlador1.Id.ToString(), this.MapeoJoystick1 );
                }
            }

            if ( mapeoJoystick2 != null )
            {
                this.Controlador2 = controladores.FirstOrDefault( x => x.Id.ToString().Equals( mapeoJoystick2.Id ) );
                if ( this.Controlador2 != null )
                {
                    this.Controlador2.Inicializar();
                    this.controladoresPorId.Add( this.Controlador2.Id.ToString(), this.Controlador2 );
                    this.mapeosPorId.Add( this.Controlador2.Id.ToString(), this.MapeoJoystick2 );
                }
            }
        }

        public Controlador ObtenerControladorPorId( string id )
        {
            if ( this.controladoresPorId.ContainsKey( id ) )
                return this.controladoresPorId[ id ];
            else
                return null;
        }

        public MapeoJoystick ObtenerMapeoPorId( string id )
        {
            if ( this.mapeosPorId.ContainsKey( id ) )
                return this.mapeosPorId[ id ];
            else
                return null;
        }
    }
}
