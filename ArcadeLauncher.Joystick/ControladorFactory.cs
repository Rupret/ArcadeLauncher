using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeLauncher.Joystick
{
    public class ControladorFactory
    {
        public static List<Controlador> ObtenerControladoresDisponibles()
        {
            JoystickReader jr = new JoystickReader();
            jr.Inicializar();
            var controladores = jr.ObtenerJoysticks();

            return controladores;
        }
    }
}
