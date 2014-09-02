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
        delegate void controlador_botonesPresionadosDelegado( string id, string[] botones );

        private ConfiguradorJoystickController controller;
        private TextBox textBoxAConfigurar;
        private Controlador controladorAConfigurar;

        private Dictionary<EnumAcciones, TextBox> mapeoAccionesTxtJ1 = new Dictionary<EnumAcciones, TextBox>();
        private Dictionary<EnumAccionesBuscador, TextBox> mapeoAccionesBuscadorTxtJ1 = new Dictionary<EnumAccionesBuscador, TextBox>();
        private Dictionary<EnumAcciones, TextBox> mapeoAccionesTxtJ2 = new Dictionary<EnumAcciones, TextBox>();
        private Dictionary<EnumAccionesBuscador, TextBox> mapeoAccionesBuscadorTxtJ2 = new Dictionary<EnumAccionesBuscador, TextBox>();

        public ConfiguradorJoystick()
        {
            InitializeComponent();
            this.controller = new ConfiguradorJoystickController();
            this.CompletarCombosJoystick();
            this.CargarMapeoTxtJ1();
            this.CargarMapeoTxtJ2();
            this.CargarJoystick1();
            this.CargarJoystick2();
        }

        private void CargarMapeoTxtJ1()
        {
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.Ejecutar, this.txtJ1Seleccionar );
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.Escape, this.txtJ1Escape );
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.Eliminar, this.txtJ1EliminarJuego );
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.Buscar, this.txtJ1Activar );
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.CambiarListaDeJuegos, this.txtJ1OrdenarPorUso );
            this.mapeoAccionesTxtJ1.Add( EnumAcciones.ForzarCierre, this.txtJ1ForzarCierre );

            this.mapeoAccionesBuscadorTxtJ1.Add( EnumAccionesBuscador.Borrar, this.txtJ1BorrarLetra );
            this.mapeoAccionesBuscadorTxtJ1.Add( EnumAccionesBuscador.BuscarYAvanzar, this.txtJ1LetraSiguiente );
        }

        private void CargarMapeoTxtJ2()
        {
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.Ejecutar, this.txtJ2Seleccionar );
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.Escape, this.txtJ2Escape );
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.Eliminar, this.txtJ2EliminarJuego );
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.Buscar, this.txtJ2Activar );
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.CambiarListaDeJuegos, this.txtJ2OrdenarPorUso );
            this.mapeoAccionesTxtJ2.Add( EnumAcciones.ForzarCierre, this.txtJ2ForzarCierre );

            this.mapeoAccionesBuscadorTxtJ2.Add( EnumAccionesBuscador.Borrar, this.txtJ2BorrarLetra );
            this.mapeoAccionesBuscadorTxtJ2.Add( EnumAccionesBuscador.BuscarYAvanzar, this.txtJ2LetraSiguiente );
        }

        private void CargarJoystick1()
        {
            MapeoJoystick mapeo = this.controller.CargarJoystick1();
            this.AsignarValoresATextBox( mapeo, this.mapeoAccionesTxtJ1, this.mapeoAccionesBuscadorTxtJ1 );
            Controlador controlador = this.controller.ObtenerControlador( mapeo.Id );
            if ( controlador != null )
                this.cmbJoystick1.SelectedValue = controlador.Id;
        }

        private void CargarJoystick2()
        {
            MapeoJoystick mapeo = this.controller.CargarJoystick2();
            this.AsignarValoresATextBox( mapeo, this.mapeoAccionesTxtJ2, this.mapeoAccionesBuscadorTxtJ2 );
            Controlador controlador = this.controller.ObtenerControlador( mapeo.Id );
            if ( controlador != null )
                this.cmbJoystick2.SelectedValue = controlador.Id;
        }

        private void AsignarValoresATextBox( MapeoJoystick mapeo, Dictionary<EnumAcciones, TextBox> mapeoAcciones, Dictionary<EnumAccionesBuscador, TextBox> mapeoAccionesBuscador )
        {
            foreach ( var item in mapeoAcciones )
            {
                item.Value.Text = this.ObtenerIdBoton( mapeo, item.Key );
            }
            foreach ( var item in mapeoAccionesBuscador )
            {
                item.Value.Text = this.ObtenerIdBoton( mapeo, item.Key );
            }
        }

        private string ObtenerIdBoton( MapeoJoystick mapeo, EnumAcciones accion )
        {
            ItemAccionBotonJoystick itemAccion = mapeo.AccionesSegunBoton.Find( x => x.Accion == accion );
            if ( itemAccion == null )
                return string.Empty;
            else
                return string.Join( " + ", itemAccion.Botones );
        }

        private string ObtenerIdBoton( MapeoJoystick mapeo, EnumAccionesBuscador accion )
        {
            ItemAccionBotonJoystick itemAccion = mapeo.AccionesSegunBoton.Find( x => x.AccionBuscador == accion );
            if ( itemAccion == null )
                return string.Empty;
            else
                return string.Join( " + ", itemAccion.Botones );
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
            this.controladorAConfigurar.combinacionBotones += this.controladorAConfigurar_combinacionBotones;
        }

        private void J2Botones_Click( object sender, EventArgs e )
        {
            Button botonPresionado = (Button) sender;
            this.textBoxAConfigurar = (TextBox) this.Controls[ botonPresionado.Tag.ToString() ];
            this.textBoxAConfigurar.Text = "Presionar";
            this.controladorAConfigurar = this.controller.ObtenerControlador( this.cmbJoystick2.SelectedValue.ToString() );
            this.controladorAConfigurar.combinacionBotones += this.controladorAConfigurar_combinacionBotones;
        }

        private void controladorAConfigurar_combinacionBotones( string id, string[] botones )
        {
            if ( this.InvokeRequired )
            {
                controlador_botonesPresionadosDelegado delegado = new controlador_botonesPresionadosDelegado( this.controladorAConfigurar_combinacionBotones );
                this.Invoke( delegado, id, botones );
            }
            else
            {
                this.textBoxAConfigurar.Text = string.Join( " + ", botones );
                this.controladorAConfigurar.combinacionBotones -= this.controladorAConfigurar_combinacionBotones;
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

            this.CargarMapeo( ref mapeo, this.mapeoAccionesTxtJ1, this.mapeoAccionesBuscadorTxtJ1 );

            return mapeo;
        }

        private MapeoJoystick ObtenerMapeoJoystick2()
        {
            MapeoJoystick mapeo = new MapeoJoystick();
            mapeo.Id = ( (Guid) this.cmbJoystick2.SelectedValue ).ToString();
            this.CargarMapeo( ref mapeo, this.mapeoAccionesTxtJ2, this.mapeoAccionesBuscadorTxtJ2 );

            return mapeo;
        }
        private void CargarMapeo( ref MapeoJoystick mapeo, Dictionary<EnumAcciones, TextBox> mapeoAcciones, Dictionary<EnumAccionesBuscador, TextBox> mapeoAccionesBuscador )
        {
            foreach ( var item in mapeoAcciones )
            {
                ItemAccionBotonJoystick itemAcciones = new ItemAccionBotonJoystick( item.Value.Text, item.Key );
                mapeo.AccionesSegunBoton.Add( itemAcciones );
            }
            foreach ( var item in mapeoAccionesBuscador )
            {
                ItemAccionBotonJoystick itemAcciones = new ItemAccionBotonJoystick( item.Value.Text, item.Key );
                mapeo.AccionesSegunBoton.Add( itemAcciones );
            }
        }

        private void btnCancelar_Click( object sender, EventArgs e )
        {
            this.Close();
        }
    }
}
