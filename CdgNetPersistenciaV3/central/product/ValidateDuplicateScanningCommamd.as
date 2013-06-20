package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import mx.rpc.IResponder;
	import com.adobe.cairngorm.commands.ICommand;
	import components.model.ModelLocator;
	import components.events.central.product.ValidateDuplicateScanningEvent;
	import components.business.BusinessDelegate;
	import mx.rpc.events.ResultEvent;
	import mx.controls.Alert;

	public class ValidateDuplicateScanningCommamd implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var ev:ValidateDuplicateScanningEvent = ValidateDuplicateScanningEvent(event);
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.validateDuplicateScanning(Number(ev.scanning));
			
		}
		
		public function result(event:Object):void
		{
			var e:ResultEvent = ResultEvent(event);
			if(-1 == event.result)
			{
				model.centralModel.isScanningInvalid = 1;
			}else{
				model.centralModel.isScanningInvalid = 0;
			}	
		}
		
		public function fault(event:Object):void
		{
			Alert.show("No se pudo realizar validacion","Error.");
		}
		
	}
}