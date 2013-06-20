package components.control.central.categoryStock
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralGetCategoryStockFormatCrudCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.getCategoryStockFormatForCRUD();
		}
		
		public function result(event:Object):void
		{
			if(event.result != null)
			{
				model.centralModel.categoryStockFormatList.removeAll();
				model.centralModel.categoryStockFormatList = event.result;
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