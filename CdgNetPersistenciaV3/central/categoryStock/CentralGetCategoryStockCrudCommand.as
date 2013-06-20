package components.control.central.categoryStock
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.categoryStock.CentralGetCategoryStockCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.categoryStock.CategoryStockCrudDtoPager;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class CentralGetCategoryStockCrudCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var evt:CentralGetCategoryStockCrudEvent = CentralGetCategoryStockCrudEvent(event);  
			delegate.getCategoryStockForCRUD(evt.pager, evt.filter);
		}
		
		public function result(event:Object):void
		{
			if(event.result != null)
			{
				model.centralModel.srpGetCategoryStockCrudDto = 
					CategoryStockCrudDtoPager(ResultEvent(event).result);
				model.centralModel.categoryStockCrudDtoList.removeAll();
				model.centralModel.categoryStockCrudDtoList = 
					new ArrayCollection(ModelLocator.getInstance().centralModel.srpGetCategoryStockCrudDto.resultPart);
				model.centralModel.srpGetCategoryStockCrudDto.resultPart = 
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