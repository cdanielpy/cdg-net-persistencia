package components.control.central.tipoEspacio
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.tipoEspacio.TipoContratoEspacioCrudEvent;
	import components.model.ModelLocator;
	import components.vo.flow.espacioAdicional.TipoContratoEspacio;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent; 
	import mx.rpc.events.ResultEvent;
	
	
	public class TipoContratoEspacioCRUDCommand implements ICommand, IResponder
	{
		private var modelo:ModelLocator = ModelLocator.getInstance();
		public var evento:TipoContratoEspacioCrudEvent;
		
		//metodo de ejecucion de procesos del comando
		public function execute(event:CairngormEvent):void
		{
			// tomamos una copia del evento
			this.evento= event as TipoContratoEspacioCrudEvent;
			
			//creamos una instancia del delegado local
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			// evaluamos la accion seleccionada
			switch(this.evento.accion){
				default:
					// invocamos al metodo de recuperacion de lista de tipos de contratos de espacios
					delegate.getListaTiposContratosEspacios();
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
				
				default:
					modelo.centralModel.acTiposContratosEspacios = resultado.result as ArrayCollection;
					break;
			}
		}
		
		// manejador de errores
		public function fault(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:FaultEvent = data as FaultEvent;
			Alert.show("Error recupertando Tipos de Contratos de Espacios:\n" 
				+ resultado.message
				,"Comando de Tipo Contrato de Espacio");
		}
	}
}