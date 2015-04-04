using System;
using System.Collections.Generic;

namespace Testing
{
    [CdgNetPersistenciaV3_5.Atributos.Tabla("PERSONAS", CdgNetPersistenciaV3_5.Atributos.Tabla.SGBD.SQL_SERVER)]
    public class PersonaOTD : CdgNetPersistenciaV3_5.ClasesBases.OTDbase
    {
        /**
         * Comandos de creacion de la tabla
         * -- SQL SERVER
         * CREATE TABLE [dbo].[PERSONAS](
	            [id] [int] IDENTITY(1,1) NOT NULL,
	            [descripcion] [varchar](50) NOT NULL,
	            [activo] [bit] NOT NULL,
             CONSTRAINT [PK_PERSONAS] PRIMARY KEY CLUSTERED 
            (
	            [id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY];
         * 
         * -- MySQL
         * CREATE TABLE `personas` (
	            `id` INT(11) NOT NULL AUTO_INCREMENT,
	            `descripcion` VARCHAR(50) NOT NULL,
	            `activo` BIT(1) NOT NULL,
	            PRIMARY KEY (`id`)
            )
            COMMENT='tabla de testing de utileria de conexion'
            COLLATE='latin1_swedish_ci'
            ENGINE=InnoDB
            AUTO_INCREMENT=5001
            ;
         * */


        public PersonaOTD()
            : base()
        {
        }

        public PersonaOTD(long nId, string cNombreUsuario)
            : base(nId, cNombreUsuario)
        {
        }

        [CdgNetPersistenciaV3_5.Atributos.Campo("ID", Identificador = true, AutoGenerado = true)]
        public override long Id
        {
            get { return _nId; }
            set { _nId = value; }
        }

        [CdgNetPersistenciaV3_5.Atributos.Campo("DESCRIPCION", Nulable = false)]
        public override string Descripcion
        {
            get { return _cDescripcion; }
            set { _cDescripcion = value; }
        }

        [CdgNetPersistenciaV3_5.Atributos.Campo("ACTIVO")]
        public Boolean Activo
        {
            get;
            set;
        }

    }
}
