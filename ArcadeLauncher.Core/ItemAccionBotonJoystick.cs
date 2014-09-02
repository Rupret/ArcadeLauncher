﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ArcadeLauncher.Core.Enumerables;

namespace ArcadeLauncher.Core
{
    public class ItemAccionBotonJoystick
    {
        public string IdBoton { get; set; }
        public string[] Botones { get; set; }
        public EnumAcciones Accion { get; set; }
        public EnumAccionesBuscador AccionBuscador { get; set; }

        public ItemAccionBotonJoystick()
        {
            this.Botones = new string[] {};
        }

        public ItemAccionBotonJoystick( string botones, EnumAcciones accion )
        {

            this.Botones = botones.Split( new string[] { " + " }, StringSplitOptions.None );
            this.Accion = accion;
        }

        public ItemAccionBotonJoystick( string botones, EnumAccionesBuscador accionBuscador )
        {
            this.Botones = botones.Split( new string[] { " + " }, StringSplitOptions.None );
            this.AccionBuscador = accionBuscador;
        }
    }
}
