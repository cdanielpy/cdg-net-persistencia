package components.control.central.fechaSinAnalisis
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.fechaSinAnalisis.CentralFechaSinAnalisisCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.fechaSinAnalisis.FechaSinAnalisisDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;
	
	public class FechaSinAnalisisCrudCommand implements ICommand, IResponder
	{
		private var modelo:ModelLocator = ModelLocator.getInstance();
		
		public var evento:CentralFechaSinAnalisisCrudEvent; 
		
		//metodo de ejecucion de procesos de comando
		
		public function execute(event:CairngormEvent):void
		{
			//tomamos una copia del evento
			this.evento= event as CentralFechaSinAnalisisCrudEvent;
			
			//creamos una instancia del delegado
			var delegate:BusinessDelegate= new BusinessDelegate(this);
			
			//se evalua la accion que selecciono
			switch(this.evento.accion){
				
				case CentralFechaSinAnalisisCrudEvent.AGREGAR:
					delegate.upsertFechaSinAnalisis(modelo.centralModel.fechaSinAnalisisInsertando);
					break;
				
				case CentralFechaSinAnalisisCrudEvent.EDITAR:
					delegate.upsertFechaSinAnalisis(modelo.centralModel.fechaSinAnalisisEditando);
					break;
				
				default:
					delegate.getListaFechaSinAnalisis();
					break;
			}
		
		}
		
		//manejador de resultado exitoso
		public function result(data:Object):void
		{
			var resultado:ResultEvent= data as ResultEvent;
			
			switch (this.evento.accion)
			{
				case CentralFechaSinAnalisisCrudEvent.AGREGAR:
					this.resultadoUpsert(resultado.result as FechaSinAnalisisDto);
					break;
				case CentralFechaSinAnalisisCrudEvent.EDITAR:
					this.resultadoUpsert(resultado.result as FechaSinAnalisisDto);
					break;
				default:
					this.listarFechaSinAnalisis(resultado.result as ArrayCollection);
					break;
			}
		}
		//metodo manejador de fecha sin analisis
		public function listarFechaSinAnalisis(acFechaAnalisis:ArrayCollection):void
		{
			modelo.centralModel.acFechaSinAnalisis=acFechaAnalisis;
		}
		
		//metodo menejador de insercion/actualizacion de datos de fecha sin analisis
		public function resultadoUpsert(fechaResultado:FechaSinAnalisisDto):void
		{
			switch (this.evento.accion)
			{
				// se agrega  un nuevo elemento a la lista en memoria.
				case CentralFechaSinAnalisisCrudEvent.AGREGAR:
					if(fechaResultado != null) modelo.centralModel.acFechaSinAnalisis.addItem(fechaResultado);
					Alert.show("Datos de Fecha sin Analisis actualizados correctamente","Comando de Fecha");
					break;
				
				case CentralFechaSinAnalisisCrudEvent.EDITAR:
					Alert.show("Datos de fecha sin Analisis actualizados correctamente","Comando de fecha");
					break;
				
				default:
					break;
			}
			
			//refrescamos la coleccion
			modelo.centralModel.acFechaSinAnalisis.refresh();
			
		}
		
		public function fault(data:Object):void
		{
			var resultado:FaultEvent= data as FaultEvent;
			Alert.show("Error Recuperando Fecha sin Analisis:\n"
						+ resultado.fault.faultString
						,"Comando de Fecha sin Analisis");
		
		}

	}
}