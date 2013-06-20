package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.DeleteProductEvent;
	import components.model.ModelLocator;
	import components.vo.central.ProductResultPager;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.ResultEvent;

	public class DeleteProductCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();

		public function execute(event:CairngormEvent):void
		{
			var bs:BusinessDelegate = new BusinessDelegate(this);
			var ev:DeleteProductEvent = DeleteProductEvent(event);
			bs.deleteProduct(event.data,ev.pager,ev.search);
			
		}
		
		public function result(data:Object):void
		{
			//model.centralModel.acGetProduct = data.result;
			//ModelLocator.getInstance().centralModel.srpGetProduct = ProductResultPager(ResultEvent(data).result);
				model.centralModel.srpGetProduct = ProductResultPager(ResultEvent(data).result);
				model.centralModel.acGetProduct.removeAll();
				model.centralModel.acGetProduct = new ArrayCollection(model.centralModel.srpGetProduct.resultPart);
				model.centralModel.srpGetProduct.resultPart = new Array();
			Alert.show("Se eliminó correctamente el artículo");	
			
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error al intentar eliminar un producto");
		}
		
	}
}