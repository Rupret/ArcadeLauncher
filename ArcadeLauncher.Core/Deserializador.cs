using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ArcadeLauncher.Core
{
    public class Deserializador
    {
        public static T Deserializar<T>( string archivo )
        {
            if ( File.Exists( archivo ) )
            {
                T retorno;
                using( StreamReader sr = new StreamReader( archivo ) )
                { 
                    XmlSerializer serializador = new XmlSerializer( typeof( T ) );
                    retorno = (T) serializador.Deserialize( sr );
                }
                return retorno;
            }
            else
                throw new FileNotFoundException( string.Format( "No se encuentra el archivo {0}.", archivo ) );
        }
    }
}
