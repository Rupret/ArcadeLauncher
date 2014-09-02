using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using ArcadeLauncher.Core;

namespace ArcadeLauncher
{
    public class Plataforma
    {
        [DllImport( "User32.dll" )]
        static extern int SetForegroundWindow( IntPtr point );

        private Process proceso;

        public event EventHandler FinalizoPrograma;
        public string Nombre { get; set; }
        public string RutaImagen { get; set; }
        public string RutaEmulador { get; set; }
        public string Parametros { get; set; }
        public string ArchivoListaDeJuegos { get; set; }
        public string RutaRoms { get; set; }
        public string UltimoJuegoEjecutado { get; set; }
        public string RutaImagenesJuegos { get; set; }
        public string RutaVideosJuegos { get; set; }
        public string ExtensionDeRom { get; set; }
        public bool IncluirRutaCompletaRom { get; set; }
        

        public List<Juego> Juegos { get { return this._juegos; } set { this._juegos = value; } }
        [NonSerializedAttribute]
        private List<Juego> _juegos;
        public List<string> JuegosEliminados { get; set; }
        public List<JuegoEstadistica> Estadisticas { get { return this._estadisticas; } set { this._estadisticas = value; } }

        [NonSerializedAttribute]
        private List<JuegoEstadistica> _estadisticas;

        public void CargarListaDeJuegos()
        {
            this.Juegos = new List<Juego>();

            this.CargarListaDeJuegosEliminados();
            this.CargarEstadisticas();

            string archivoXml = string.Format( "ListasDeJuegos\\{0}", this.ArchivoListaDeJuegos );
            XmlDocument xml = new XmlDocument();
            xml.Load( Path.Combine( Contexto.Instancia.RutaAplicacion, archivoXml ) );
            XmlNodeList nodosJuegos = xml.SelectNodes( "//game" );
            
            foreach ( XmlNode nodo in nodosJuegos )
            {
                string nombreArchivo = string.Format( "{0}.zip", nodo.Attributes[ "name" ].Value );
                string rutaRom = Path.Combine( this.RutaRoms, nombreArchivo );
                if ( File.Exists( rutaRom ) )
                {
                    Juego juego = new Juego();
                    juego.Nombre = nodo.ChildNodes[ 0 ].InnerText;
                    juego.NombreArchivo = nombreArchivo;
                    JuegoEstadistica estadisticasLocal = this._estadisticas.Find( x => x.JuegoArchivo.Equals( juego.NombreArchivo, StringComparison.InvariantCultureIgnoreCase ) );
                    if ( estadisticasLocal != null )
                        juego.Estadisticas = estadisticasLocal;

                    this.Juegos.Add( juego );
                }
            }

            this.Juegos.RemoveAll( x => this.JuegosEliminados.Exists( y => y.Equals( x.NombreArchivo, StringComparison.InvariantCultureIgnoreCase ) ) );
        }

        private void CargarEstadisticas()
        {
            string archivoEstadisticas = this.ObtenerRutaArchivoEstadisticas();

            if ( File.Exists( archivoEstadisticas ) )
                this._estadisticas = Deserializador.Deserializar<List<JuegoEstadistica>>( archivoEstadisticas );
            else
                this._estadisticas = new List<JuegoEstadistica>();

        }

        private void CargarListaDeJuegosEliminados()
        {
            string archivo = this.ObtenerRutaArchivoJuegosEliminados();
            if( File.Exists( archivo ))
                this.JuegosEliminados = Deserializador.Deserializar<List<string>>( archivo );
            else
                this.JuegosEliminados = new List<string>();
        }

        private string ObtenerRutaArchivoJuegosEliminados()
        {
            string nombreArchivo = string.Format( "{0}_ELIMINADOS{1}", Path.GetFileNameWithoutExtension( this.ArchivoListaDeJuegos ), Path.GetExtension( this.ArchivoListaDeJuegos ) );
            nombreArchivo = string.Format( "ListasDeJuegos\\{0}", nombreArchivo );
            string archivo = Path.Combine( Contexto.Instancia.RutaAplicacion, nombreArchivo );

            return archivo;
        }

        private string ObtenerRutaArchivoEstadisticas()
        {
            string nombreArchivo = string.Format( "{0}_ESTADISTICAS{1}", Path.GetFileNameWithoutExtension( this.ArchivoListaDeJuegos ), Path.GetExtension( this.ArchivoListaDeJuegos ) );
            nombreArchivo = string.Format( "ListasDeJuegos\\{0}", nombreArchivo );
            string archivo = Path.Combine( Contexto.Instancia.RutaAplicacion, nombreArchivo );

            return archivo;
        }

        public void EjecutarJuego( Juego juego )
        {
            this.ActualizarEstadisticas( juego );

            this.UltimoJuegoEjecutado = juego.NombreArchivo;
            //Contexto.Instancia.GrabarPlataforma( this );

            string parametros = this.ObtenerParametros( juego );

            Process proceso = new Process();
            this.proceso = proceso;
            proceso.StartInfo = new ProcessStartInfo( Path.GetFileName( this.RutaEmulador ), parametros );
            proceso.StartInfo.WorkingDirectory = Path.GetDirectoryName( this.RutaEmulador );
            proceso.StartInfo.CreateNoWindow = true;
            proceso.EnableRaisingEvents = true;
            proceso.Exited += proceso_Exited;
            proceso.Start();
        }

        public void CerrarEmulador()
        {
            try
            {
                if ( this.proceso != null && !this.proceso.HasExited )
                {
                    this.proceso.Kill();
                }
            }
            catch { }
        }

        private string ObtenerParametros( Juego juego )
        {
            StringBuilder parametros = new StringBuilder();
            string nombreArchivoRom = this.ObtenerNombreArchivoRom( juego );

            if( this.IncluirRutaCompletaRom )
                parametros.AppendFormat( "\"{0}\"", Path.Combine( this.RutaRoms, nombreArchivoRom ) );
            else
                parametros.Append( nombreArchivoRom );

            if ( !string.IsNullOrWhiteSpace( this.Parametros ) )
            {
                parametros.Append( " " );
                parametros.Append( this.Parametros );
            }

            return parametros.ToString();
        }

        private string ObtenerNombreArchivoRom( Juego juego )
        { 
            string retorno;
            if ( string.IsNullOrWhiteSpace( this.ExtensionDeRom ) )
                retorno = Path.GetFileNameWithoutExtension( juego.NombreArchivo );
            else
                retorno = Path.ChangeExtension( juego.NombreArchivo, this.ExtensionDeRom );

            return retorno;
        }

        private void ActualizarEstadisticas( Juego juego )
        {
            string archivoEstadisticas = this.ObtenerRutaArchivoEstadisticas();

            JuegoEstadistica juegoEstadistica = this.Estadisticas.Find( x => x.JuegoArchivo.Equals( juego.NombreArchivo, StringComparison.InvariantCultureIgnoreCase ) );
            if ( juegoEstadistica == null )
            {
                juegoEstadistica = new JuegoEstadistica();
                juegoEstadistica.JuegoArchivo = juego.NombreArchivo;
                this.Estadisticas.Add( juegoEstadistica );
            }
            juegoEstadistica.CantidadDeEjecuciones++;
            juegoEstadistica.FechaUltimaEjecucion = DateTime.Now;

            Serializador.Serializar<List<JuegoEstadistica>>( this.Estadisticas, archivoEstadisticas );
        }

        private void proceso_Exited( object sender, EventArgs e )
        {
            this.proceso.Dispose();
            this.proceso = null;
            if ( this.FinalizoPrograma != null )
                this.FinalizoPrograma( sender, e );
        }

        public void EliminarJuego( Juego juego )
        {
            this.Juegos.Remove( juego );
            string archivoEliminados = this.ObtenerRutaArchivoJuegosEliminados();
            List<string> juegosEliminados = null;
            if ( File.Exists( archivoEliminados ) )
                juegosEliminados = Deserializador.Deserializar<List<string>>( archivoEliminados );
            else
                juegosEliminados = new List<string>();

            juegosEliminados.Add( juego.NombreArchivo );
            Serializador.Serializar<List<string>>( juegosEliminados, archivoEliminados );            
        }

        public List<Juego> OrdenarPorMasJugados()
        {
            return this.Juegos.OrderByDescending( x => x.Estadisticas.CantidadDeEjecuciones ).ThenBy( x => x.Nombre ).ToList();
        }
    }
}
