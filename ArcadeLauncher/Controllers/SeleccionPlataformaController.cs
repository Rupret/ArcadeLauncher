using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcadeLauncher.Configurator.Forms;
using ArcadeLauncher.Core;
using ArcadeLauncher.Core.Enumerables;
using ArcadeLauncher.Forms;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher.Controllers
{
    public class SeleccionPlataformaController
    {
        private delegate void ParametrosPlataformaCallback( Plataforma plataforma );
        public event EventHandler Refrescar;
        public event EventHandler Escape;
        public List<Plataforma> ListaDePlataformas { get; set; }        

        public Plataforma PlataformaSeleccionada { get { return this.ListaDePlataformas[ this.IndicePlataformaSeleccionada ]; } }

        public int IndicePlataformaSeleccionada { get; private set; }
        public Form Owner { get; set; }

        public SeleccionPlataformaController( Form owner )
        {
            this.IndicePlataformaSeleccionada = 0;
            this.IniciarControladores( Contexto.Instancia.Controladores );
            this.ListaDePlataformas = Contexto.Instancia.Plataformas;
            this.Owner = owner;
        }

        private void IniciarControladores( Controladores controladores )
        {
            if ( Contexto.Instancia.Controladores.Controlador1 != null )
                this.AsignarEventosControlador( controladores.Controlador1, controladores.MapeoJoystick1 );

            if ( Contexto.Instancia.Controladores.Controlador2 != null )
                this.AsignarEventosControlador( controladores.Controlador2, controladores.MapeoJoystick2 );
            
        }

        private void DetenerControladores( Controladores controladores )
        {
            if ( Contexto.Instancia.Controladores.Controlador1 != null )
                this.DetenerEventosControlador( Contexto.Instancia.Controladores.Controlador1 );

            if ( Contexto.Instancia.Controladores.Controlador2 != null )
                this.DetenerEventosControlador( Contexto.Instancia.Controladores.Controlador2 );
        }

        private void DetenerEventosControlador( Controlador controlador )
        {
            controlador.movimientoJoystick_Abajo -= MoverHaciaAbajo;
            controlador.movimientoJoystick_Arriba -= MoverHaciaArriba;
            controlador.combinacionBotones -= controlador_combinacionBotones;
        }

        private void AsignarEventosControlador( Controlador controlador, MapeoJoystick mapeo )
        {
            controlador.movimientoJoystick_Abajo += MoverHaciaAbajo;
            controlador.movimientoJoystick_Arriba += MoverHaciaArriba;
            controlador.combinacionBotones += controlador_combinacionBotones;
        }

        private void controlador_combinacionBotones( string id, string[] botones )
        {
            MapeoJoystick mapeo = Contexto.Instancia.Controladores.ObtenerMapeoPorId( id );
            EnumAcciones accion = mapeo.ObtenerAccionDeBoton( botones );
            switch ( accion )
            {
                case EnumAcciones.Nada:
                    break;
                case EnumAcciones.Ejecutar:
                    this.SeleccionarPlataforma( this.PlataformaSeleccionada );
                    break;
                case EnumAcciones.Buscar:
                    break;
                case EnumAcciones.PageUp:
                    break;
                case EnumAcciones.PageDown:
                    break;
                case EnumAcciones.Escape:
                    if ( this.Escape != null )
                        this.Escape( this, null );
                    break;
                default:
                    break;
            }
        }

        private void MoverHaciaAbajo( object sender, EventArgs e )
        {
            this.MoverHaciaAbajo();
        }

        private void MoverHaciaArriba( object sender, EventArgs e )
        {
            this.MoverHaciaArriba();
        }

        public List<Plataforma> ObtenerPlataformas()
        {
            return Contexto.Instancia.Plataformas;
        }

        public void SeleccionarPlataforma( Plataforma plataforma )
        {
            if ( this.Owner.InvokeRequired )
            {
                ParametrosPlataformaCallback callback = new ParametrosPlataformaCallback( SeleccionarPlataforma );
                this.Owner.Invoke( callback, plataforma );
            }
            else
            {
                this.DetenerControladores( Contexto.Instancia.Controladores );
                SeleccionJuegos formJuegos = new SeleccionJuegos( plataforma );
                formJuegos.Show( this.Owner );
                formJuegos.FormClosed += formJuegos_FormClosed;
            }            
        }

        private void formJuegos_FormClosed( object sender, FormClosedEventArgs e )
        {
            this.IniciarControladores( Contexto.Instancia.Controladores );
            this.Owner.Focus();
        }

        public void MoverHaciaArriba()
        {
            if ( this.IndicePlataformaSeleccionada > 0 )
                this.IndicePlataformaSeleccionada--;

            if ( this.Refrescar != null )
                this.Refrescar( this, null );
        }

        public void MoverHaciaAbajo()
        {
            if ( this.IndicePlataformaSeleccionada < ( Contexto.Instancia.Plataformas.Count - 1 ) )
                this.IndicePlataformaSeleccionada++;

            if ( this.Refrescar != null )
                this.Refrescar( this, null );
        }

        public void EjecutarConfiguradorJoysticks()
        {
            this.DetenerControladores( Contexto.Instancia.Controladores );
            ConfiguradorJoystick configuradorJoystick = new ConfiguradorJoystick();
            configuradorJoystick.ShowDialog();
            
            Contexto.Instancia.ReiniciarControladores();
            this.IniciarControladores( Contexto.Instancia.Controladores );
        }
    }
}
