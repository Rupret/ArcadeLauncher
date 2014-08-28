using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ArcadeLauncher.Core;
using ArcadeLauncher.Core.Enumerables;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher.Spike
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click( object sender, EventArgs e )
        {
            MapeoJoystick mapeo = new MapeoJoystick();
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Ejecutar, IdBoton = "Boton0" } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.PageDown, IdBoton = "Boton1" } );
            mapeo.Id = Guid.NewGuid().ToString();

            StreamWriter sw = new StreamWriter( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Joystick1.xml" ) );
            XmlSerializer serializer = new XmlSerializer( typeof( MapeoJoystick ));
            serializer.Serialize( sw, mapeo );            

            //XmlSerializer serializer = new XmlSerializer( typeof( ItemAccionBotonJoystick[] ), new XmlRootAttribute() { ElementName = "items" } );
            //serializer.Serialize( sw, mapeo.AccionesSegunBoton.Select( kv => new ItemAccionBotonJoystick() { Id = kv.Key, Accion = kv.Value } ).ToArray() );
        }

        private void button2_Click_1( object sender, EventArgs e )
        {
            JoystickReader jr = new JoystickReader();
            jr.Inicializar();
            var joys = jr.ObtenerJoysticks();
            joys[ 0 ].Inicializar();
        }

        private void button3_Click( object sender, EventArgs e )
        {
            List<string> juegosEliminados = new List<string>();
            juegosEliminados.Add( "Juego de mentira" );

            StreamWriter sw = new StreamWriter( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "MAME_ELIMINADOS.xml" ) );
            XmlSerializer serializer = new XmlSerializer( typeof( List<string> ) );
            serializer.Serialize( sw, juegosEliminados ); 
        }
    }
}
