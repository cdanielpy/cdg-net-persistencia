package components.control.central.tipoEspacio
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.tipoEspacio.CentralTipoEspacioCrudEvent;
	import components.model.ModelLocator;
	import components.vo.flow.suppliers.TiposDeEspacios;
	
	import flash.events.Event;
	import flash.events.EventDispatcher;
	
	import mx.collections.ArrayCollection; 
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class TipoEspacioCrudCommand implements ICommand, IResponder
	{	
		private var modelo:ModelLocator = ModelLocator.getInstance();
		public var evento:CentralTipoEspacioCrudEvent;
		
		//metodo de ejecucion de procesos del comando
		public function execute(event:CairngormEvent):void
		{
			// tomamos una copia del evento
			this.evento= event as CentralTipoEspacioCrudEvent;
			
			//creamos una instancia del delegado local
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			// evaluamos la accion seleccionada
			switch(this.evento.accion){
				
				case CentralTipoEspacioCrudEvent.AGREGAR:
					delegate.upsertTipoEspacio(modelo.centralModel.tipoEspacioEditando);
					break;
				
				case CentralTipoEspacioCrudEvent.EDITAR:
					delegate.upsertTipoEspacio(modelo.centralModel.tipoEspacioEditando);
					break;
				
				default:
					// invocamos al metodo de recuperacion de lista de tipos de espacios adicionales
					delegate.getListaTipoEspacio();
					break;
			}
		}
		
		//manejador de resultado exitoso
      	public function result(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:ResultEvent = data as ResultEvent;
			
			// evaluamos la accion ejecutando y llamamos al metodo correspondiente
			switch(this.evento.accion){
				case CentralTipoEspacioCrudEvent.AGREGAR:
					this.resultadoUpsert(resultado.result as TiposDeEspacios);
					break;
					
				case CentralTipoEspacioCrudEvent.EDITAR:
					this.resultadoUpsert(resultado.result as TiposDeEspacios);
					break;
					
				default:
					this.listarTipoEspacio(resultado.result as ArrayCollection);
					break;
			}
		}
		
		// metodo manejador de listado de cadcenas de competencia
		private function listarTipoEspacio(acEspacio:ArrayCollection):void{
			
			//pasamos la coleccion al modelo de datos
			modelo.centralModel.acTipoEspacio = acEspacio;
			
		}
		
		// metodo manejador de insercion/actualizacion de datos de tipos de espacios
		private function resultadoUpsert(tipoResultado:TiposDeEspacios):void{
			
			// pasamos el elemento devuelto a la instancia en memoria
			modelo.centralModel.tipoEspacioEditando = tipoResultado;
			
			switch(this.evento.accion){
				
				case CentralTipoEspacioCrudEvent.AGREGAR:
					// agregamos el nuevo elemento a la lista en memoria
					modelo.centralModel.acTipoEspacio.addItem(tipoResultado);
					modelo.centralModel.acTipoEspacio.refresh();
					
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Tipo Espacio Agregados Correctamente","Comando de Tipo Espacio");
					break; 
				
				case CentralTipoEspacioCrudEvent.EDITAR:
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Tipo Espacio Actualizados Correctamente","Comando de Tipo Espacio");
					break;
			}
			
		}
		
		// manejador de errores
		public function fault(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:FaultEvent = data as FaultEvent;
			Alert.show("Error recupertando Tipos de Espacios Adicionales:\n" 
						+ resultado.message
						,"Comando de Tipo Espacio");
		}
	}
}
