package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;

	public class SearchForProductsCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		public function execute(event:CairngormEvent):void
		{
			var bd:BusinessDelegate = new BusinessDelegate(this);
			bd.searchForProducts(event.data);
		}
		
		public function result(data:Object):void
		{
			model.centralModel.acSearchForProducts = data.result;
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Hubo un error buscando los productos");
		}
		
	}
}