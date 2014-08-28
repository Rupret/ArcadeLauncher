using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ArcadeLauncher.Core;
using ArcadeLauncher.Core.Enumerables;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher.Configurator.Controllers
{
    public class ConfiguradorJoystickController
    {
        public List<Controlador> Controladores { get; private set; }

        public ConfiguradorJoystickController()
        {
            this.IniciarControladores();
        }

        private void IniciarControladores()
        {
            this.Controladores = ControladorFactory.ObtenerControladoresDisponibles();
            foreach ( var item in this.Controladores )
            {
                item.Inicializar();
            }
        }

        public void GrabarJoystick1( MapeoJoystick mapeo )
        {
            this.GrabarMapeo( mapeo, Configuracion.ArchivoMapeoJoystick1 );
        }
        public void GrabarJoystick2( MapeoJoystick mapeo )
        {
            this.GrabarMapeo( mapeo, Configuracion.ArchivoMapeoJoystick2 );
        }

        private void GrabarMapeo( MapeoJoystick mapeo, string archivo )
        {
            Serializador.Serializar<MapeoJoystick>( mapeo, archivo );
        }

        public MapeoJoystick CargarJoystick1()
        {
            return this.CargarMapeo( Configuracion.ArchivoMapeoJoystick1 );
        }
        public MapeoJoystick CargarJoystick2()
        {
            return this.CargarMapeo( Configuracion.ArchivoMapeoJoystick2 );
        }

        private MapeoJoystick CargarMapeo( string archivo )
        {
            MapeoJoystick mapeoJoystick;
            try
            {
                mapeoJoystick = Deserializador.Deserializar<MapeoJoystick>( archivo );
            }
            catch ( Exception ex )
            {
                mapeoJoystick = new MapeoJoystick();
            }

            return mapeoJoystick;
        }

        public Controlador ObtenerControlador( string id )
        {
            Controlador controlador = this.Controladores.Find( x => x.Id.ToString().Equals( id, StringComparison.InvariantCultureIgnoreCase ) );

            return controlador;
        }
    }
}
