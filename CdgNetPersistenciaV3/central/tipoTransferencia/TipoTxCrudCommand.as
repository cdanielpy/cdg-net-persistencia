package components.control.central.tipoTransferencia
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.tipoTransferencia.TipoTransferenciaCrudEvent;
	import components.model.ModelLocator;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class TipoTxCrudCommand implements ICommand, IResponder
	{	
		private var modelo:ModelLocator = ModelLocator.getInstance();
		public var evento:TipoTransferenciaCrudEvent;
		
		//metodo de ejecucion de procesos del comando
		public function execute(event:CairngormEvent):void
		{
			// tomamos una copia del evento
			this.evento= event as TipoTransferenciaCrudEvent;
			
			//creamos una instancia del delegado local
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			// evaluamos la accion seleccionada
			switch(this.evento.accion){
				
				case TipoTransferenciaCrudEvent.AGREGAR:
					break;
				
				case TipoTransferenciaCrudEvent.EDITAR:
					break;
				
				case TipoTransferenciaCrudEvent.ELIMINAR:
					break;

				default:
					// invocamos al metodo de recuperacion de lista de tipos de espacios adicionales
					delegate.getTransferenceTypes();
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
				case TipoTransferenciaCrudEvent.AGREGAR:
					break;
				
				case TipoTransferenciaCrudEvent.EDITAR:
					break;
				
				case TipoTransferenciaCrudEvent.ELIMINAR:
					break;
				
				default:
					modelo.management.acMotivoTx = data.result;
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
