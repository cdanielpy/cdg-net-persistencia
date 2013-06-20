package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;

	public class ValidateSkuCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		private var newSku:String = "";
		public function execute(event:CairngormEvent):void
		{
			var bd:BusinessDelegate = new BusinessDelegate(this);
			newSku = event.data;
			bd.validateSku(event.data);
			
		}
		
		public function result(data:Object):void
		{
			if(data.result){
				model.centralModel.loadProduct.basicData.sku = newSku; 
			}else{
				model.centralModel.loadProduct.basicData.sku = "";
				Alert.show("El sku: "+ newSku+ " no es válido, se encuentra repetido o no es perteneciente a una categoría");
			}
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Hubo un error validando el sku");
		}
		
	}
}