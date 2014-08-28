using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ArcadeLauncher.Core;

namespace ArcadeLauncher
{
    public class Contexto
    {
        private static Contexto instancia;
        private List<Plataforma> plataformas;
        public string ArchivoPlataformas { get; set; }

        public Controladores Controladores { get; set; }

        public static Contexto Instancia
        {
            get
            {
                if ( instancia == null )
                    instancia = new Contexto();

                return instancia;
            }
        }

        private Contexto()
        {
            this.ArchivoPlataformas = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Plataformas.xml" );
            this.ReiniciarControladores();
        }

        public void ReiniciarControladores()
        {
            this.Controladores = new Controladores( this.MapeoJoystick1, this.MapeoJoystick2 );
        }

        public string RutaAplicacion { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public List<Plataforma> Plataformas 
        {
            get
            {
                if ( this.plataformas == null )
                    this.plataformas = Deserializador.Deserializar<List<Plataforma>>( this.ArchivoPlataformas );

                return this.plataformas;
            }
        }

        public void GrabarPlataforma( Plataforma plataforma )
        { 
            Serializador.Serializar<Plataforma>( plataforma, this.ArchivoPlataformas );
        }

        public MapeoJoystick MapeoJoystick1
        {
            get { return Deserializador.Deserializar<MapeoJoystick>( Configuracion.ArchivoMapeoJoystick1 ); } 
        }

        public MapeoJoystick MapeoJoystick2
        {
            get { return Deserializador.Deserializar<MapeoJoystick>( Configuracion.ArchivoMapeoJoystick2 ); }
        }
    }
}
