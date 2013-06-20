package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.CentralBusinessDelegate;
	import components.events.central.product.CentralEvent;
	import components.model.ModelLocator;
	
	import flash.display.DisplayObject;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;

	public class CentralCommand implements ICommand, IResponder	{
		
		private var ev:CentralEvent;
		private var container:DisplayObject;
		private var tipeEvent:int;
		private var model:ModelLocator = ModelLocator.getInstance();

		public function execute(event:CairngormEvent):void{
			ev = CentralEvent(event);
			this.tipeEvent = ev.tipo;
			switch(this.tipeEvent){
				case CentralEvent.GET_INTERNAL_SUPPLIERS:
					new CentralBusinessDelegate(this).getInternalSuppler();
					break;
				default:break;	
			}
		}
		
		public function result(data:Object):void{
			switch(this.tipeEvent){
				case CentralEvent.GET_INTERNAL_SUPPLIERS:
					model.centralModel.acInternalSupplier = data.result;
					break;
				default:break;	
			}
		}
		
		public function fault(info:Object):void{
			Alert.show("Error code " + this.tipeEvent);
		}
		
	}
}