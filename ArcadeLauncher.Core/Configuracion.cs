using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeLauncher.Core
{
    public class Configuracion
    {
        public static string ArchivoMapeoJoystick1 { get { return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Joystick1.xml" ); } }
        public static string ArchivoMapeoJoystick2 { get { return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Joystick2.xml" ); } }
    }
}
