package components.control.central.categoryStock
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.categoryStock.CentralGetCategoryStockCrudEvent;
	import components.events.central.categoryStock.CentralSaveOrUpdateCategoryStockCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.categoryStock.CategoryStockCrudDto;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralSaveOrUpdateCategoryStockCrudCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var serviceName:String;
		private var filter:String ='';
		
		public function execute(event:CairngormEvent):void
		{
			var ev:CentralSaveOrUpdateCategoryStockCrudEvent = event as CentralSaveOrUpdateCategoryStockCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:CategoryStockCrudDto = ev.local;
			this.filter = ev.filter;
			
			if(ev.op_status==CentralSaveOrUpdateCategoryStockCrudEvent.SAVE 
				|| ev.op_status==CentralSaveOrUpdateCategoryStockCrudEvent.UPDATE ){
				delegate.saveOrUpdateCategoryStockCRUD(item);
			}	
		}
		
		public function result(event:Object):void
		{
			//TODO Refrescar la grilla -- JICM
			var e: CentralGetCategoryStockCrudEvent=new CentralGetCategoryStockCrudEvent();
			e.pager=model.centralModel.srpGetCategoryStockCrudDto;
			e.filter=this.filter;
			CairngormEventDispatcher.getInstance().dispatchEvent(e);
			Alert.show("La categoria fue insertada correctamente");
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error al buscar datos.\r\n Detalles:" + 
						evento.fault.faultString + "" +
						evento.fault.rootCause + " " +  
						evento.fault.faultDetail, "Atencion");
		}
		
	}
}