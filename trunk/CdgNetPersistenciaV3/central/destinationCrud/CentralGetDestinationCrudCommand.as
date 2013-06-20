package components.control.central.destinationCrud
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.destinationCrud.CentralGetDestinationCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.destinationCrud.DestinationCrudDtoPager;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class CentralGetDestinationCrudCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var evt:CentralGetDestinationCrudEvent = CentralGetDestinationCrudEvent(event);  
			delegate.getDestinationForCRUD(evt.pager, evt.filter);
		}
		
		public function result(event:Object):void
		{
			if(event.result != null)
			{
				model.centralModel.srpGetDestinationCrudDto = 
					DestinationCrudDtoPager(ResultEvent(event).result);
				model.centralModel.destinationCrudDtoList.removeAll();
				model.centralModel.destinationCrudDtoList = 
					new ArrayCollection(ModelLocator.getInstance().centralModel.srpGetDestinationCrudDto.resultPart);
				model.centralModel.srpGetDestinationCrudDto.resultPart = 
					new Array();
			}else{
				Alert.show("No se encontro información","Advertencia");
			}
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