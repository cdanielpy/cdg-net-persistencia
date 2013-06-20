package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import mx.rpc.IResponder;
	import com.adobe.cairngorm.commands.ICommand;
	import mx.controls.Alert;
	import components.model.ModelLocator;
	import components.events.central.product.ValidateDuplicateStructureEvent;
	import components.business.BusinessDelegate;
	import mx.core.IFlexDisplayObject;
	import mx.managers.PopUpManager;
	import mx.core.Application;
	import flash.display.DisplayObject;
	import mx.rpc.events.ResultEvent;


	public class ValidateDuplicateStructureCommamd implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var ev:ValidateDuplicateStructureEvent = ValidateDuplicateStructureEvent(event);
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.validateDuplicateStructure(ev.structure);
			
		}
		
		public function result(event:Object):void
		{
			var e:ResultEvent = ResultEvent(event);
			if(-1 == event.result)
			{
				model.centralModel.isStructureInvalid = 1;
			}else{
				model.centralModel.isStructureInvalid = 0;
			}	
		}
		
		public function fault(event:Object):void
		{
			Alert.show("No se pudo realizar validacion","Error.");
		}
		
	}
}