using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using SharpDX;
using System.Windows.Forms;

namespace ArcadeLauncher.Joystick
{
    public class Controlador
    {
        public delegate void BotonPresionadoDelegate( string id, string boton );
        public delegate void CombinacionBotonesDelegate( string id, string[] botones );
        //private int cantidadDeBotones;

        private SharpDX.DirectInput.Joystick joystick;
        public event EventHandler movimientoJoystick_Arriba;
        public event EventHandler movimientoJoystick_Abajo;
        public event EventHandler movimientoJoystick_Izquierda;
        public event EventHandler movimientoJoystick_Derecha;
        public event BotonPresionadoDelegate botonPresionado;
        public event CombinacionBotonesDelegate combinacionBotones;
        private Timer timerLectura;

        public Guid Id { get { return this.joystick.Information.InstanceGuid; } }
        public string Nombre { get { return this.joystick.Information.ProductName; } }
        public bool Inicializado { get; set; }

        public Controlador( SharpDX.DirectInput.Joystick joystick )
        {
            this.joystick = joystick;
            this.Inicializado = false;
        }

        public void Inicializar()
        {
            if ( !this.Inicializado )
            {
                this.joystick.Properties.BufferSize = 128;
                this.joystick.Acquire();

                this.timerLectura = new Timer();
                timerLectura.Interval = 100;
                timerLectura.Tick += timerLectura_Tick;
                timerLectura.Start();
                this.Inicializado = true;
            }
        }

        void timerLectura_Tick( object sender, EventArgs e )
        {
            this.Leer();
        }

        private void Leer()
        {
            this.joystick.Poll();

            JoystickUpdate[] datas = joystick.GetBufferedData();

            if ( datas.Length > 0 )
            {
                List<JoystickUpdate> botonesPresionados = datas.ToList().FindAll( x => x.Value == 128 &&
                    x.Offset.ToString().StartsWith( "buttons", StringComparison.InvariantCultureIgnoreCase ) );

                if ( botonesPresionados.Count == 1 )
                {
                    if ( this.botonPresionado != null )
                        this.botonPresionado( this.Id.ToString(), botonesPresionados[ 0 ].Offset.ToString() );
                }
                else if ( botonesPresionados.Count > 1 )
                {
                    if ( this.combinacionBotones != null )
                    {
                        string[] botonesOffset = botonesPresionados.Select( x => x.Offset.ToString() ).ToArray();
                        this.combinacionBotones( this.Id.ToString(), botonesOffset );
                    }
                }
                else
                {
                    JoystickUpdate flechaPresionadas = datas.FirstOrDefault( x => x.Offset.ToString().StartsWith( "PointOfViewControllers", StringComparison.InvariantCultureIgnoreCase ) );
                    switch ( flechaPresionadas.Value )
                        {
                            case 0:
                                if ( this.movimientoJoystick_Arriba != null )
                                    this.movimientoJoystick_Arriba( this, null );
                                break;
                            case 9000:
                                if ( this.movimientoJoystick_Derecha != null )
                                    this.movimientoJoystick_Derecha( this, null );
                                break;
                            case 18000:
                                if ( this.movimientoJoystick_Abajo != null )
                                    this.movimientoJoystick_Abajo( this, null );
                                break;
                            case 27000:
                                if ( this.movimientoJoystick_Izquierda != null )
                                    this.movimientoJoystick_Izquierda( this, null );
                                break;
                            default:
                                break;
                        }
                }
            }
                //foreach ( JoystickUpdate item in datas )
                //{
                //    //if ( item.Offset.ToString().StartsWith( "buttons", StringComparison.InvariantCultureIgnoreCase ) && 
                //    //    this.botonPresionado != null && item.Value == 128 )
                //    //{
                //    //    this.botonPresionado( this.Id.ToString(), item.Offset.ToString() );
                //    //    JoystickUpdate otroBoton = datas.FirstOrDefault( x => x.Value == 128 && x.Offset != item.Offset );
                //    //}
                //    //else if ( item.Offset.ToString().StartsWith( "PointOfViewControllers", StringComparison.InvariantCultureIgnoreCase ) )
                //    if ( item.Offset.ToString().StartsWith( "PointOfViewControllers", StringComparison.InvariantCultureIgnoreCase ) )
                //    {
                //        switch ( item.Value )
                //        {
                //            case 0:
                //                if ( this.movimientoJoystick_Arriba != null )
                //                    this.movimientoJoystick_Arriba( this, null );
                //                break;
                //            case 9000:
                //                if ( this.movimientoJoystick_Derecha != null )
                //                    this.movimientoJoystick_Derecha( this, null );
                //                break;
                //            case 18000:
                //                if ( this.movimientoJoystick_Abajo != null )
                //                    this.movimientoJoystick_Abajo( this, null );
                //                break;
                //            case 27000:
                //                if ( this.movimientoJoystick_Izquierda != null )
                //                    this.movimientoJoystick_Izquierda( this, null );
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}
        }
    }
}
