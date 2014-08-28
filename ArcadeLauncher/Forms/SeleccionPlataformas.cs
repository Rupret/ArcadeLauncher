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
using ArcadeLauncher.Configurator.Forms;
using ArcadeLauncher.Controllers;

namespace ArcadeLauncher.Forms
{
    public partial class SeleccionPlataformas : Form
    {
        private delegate void SinParametrosCallback();

        private int cantidadPlataformasEnPantalla = 6;
        private SeleccionPlataformaController controller;
        private List<PictureBox> titulosPlataformasSuperior = new List<PictureBox>();
        private List<PictureBox> titulosPlataformasInferior = new List<PictureBox>();

        public SeleccionPlataformas()
        {
            InitializeComponent();
            this.controller = new SeleccionPlataformaController( this );
            this.controller.Refrescar += controller_Refrescar;
            this.controller.Escape += controller_Escape;

            this.ImagenCentral.ImageLocation = Path.Combine( Contexto.Instancia.RutaAplicacion, this.controller.PlataformaSeleccionada.RutaImagen );

            this.LlenarPlataformasVisual();
            this.RefrescarVista();
        }

        private void controller_Escape( object sender, EventArgs e )
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.Close );
                this.Invoke( callback );
            }
            else
                this.Close();
        }

        private void controller_Refrescar( object sender, EventArgs e )
        {
            this.RefrescarVista();
        }

        private void LlenarPlataformasVisual()
        { 
            int primeraMitad = this.cantidadPlataformasEnPantalla / 2;
            for ( int i = 0; i < primeraMitad; i++ )
            {
                string nombreControl = string.Format( "Imagen{0}", i + 1 );
                this.titulosPlataformasInferior.Add( (PictureBox) this.panelTitulos.Controls[ nombreControl ] );
            }
            for ( int i = primeraMitad; i < this.cantidadPlataformasEnPantalla; i++ )
            {
                string nombreControl = string.Format( "Imagen{0}", i + 1 );
                this.titulosPlataformasSuperior.Add( (PictureBox) this.panelTitulos.Controls[ nombreControl ] );
            }
        }

        private void MainForm_KeyDown( object sender, KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.Down:
                    this.controller.MoverHaciaAbajo();
                    break;
                case Keys.Up:
                    this.controller.MoverHaciaArriba();
                    break;
                case Keys.Enter:
                    this.SeleccionarPlataforma();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F10:
                    this.ConfigurarJoysticks();
                    break;
            }

            this.RefrescarVista();
        }

        private void ConfigurarJoysticks()
        {
            this.controller.EjecutarConfiguradorJoysticks();
        }

        private void SeleccionarPlataforma()
        {
            if ( this.InvokeRequired )
            {
                SinParametrosCallback callback = new SinParametrosCallback( this.SeleccionarPlataforma );
                this.Invoke( callback );
            }
            else
                this.controller.SeleccionarPlataforma( this.controller.PlataformaSeleccionada );
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
                this.ImagenCentral.ImageLocation = this.controller.PlataformaSeleccionada.RutaImagen;
                int indiceMenor = this.controller.IndicePlataformaSeleccionada - 1;
                for ( int i = this.titulosPlataformasInferior.Count - 1; i >= 0; i-- )
                {
                    if ( indiceMenor < 0 )
                    {
                        this.titulosPlataformasInferior[ i ].Visible = false;
                        this.titulosPlataformasInferior[ i ].ImageLocation = string.Empty;
                    }
                    else
                    {
                        this.titulosPlataformasInferior[ i ].ImageLocation = this.controller.ListaDePlataformas[ indiceMenor ].RutaImagen;
                        this.titulosPlataformasInferior[ i ].Visible = true;
                    }

                    this.Refresh();
                    indiceMenor--;
                }
                int indiceMayor = this.controller.IndicePlataformaSeleccionada + 1;
                for ( int i = 0; i < this.titulosPlataformasSuperior.Count; i++ )
                {
                    if ( indiceMayor > this.controller.ListaDePlataformas.Count - 1 )
                    {
                        this.titulosPlataformasSuperior[ i ].Visible = false;
                        this.titulosPlataformasSuperior[ i ].ImageLocation = string.Empty;
                    }
                    else
                    {
                        this.titulosPlataformasSuperior[ i ].ImageLocation = this.controller.ListaDePlataformas[ indiceMayor ].RutaImagen;
                        this.titulosPlataformasSuperior[ i ].Visible = true;
                    }

                    this.Refresh();
                    indiceMayor++;
                }
            }
        }
    }
}
