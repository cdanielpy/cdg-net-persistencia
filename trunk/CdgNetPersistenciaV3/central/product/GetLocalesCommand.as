package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.commands.ICommand;
	import mx.rpc.IResponder;
	import components.model.ModelLocator;
	import components.business.BusinessDelegate;
	import components.events.central.product.GetLocalesEvent;
	import mx.controls.Alert;

	public class GetLocalesCommand implements ICommand, IResponder
	{
		[Bindable] public var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			var evet:GetLocalesEvent = GetLocalesEvent(event);
			d.getLocales(evet.acEnterprice,evet.acMarketChain,evet.acRegion);
		}
		
		public function result(data:Object):void
		{
			model.centralModel.acLocales = data.result;
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error al buscar los locales.","Error.");
		}
		
	}
}