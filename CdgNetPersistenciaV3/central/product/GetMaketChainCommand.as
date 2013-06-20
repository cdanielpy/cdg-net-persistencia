package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.commands.ICommand;
	import mx.rpc.IResponder;
	import mx.controls.Alert;
	import components.model.ModelLocator;
	import components.business.BusinessDelegate;

	public class GetMaketChainCommand implements ICommand, IResponder
	{
		[Bindable] public var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			d.getMaketChain();
		}
		
		public function result(data:Object):void
		{
			model.centralModel.acMaketChain = data.result;
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error al buscar los datos.","Error.");
		}
		
	}
}