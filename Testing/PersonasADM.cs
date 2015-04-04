using System;
using System.Collections.Generic;

namespace Testing
{
    public class PersonasADM : CdgNetPersistenciaV3_5.ClasesBases.ADMbase
    {
        /**
         * CREATE TABLE [dbo].[PERSONAS](
	        [id] [int] IDENTITY(1,1) NOT NULL,
	        [descripcion] [varchar](50) NOT NULL,
	        [activo] [bit] NOT NULL,
         CONSTRAINT [PK_PERSONAS] PRIMARY KEY CLUSTERED 
        (
	        [id] ASC
        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
        ) ON [PRIMARY]
         * */

        private static PersonasADM __oInstancia;

        public PersonasADM(CdgNetPersistenciaV3_5.ClasesBases.ConectorBase oConexion)
            : base(oConexion, typeof(PersonaOTD))
        {
        }

        public static PersonasADM Instancia(CdgNetPersistenciaV3_5.ClasesBases.ConectorBase oConexion)
        {
            if (__oInstancia == null)
                __oInstancia = new PersonasADM(oConexion);
            return __oInstancia;
        }

        public override object[] aAgregar(CdgNetPersistenciaV3_5.ClasesBases.OTDbase oOTDbase)
        {
            return this.aAgregar<PersonaOTD>(oOTDbase);
        }

        public override object[] aActualizar(CdgNetPersistenciaV3_5.ClasesBases.OTDbase oOTDbase)
        {
            return this.aActualizar<PersonaOTD>(oOTDbase);
        }

        public override object[] aEliminar(CdgNetPersistenciaV3_5.ClasesBases.OTDbase oOTDbase)
        {
            return this.aEliminar<PersonaOTD>(oOTDbase);
        }

        public override object[] aGet_elemento(CdgNetPersistenciaV3_5.ClasesBases.OTDbase oOTDbase)
        {
            return this.aGet_elemento<PersonaOTD>(oOTDbase);
        }

        public override object[] aGet_elementos(string cFiltroWhere)
        {
            return this.aGet_elementos<PersonaOTD>(cFiltroWhere);
        }

        public override object[] aGet_elementos(string cFiltroWhere, int nCantidadRegistros)
        {
            return this.aGet_elementos<PersonaOTD>(cFiltroWhere, nCantidadRegistros);
        }

        public override object[] aGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros)
        {
            return this.aGet_elementos<PersonaOTD>(cFiltroWhere, dicParametros);
        }

        public override object[] aGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros, int nCantidadRegistros)
        {
            return this.aGet_elementos<PersonaOTD>(cFiltroWhere, dicParametros, nCantidadRegistros);
        }
    }
}
