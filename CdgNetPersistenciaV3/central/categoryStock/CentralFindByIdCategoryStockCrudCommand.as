package components.control.central.categoryStock
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.categoryStock.CentralFindByIdCategoryStockCrudEvent;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralFindByIdCategoryStockCrudCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var storageVar:int =0;
		
		public function execute(event:CairngormEvent):void
		{
			var ev:CentralFindByIdCategoryStockCrudEvent = event as CentralFindByIdCategoryStockCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var id:Number = ev.data.id;
			this.storageVar = ev.data.storageVar;
			delegate.findByIdCategoryStockCRUD(id);
		}
		
		public function result(data:Object):void
		{
			switch (this.storageVar)
			{
				case CentralFindByIdCategoryStockCrudEvent.GENERIC:
					model.centralModel.categoryStockCrudDtoFindGeneric=data.result;
					break;
				case CentralFindByIdCategoryStockCrudEvent.RELATED_PARENT:
					model.centralModel.categoryStockCrudDtoFindParent=data.result;
					break;
				default:
				    model.centralModel.categoryStockCrudDtoFindGeneric=data.result;
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
		
	}
}