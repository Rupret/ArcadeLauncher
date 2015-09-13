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
using ArcadeLauncher.Controllers;
using ArcadeLauncher.Core.Enumerables;

namespace ArcadeLauncher.Forms
{
    public partial class SeleccionJuegos : Form
    {
        private delegate void SinParametrosCallback();

        private int cantidadTitulosEnPantalla = 18;
        private SeleccionJuegosController controller;
        private List<Label> titulosJuegos = new List<Label>();
        
        private List<Label> titulosJuegosSuperior = new List<Label>();
        private List<Label> titulosJuegosInferior = new List<Label>();
        private List<char> caracteres;
        private int indiceLetraActual = 0;
        private MapeoAccionesTeclasBuscador mapeoAccionesTeclas = new MapeoAccionesTeclasBuscador();

        public SeleccionJuegos( Plataforma plataforma )
        {
            InitializeComponent();
            this.CrearFormController( plataforma );

            this.ImagenPlataforma.ImageLocation = Path.Combine( Contexto.Instancia.RutaAplicacion, this.controller.PlataformaSeleccionada.RutaImagen );

            this.InicializarCaracteres();
            this.LlenarListasDeLabels();
            this.RefrescarVista();

            if ( Screen.PrimaryScreen.Bounds.Width == 800 && Screen.PrimaryScreen.Bounds.Height == 600 )
                this.Bounds = Screen.PrimaryScreen.Bounds;
        }

        private void CrearFormController( Plataforma plataforma )
        {
            this.controller = new SeleccionJuegosController( plataforma );
            this.controller.Refrescar += controller_Refrescar;
            this.controller.ActivarBusqueda += controller_ActivarBusqueda;
            this.controller.Escape += controller_Escape;
            this.controller.BuscadorSiguiente += controller_BuscadorSiguiente;
            this.controller.BuscadorAnterior += controller_BuscadorAnterior;
            this.controller.BuscarYAvanzar += controller_BuscarYAvanzar;
            this.controller.BuscadorBorrar += controller_BuscadorBorrar;
        }

        #region eventos controller
        private void controller_BuscadorAnterior( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.BuscadorAnteriorLetra );
                this.Invoke( callback );
            }
            else
                this.BuscadorAnteriorLetra();
        }

        private void controller_BuscadorSiguiente( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.BuscadorSiguienteLetra );
                this.Invoke( callback );
            }
            else
                this.BuscadorSiguienteLetra();
        }

        private void controller_BuscadorBorrar( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.BorrarCaracter );
                this.Invoke( callback );
            }
            else
                this.BorrarCaracter();
        }

        private void controller_BuscarYAvanzar( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.BuscadorBuscarYAvanzar );
                this.Invoke( callback );
            }
            else
                this.BuscadorBuscarYAvanzar();
        }

        private void controller_Escape( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.Cerrar );
                this.Invoke( callback );
            }
            else
                this.Cerrar();
        }

        private void controller_ActivarBusqueda( object sender, EventArgs e )
        {
            this.SwitchActivarBusqueda();
        }

        private void controller_Refrescar( object sender, EventArgs e )
        {
            this.RefrescarVista();
        }

        #endregion

        private void LlenarListasDeLabels()
        { 
            int primeraMitad = this.cantidadTitulosEnPantalla / 2;
            for ( int i = 0; i < primeraMitad; i++ )
            {
                string nombreControl = string.Format( "lblTitulo{0}", i + 1 );
                this.titulosJuegosInferior.Add( (Label) this.panelTitulos.Controls[nombreControl] );
            }
            for ( int i = primeraMitad; i < this.cantidadTitulosEnPantalla; i++ )
            {
                string nombreControl = string.Format( "lblTitulo{0}", i + 1 );
                this.titulosJuegosSuperior.Add( (Label) this.panelTitulos.Controls[ nombreControl ] );
            }
        }

        private void CrearLabels()
        {
            int x = 10;
            int y = 10;
            int constanteSeparacion = 30;
            for ( int i = 0; i < this.cantidadTitulosEnPantalla; i++ )
            {
                Label nuevoTitulo = new Label();
                nuevoTitulo.Text = string.Format( "titulo {0}", i );
                nuevoTitulo.Font = new Font("Verdana", 14);
                nuevoTitulo.Location = new Point( x, y + ( i * constanteSeparacion ) );
                nuevoTitulo.AutoSize = true;

                this.titulosJuegos.Add( nuevoTitulo );
                this.Controls.Add( nuevoTitulo );
            }

            int indiceMitad = this.cantidadTitulosEnPantalla / 2;
            this.titulosJuegos[ indiceMitad ].Font = new Font( this.titulosJuegos[ indiceMitad ].Font.FontFamily, 22 );
        }

        private void MainForm_KeyDown( object sender, KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.Down:
                    this.controller.MoverHaciaAbajo();
                    this.RefrescarVista();
                    break;

                case Keys.Up:
                    this.controller.MoverHaciaArriba();
                    this.RefrescarVista();
                    break;

                case Keys.Enter:
                    //this.controller.EjecutarJuegoSeleccionado();
                    break;

                case Keys.F3:
                    this.SwitchActivarBusqueda();
                    break;

                case Keys.Escape:
                    this.Cerrar();
                    break;

                case Keys.Delete:
                    this.controller.EliminarJuegoSeleccionado();
                    break;

                case Keys.O:
                    this.controller.OrdenarPorMasJugados();
                    break;

                case Keys.C:
                    this.controller.CerrarEmulador();
                    break;
            }

        }

        private void Cerrar()
        {
            this.controller.Cerrar();
            this.Close();
        }

        private void RefrescarVista()
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.RefrescarVista );
                this.Invoke( callback );
            }
            else
            {
                if ( this.controller.JuegoSeleccionado == null )
                    this.BlanquearTitulos();
                else
                {
                    this.lblTituloCentral.Text = this.controller.JuegoSeleccionado.Nombre;
                    int indiceMenor = this.controller.IndiceJuegoSeleccionado - 1;
                    for ( int i = this.titulosJuegosInferior.Count - 1; i >= 0; i-- )
                    {
                        if ( indiceMenor < 0 )
                            this.titulosJuegosInferior[ i ].Text = string.Empty;
                        else
                            this.titulosJuegosInferior[ i ].Text = this.controller.Juegos[ indiceMenor ].Nombre;

                        this.Refresh();
                        indiceMenor--;
                    }

                    int indiceMayor = this.controller.IndiceJuegoSeleccionado + 1;
                    for ( int i = 0; i < this.titulosJuegosSuperior.Count; i++ )
                    {
                        if ( indiceMayor > this.controller.Juegos.Count - 1 )
                            this.titulosJuegosSuperior[ i ].Text = string.Empty;
                        else
                            this.titulosJuegosSuperior[ i ].Text = this.controller.Juegos[ indiceMayor ].Nombre;

                        this.Refresh();
                        indiceMayor++;
                    }
                }
            }
        }

        private void BlanquearTitulos()
        {
            this.lblResultadoBusqueda.Text = "0 resultados";
            this.lblTituloCentral.Text = "No hay juegos bajo ese filtro";
            for ( int i = this.titulosJuegosInferior.Count - 1; i >= 0; i-- )
            {
                this.titulosJuegosInferior[ i ].Text = string.Empty;
                this.Refresh();
            }
            for ( int i = 0; i < this.titulosJuegosSuperior.Count; i++ )
            {
                this.titulosJuegosSuperior[ i ].Text = string.Empty;
                this.Refresh();
            }
        }
        #region búsqueda

        private void SwitchActivarBusqueda()
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.SwitchActivarBusqueda );
                this.Invoke( callback );
            }
            else
            {
                this.txtBusqueda.Visible = !this.txtBusqueda.Visible;
                if ( this.txtBusqueda.Visible )
                {
                    this.txtBusqueda.Focus();
                    this.txtBusqueda.Text = string.Empty;
                    this.indiceLetraActual = 0;
                    this.txtBusqueda.Text = this.caracteres[ this.indiceLetraActual ].ToString();
                }
                else
                {
                    this.controller.Filtro = string.Empty;
                    this.RefrescarVista();
                    this.lblResultadoBusqueda.Visible = false;
                }
            }
        }

        private void InicializarCaracteres()
        {
            this.caracteres = new List<char>() 
                { 
                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U','V', 'W', 'X', 'Y', 'Z',
                    '0','1','2','3','4','5','6','7','8','9',' ' 
                };
        }

        private void txtBusqueda_KeyDown( object sender, KeyEventArgs e )
        {
            EnumAccionesBuscador accionBuscador = this.mapeoAccionesTeclas.ObtenerAccion( e.KeyData );
            this.Busqueda( accionBuscador );
            e.SuppressKeyPress = true;
        }

        private void Busqueda( EnumAccionesBuscador accionBuscador )
        {
            switch ( accionBuscador )
            {
                case EnumAccionesBuscador.Siguiente:
                    this.BuscadorSiguienteLetra();
                    break;
                case EnumAccionesBuscador.Anterior:
                    this.BuscadorAnteriorLetra();
                    break;
                case EnumAccionesBuscador.BuscarYAvanzar:
                    this.BuscadorBuscarYAvanzar();
                    break;
                case EnumAccionesBuscador.Borrar:
                    this.BorrarCaracter();
                    break;
                default:
                    break;
            }
        }

        private void BorrarCaracter()
        {
            if ( this.txtBusqueda.Text.Length > 1 )
            {
                this.txtBusqueda.Text = this.txtBusqueda.Text.Substring( 0, this.txtBusqueda.Text.Length - 1 );
                this.controller.Filtro = this.txtBusqueda.Text.Substring( 0, this.txtBusqueda.Text.Length - 1 );

                this.lblResultadoBusqueda.Text = string.Format( "{0} resultados", this.controller.Juegos.Count );
                this.RefrescarVista();
            }
        }

        private void BuscadorBuscarYAvanzar()
        {
            this.controller.Filtro = this.txtBusqueda.Text;
            this.RefrescarVista();
            //buscar hasta el ultimo caracter ingresado, sin el nuevo
            this.txtBusqueda.Text = this.txtBusqueda.Text + this.caracteres[ this.indiceLetraActual ].ToString();
            this.txtBusqueda.SelectionStart = this.txtBusqueda.Text.Length - 1;
            this.txtBusqueda.SelectionLength = 1;
            this.lblResultadoBusqueda.Text = string.Format( "{0} resultados", this.controller.Juegos.Count );
            this.lblResultadoBusqueda.Visible = true;
        }

        private void BuscadorAnteriorLetra()
        {
            if ( this.indiceLetraActual == 0 )
                this.indiceLetraActual = this.caracteres.Count - 1;
            else
                this.indiceLetraActual--;

            if ( string.IsNullOrEmpty( this.txtBusqueda.Text ) )
                this.txtBusqueda.Text = this.caracteres[ this.indiceLetraActual ].ToString();
            else
                this.txtBusqueda.Text = this.txtBusqueda.Text.Substring( 0, this.txtBusqueda.Text.Length - 1 ) + this.caracteres[ this.indiceLetraActual ];
        }

        private void BuscadorSiguienteLetra()
        {
            if ( this.indiceLetraActual == this.caracteres.Count - 1 )
                this.indiceLetraActual = 0;
            else
                this.indiceLetraActual++;

            if ( this.txtBusqueda.Text.Length == 0 )
                this.txtBusqueda.Text = this.caracteres[ this.indiceLetraActual ].ToString();
            else
                this.txtBusqueda.Text = this.txtBusqueda.Text.Substring( 0, this.txtBusqueda.Text.Length - 1 ) + this.caracteres[ this.indiceLetraActual ];
        }
        #endregion

    }
}
