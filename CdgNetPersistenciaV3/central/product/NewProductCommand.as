package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.GetProductEvent;
	import components.events.central.product.NewProductEvent;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;


	public class NewProductCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			var evet:NewProductEvent = NewProductEvent(event);
			d.newProduct(evet.basicData,evet.atribuite,evet.tax,evet.stockToDataBase,evet.acLocales);
			model.management.isSavingProduct = true;
		}
		
		public function result(data:Object):void
		{
			model.management.isSavingProduct = false;
			
			var msg:String = data.result;
			Alert.show(msg);
		}
		
		public function fault(event:Object):void
		{
			model.management.isSavingProduct = false;
			
			var evento:FaultEvent=event as FaultEvent;
			Alert.show('Error al grabar articulo.\r\n Detalles: \r\n' + 
						evento.fault.faultString, 
						'Atencion');
		}
	}
}