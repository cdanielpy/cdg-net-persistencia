package components.control.central.destinationCrud
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.destinationCrud.CentralGetDestinationCrudEvent;
	import components.events.central.destinationCrud.CentralSaveOrUpdateDestinationCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.destinationCrud.DestinationCrudDto;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralSaveOrUpdateDestinationCrudCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var serviceName:String;
		private var filter:String ='';
		
		public function execute(event:CairngormEvent):void
		{
			var ev:CentralSaveOrUpdateDestinationCrudEvent = event as CentralSaveOrUpdateDestinationCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:DestinationCrudDto = ev.local;
			this.filter = ev.filter;
			
			if(ev.op_status==CentralSaveOrUpdateDestinationCrudEvent.DESTINATION_SAVE 
				|| ev.op_status==CentralSaveOrUpdateDestinationCrudEvent.DESTINATION_UPDATE ){
				delegate.saveOrUpdateDestinationCRUD(item);
			}	
		}
		
		public function result(event:Object):void
		{
			//TODO Refrescar la grilla -- JICM
			var e: CentralGetDestinationCrudEvent=new CentralGetDestinationCrudEvent();
			e.pager=model.centralModel.srpGetDestinationCrudDto;
			e.filter=this.filter;
			CairngormEventDispatcher.getInstance().dispatchEvent(e);
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error al buscar datos.\r\n Detalles:" + 
						evento.fault.faultString, 
						"Atencion");
		}
		
	}
}