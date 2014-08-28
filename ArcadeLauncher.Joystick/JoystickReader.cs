using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX;

namespace ArcadeLauncher.Joystick
{
    public class JoystickReader
    {
        private DirectInput directInput;

        public void Inicializar()
        {
            this.directInput = new DirectInput();
        }

        public List<Controlador> ObtenerJoysticks()
        {
            List<DeviceInstance> deviceInstances = new List<DeviceInstance>();

            deviceInstances.AddRange( directInput.GetDevices( DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices ) );
            deviceInstances.AddRange( directInput.GetDevices( DeviceType.Joystick, DeviceEnumerationFlags.AllDevices ) );

            List<Controlador> joysticks = new List<Controlador>();
            foreach ( DeviceInstance item in deviceInstances )
            {
                var joystick = new SharpDX.DirectInput.Joystick( this.directInput, item.InstanceGuid );
                joysticks.Add( new Controlador( joystick ) );
            }

            return joysticks;
        }
    }
}
