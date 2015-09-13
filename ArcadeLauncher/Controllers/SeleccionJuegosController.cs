using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArcadeLauncher.Core;
using ArcadeLauncher.Core.Enumerables;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher.Controllers
{
    public class SeleccionJuegosController
    {
        public event EventHandler Refrescar;
        public event EventHandler ActivarBusqueda;
        public event EventHandler Escape;
        public event EventHandler Eliminar;
        public event EventHandler BuscadorSiguiente;
        public event EventHandler BuscadorAnterior;
        public event EventHandler BuscarYAvanzar;
        public event EventHandler BuscadorBorrar;

        public Plataforma PlataformaSeleccionada { get; set; }
        private List<Juego> juegos;

        public List<Juego> Juegos
        {
            get { return this.juegos;  }
        }

        public Juego JuegoSeleccionado
        {
            get
            {
                if ( this.juegos.Count > 0 )
                    return this.juegos[ IndiceJuegoSeleccionado ];
                else
                    return null;
            }
        }
        public int IndiceJuegoSeleccionado { get; private set; }
        public string Filtro {
            set 
            {
                if ( string.IsNullOrEmpty( value ) )
                    this.juegos = this.PlataformaSeleccionada.Juegos;
                else
                {
                    this.juegos = this.PlataformaSeleccionada.Juegos.FindAll( x => x.Nombre.ToUpper().Contains( value.ToUpper() ) );
                    this.IndiceJuegoSeleccionado = 0;
                }
            }
        }

        public SeleccionJuegosController( Plataforma plataforma )
        {
            this.PlataformaSeleccionada = plataforma;
            this.PlataformaSeleccionada.FinalizoPrograma += PlataformaSeleccionada_FinalizoPrograma;
            this.PlataformaSeleccionada.CargarListaDeJuegos();
            this.juegos = this.PlataformaSeleccionada.Juegos;

            this.IndiceJuegoSeleccionado = this.juegos.FindIndex( x => x.NombreArchivo.Equals( this.PlataformaSeleccionada.UltimoJuegoEjecutado, StringComparison.InvariantCultureIgnoreCase ) );
            if( this.IndiceJuegoSeleccionado == -1 )
                this.IndiceJuegoSeleccionado = 0;

            this.IniciarControladores( Contexto.Instancia.Controladores );
        }

        private void IniciarControladores( Controladores controladores )
        {
            if ( Contexto.Instancia.Controladores.Controlador1 != null )
            {
                this.DetenerEventosControlador( controladores.Controlador1 );
                this.AsignarEventosControlador( controladores.Controlador1 );
            }
            if ( Contexto.Instancia.Controladores.Controlador2 != null )
            {
                this.DetenerEventosControlador( controladores.Controlador2 );
                this.AsignarEventosControlador( controladores.Controlador2 );
            }
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
            controlador.movimientoJoystick_Derecha -= controlador_movimientoJoystick_Derecha;
            controlador.movimientoJoystick_Izquierda -= controlador_movimientoJoystick_Izquierda;
            controlador.combinacionBotones -= controlador_combinacionBotones;
        }

        private void AsignarEventosControlador( Controlador controlador )
        {            
            controlador.movimientoJoystick_Abajo += MoverHaciaAbajo;
            controlador.movimientoJoystick_Arriba += MoverHaciaArriba;
            controlador.movimientoJoystick_Derecha += controlador_movimientoJoystick_Derecha;
            controlador.movimientoJoystick_Izquierda += controlador_movimientoJoystick_Izquierda;
            controlador.combinacionBotones += controlador_combinacionBotones;
        }

        private void MoverHaciaArriba( object sender, EventArgs e )
        {
            this.MoverHaciaArriba();
        }
        private void MoverHaciaAbajo( object sender, EventArgs e )
        {
            this.MoverHaciaAbajo();
        }
        private void controlador_movimientoJoystick_Derecha( object sender, EventArgs e )
        {
            this.ProcesarAccionBuscadorControlador( EnumAccionesBuscador.Siguiente );
        }
        private void controlador_movimientoJoystick_Izquierda( object sender, EventArgs e )
        {
            this.ProcesarAccionBuscadorControlador( EnumAccionesBuscador.Anterior );
        }

        public void MoverHaciaArriba()
        {
            if ( this.IndiceJuegoSeleccionado > 0 )
                this.IndiceJuegoSeleccionado--;

            if ( this.Refrescar != null )
                this.Refrescar( this, null );
        }

        public void MoverHaciaAbajo()
        {
            if ( this.IndiceJuegoSeleccionado < ( this.juegos.Count - 1 ) )
                this.IndiceJuegoSeleccionado++;

            if ( this.Refrescar != null )
                this.Refrescar( this, null );
        }

        private void controlador_combinacionBotones( string id, string[] botones )
        {
            MapeoJoystick mapeo = Contexto.Instancia.Controladores.ObtenerMapeoPorId( id );
            ItemAccionBotonJoystick itemAcciones = mapeo.ObtenerItemAccionesDeBoton( botones );

            this.ProcesarAccionControlador( itemAcciones.Accion );
            this.ProcesarAccionBuscadorControlador( itemAcciones.AccionBuscador );
        }

        private void controlador_combinacionBotonesSoloForzarCierre( string id, string[] botones )
        {
            MapeoJoystick mapeo = Contexto.Instancia.Controladores.ObtenerMapeoPorId( id );
            ItemAccionBotonJoystick itemAcciones = mapeo.ObtenerItemAccionesDeBoton( botones );

            if ( itemAcciones.Accion == EnumAcciones.ForzarCierre )
                this.CerrarEmulador();
        }

        private void ProcesarAccionControlador( EnumAcciones accion )
        {
            switch ( accion )
            {
                case EnumAcciones.Nada:
                    break;
                case EnumAcciones.Ejecutar:
                    this.EjecutarJuegoSeleccionado();
                    break;
                case EnumAcciones.Buscar:
                    if ( this.ActivarBusqueda != null )
                        this.ActivarBusqueda( this, null );
                    break;
                case EnumAcciones.PageUp:
                    break;
                case EnumAcciones.PageDown:
                    break;
                case EnumAcciones.Escape:
                    if ( this.Escape != null )
                        this.Escape( this, null );
                    break;
                case EnumAcciones.Eliminar:
                    this.EliminarJuegoSeleccionado();
                    break;
                case EnumAcciones.CambiarListaDeJuegos:
                    this.OrdenarPorMasJugados();
                    break;
                case EnumAcciones.ForzarCierre:
                    this.CerrarEmulador();
                    break;
                default:
                    break;
            }
        }

        public void OrdenarPorMasJugados()
        {
            this.juegos = this.PlataformaSeleccionada.OrdenarPorMasJugados();
            this.IndiceJuegoSeleccionado = 0;
            if ( this.Refrescar != null )
                this.Refrescar( this, null );
        }

        public void EliminarJuegoSeleccionado()
        {
            this.PlataformaSeleccionada.EliminarJuego( this.JuegoSeleccionado );
            this.juegos = this.PlataformaSeleccionada.Juegos;
            this.MoverHaciaArriba();
            if ( this.Eliminar != null )
                this.Eliminar( this, null );
        }

        private void ProcesarAccionBuscadorControlador( EnumAccionesBuscador accionBuscador )
        {
            switch ( accionBuscador )
            {
                case EnumAccionesBuscador.NULA:
                    break;
                case EnumAccionesBuscador.Siguiente:
                    if ( this.BuscadorSiguiente != null )
                        this.BuscadorSiguiente( this, null );
                    break;
                case EnumAccionesBuscador.Anterior:
                    if ( this.BuscadorAnterior != null )
                        this.BuscadorAnterior( this, null );
                    break;
                case EnumAccionesBuscador.BuscarYAvanzar:
                    if ( this.BuscarYAvanzar != null )
                        this.BuscarYAvanzar( this, null );
                    break;
                case EnumAccionesBuscador.Borrar:
                    if ( this.BuscadorBorrar != null )
                        this.BuscadorBorrar( this, null );
                    break;
                default:
                    break;
            }
        }

        public void EjecutarJuegoSeleccionado()
        {
            this.DetenerControladores( Contexto.Instancia.Controladores );
            if ( Contexto.Instancia.Controladores.Controlador1 != null )
                Contexto.Instancia.Controladores.Controlador1.combinacionBotones += controlador_combinacionBotonesSoloForzarCierre;

            if ( Contexto.Instancia.Controladores.Controlador2 != null )
                Contexto.Instancia.Controladores.Controlador2.combinacionBotones += controlador_combinacionBotonesSoloForzarCierre;
            
            this.PlataformaSeleccionada.EjecutarJuego( this.JuegoSeleccionado );
        }

        private void PlataformaSeleccionada_FinalizoPrograma( object sender, EventArgs e )
        {
            if ( Contexto.Instancia.Controladores.Controlador1 != null )
                Contexto.Instancia.Controladores.Controlador1.combinacionBotones -= controlador_combinacionBotonesSoloForzarCierre;

            if ( Contexto.Instancia.Controladores.Controlador2 != null )
                Contexto.Instancia.Controladores.Controlador2.combinacionBotones -= controlador_combinacionBotonesSoloForzarCierre;

            this.IniciarControladores( Contexto.Instancia.Controladores );
        }

        public void Cerrar()
        {
            this.DetenerControladores( Contexto.Instancia.Controladores );
        }

        public void CerrarEmulador()
        {
            this.PlataformaSeleccionada.CerrarEmulador();
        }
    }
}
