package components.control.central.tipoinventario
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.tipoinventario.CentralTipoInventarioCRUDEvent;
	import components.model.ModelLocator;
	import components.vo.central.tipoinventario.TipoInventarioDTO;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralTipoInventarioCRUDCommand implements ICommand, IResponder
	{
		private var ev:CentralTipoInventarioCRUDEvent;
		private var model : ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			this.ev = event as CentralTipoInventarioCRUDEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:TipoInventarioDTO = ev.tipoinventario;
			
			//Alert.show('DEBUG: this.ev.op_status ' + this.ev.op_status );
			
			switch (this.ev.op_status)
			{
				case CentralTipoInventarioCRUDEvent.DELETE:
				case CentralTipoInventarioCRUDEvent.UPDATE:
				case CentralTipoInventarioCRUDEvent.SAVE:
					delegate.makeTipoInventarioCRUD(this.ev.op_status, item)
					break;
				case CentralTipoInventarioCRUDEvent.FINDALL:
					delegate.findAllTipoInventarioCRUD();
					break;
			}
		}
		
		public function result(data:Object):void
		{
			switch (this.ev.op_status)
			{
				case CentralTipoInventarioCRUDEvent.FINDALL:
					model.centralModel.tipoInventarioDTOList=data.result;
					break;
				default: // Obs. al terminar un crear, actualizar, borrar se dispara un buscar automaticamente
					var e: CentralTipoInventarioCRUDEvent=new CentralTipoInventarioCRUDEvent();
					CairngormEventDispatcher.getInstance().dispatchEvent(e);
					break;
			}
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error al buscar datos.\r\n Detalles:" + 
						evento.fault.faultString, 
						"Atencion");
		}
		
		
       public function filterFunc(item:Object):Boolean {
           	return false;
        }		
	}
}