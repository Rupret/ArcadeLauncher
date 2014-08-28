using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcadeLauncher.Configurator.Controllers;
using ArcadeLauncher.Core;
using ArcadeLauncher.Core.Enumerables;
using ArcadeLauncher.Joystick;

namespace ArcadeLauncher.Configurator.Forms
{
    public partial class ConfiguradorJoystick : Form
    {
        delegate void controlador_botonPresionadoDelegado( string id, string boton );

        private ConfiguradorJoystickController controller;
        private TextBox textBoxAConfigurar;
        private Controlador controladorAConfigurar;

        public ConfiguradorJoystick()
        {
            InitializeComponent();
            this.controller = new ConfiguradorJoystickController();
            this.CompletarCombosJoystick();
            this.CargarJoystick1();
            this.CargarJoystick2();
        }

        private void CargarJoystick1()
        {
            MapeoJoystick mapeo = this.controller.CargarJoystick1();
            this.txtJ1Seleccionar.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Ejecutar );
            this.txtJ1Escape.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Escape );
            this.txtJ1EliminarJuego.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Eliminar );
            this.txtJ1Activar.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Buscar );
            this.txtJ1OrdenarPorUso.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.CambiarListaDeJuegos );
            this.txtJ1BorrarLetra.Text = this.ObtenerIdBoton( mapeo, EnumAccionesBuscador.Borrar );
            this.txtJ1LetraSiguiente.Text = this.ObtenerIdBoton( mapeo, EnumAccionesBuscador.BuscarYAvanzar );
            Controlador controlador = this.controller.ObtenerControlador( mapeo.Id );
            if ( controlador != null )
                this.cmbJoystick1.SelectedValue = controlador.Id;
        }

        private void CargarJoystick2()
        {
            MapeoJoystick mapeo = this.controller.CargarJoystick2();
            this.txtJ2Seleccionar.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Ejecutar );
            this.txtJ2Escape.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Escape );
            this.txtJ2EliminarJuego.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Eliminar );
            this.txtJ2Activar.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.Buscar );
            this.txtJ2OrdenarPorUso.Text = this.ObtenerIdBoton( mapeo, EnumAcciones.CambiarListaDeJuegos );
            this.txtJ2BorrarLetra.Text = this.ObtenerIdBoton( mapeo, EnumAccionesBuscador.Borrar );
            this.txtJ2LetraSiguiente.Text = this.ObtenerIdBoton( mapeo, EnumAccionesBuscador.BuscarYAvanzar );
            Controlador controlador = this.controller.ObtenerControlador( mapeo.Id );
            if ( controlador != null )
                this.cmbJoystick2.SelectedValue = controlador.Id;
        }


        private string ObtenerIdBoton( MapeoJoystick mapeo, EnumAcciones accion )
        {
            ItemAccionBotonJoystick itemAccion = mapeo.AccionesSegunBoton.Find( x => x.Accion == accion );
            if ( itemAccion == null )
                return string.Empty;
            else
                return itemAccion.IdBoton;
        }

        private string ObtenerIdBoton( MapeoJoystick mapeo, EnumAccionesBuscador accion )
        {
            ItemAccionBotonJoystick itemAccion = mapeo.AccionesSegunBoton.Find( x => x.AccionBuscador == accion );
            if ( itemAccion == null )
                return string.Empty;
            else
                return itemAccion.IdBoton;
        }

        private void CompletarCombosJoystick()
        {
            BindingList<Controlador> bindingListJ1 = new BindingList<Controlador>( this.controller.Controladores );
            this.cmbJoystick1.DataSource = bindingListJ1;
            this.cmbJoystick1.DisplayMember = "ID";
            this.cmbJoystick1.ValueMember = "ID";

            BindingList<Controlador> bindingListJ2 = new BindingList<Controlador>( this.controller.Controladores );
            this.cmbJoystick2.DataSource = bindingListJ2;
            this.cmbJoystick2.DisplayMember = "ID";
            this.cmbJoystick2.ValueMember = "ID";
        }

        private void J1Botones_Click( object sender, EventArgs e )
        {
            Button botonPresionado = (Button) sender;
            this.textBoxAConfigurar = (TextBox) this.Controls[ botonPresionado.Tag.ToString() ];
            this.textBoxAConfigurar.Text = "Presionar";
            this.controladorAConfigurar = this.controller.ObtenerControlador( this.cmbJoystick1.SelectedValue.ToString() );
            this.controladorAConfigurar.botonPresionado += this.controlador_botonPresionado;
        }

        private void J2Botones_Click( object sender, EventArgs e )
        {
            Button botonPresionado = (Button) sender;
            this.textBoxAConfigurar = (TextBox) this.Controls[ botonPresionado.Tag.ToString() ];
            this.textBoxAConfigurar.Text = "Presionar";
            this.controladorAConfigurar = this.controller.ObtenerControlador( this.cmbJoystick2.SelectedValue.ToString() );
            this.controladorAConfigurar.botonPresionado += this.controlador_botonPresionado;
        }

        private void controlador_botonPresionado( string id, string boton )
        {
            if ( this.InvokeRequired )
            {
                controlador_botonPresionadoDelegado delegado = new controlador_botonPresionadoDelegado( this.controlador_botonPresionado );
                this.Invoke( delegado, id, boton );
            }
            else
            {
                this.textBoxAConfigurar.Text = boton;
                this.controladorAConfigurar.botonPresionado -= this.controlador_botonPresionado;
            }
        }

        private void btnAceptar_Click( object sender, EventArgs e )
        {
            MapeoJoystick mapeoJ1 = this.ObtenerMapeoJoystick1();
            this.controller.GrabarJoystick1( mapeoJ1 );

            MapeoJoystick mapeoJ2 = this.ObtenerMapeoJoystick2();
            this.controller.GrabarJoystick2( mapeoJ2 );

            this.Close();
        }

        private MapeoJoystick ObtenerMapeoJoystick1()
        {
            MapeoJoystick mapeo = new MapeoJoystick();
            mapeo.Id = ( (Guid) this.cmbJoystick1.SelectedValue ).ToString();
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Ejecutar, IdBoton = this.txtJ1Seleccionar.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Escape, IdBoton = this.txtJ1Escape.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Buscar, IdBoton = this.txtJ1Activar.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Eliminar, IdBoton = this.txtJ1EliminarJuego.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.CambiarListaDeJuegos, IdBoton = this.txtJ1OrdenarPorUso.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { AccionBuscador = EnumAccionesBuscador.Borrar, IdBoton = this.txtJ1BorrarLetra.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { AccionBuscador = EnumAccionesBuscador.BuscarYAvanzar, IdBoton = this.txtJ1LetraSiguiente.Text } );

            return mapeo;
        }
        
        private MapeoJoystick ObtenerMapeoJoystick2()
        {
            MapeoJoystick mapeo = new MapeoJoystick();
            mapeo.Id = ( (Guid) this.cmbJoystick2.SelectedValue ).ToString();
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Ejecutar, IdBoton = this.txtJ2Seleccionar.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Escape, IdBoton = this.txtJ2Escape.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Buscar, IdBoton = this.txtJ2Activar.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.Eliminar, IdBoton = this.txtJ2EliminarJuego.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { Accion = EnumAcciones.CambiarListaDeJuegos, IdBoton = this.txtJ2OrdenarPorUso.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { AccionBuscador = EnumAccionesBuscador.Borrar, IdBoton = this.txtJ2BorrarLetra.Text } );
            mapeo.AccionesSegunBoton.Add( new ItemAccionBotonJoystick() { AccionBuscador = EnumAccionesBuscador.BuscarYAvanzar, IdBoton = this.txtJ2LetraSiguiente.Text } );

            return mapeo;
        }

        private void btnCancelar_Click( object sender, EventArgs e )
        {
            this.Close();
        }
    }
}
