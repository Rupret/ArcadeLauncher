using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ArcadeLauncher.Core
{
    public class Serializador
    {
        public static void Serializar<T>( T objeto, string archivo )
        {
            using ( StreamWriter sw = new StreamWriter( archivo ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( T ) );
                serializer.Serialize( sw, objeto );
            }
        }
    }
}
