package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.jicm.controls.FaultInfoHandler;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class GetEnterpriceCommand implements ICommand, IResponder
	{
		[Bindable] public var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			d.getEnterprice();
		}
		
		public function result(data:Object):void
		{
			model.centralModel.acEnterprice = data.result;
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			var faultPopUp:FaultInfoHandler =
				FaultInfoHandler(
					PopUpManager.createPopUp(ModelLocator.getInstance().crossapp.display, FaultInfoHandler, true));
			faultPopUp.txt.text = evento.fault.rootCause + "\r\n" + 
								  "Detalles tecnicos:\r\n" + evento.fault.faultString; 
			PopUpManager.centerPopUp(faultPopUp);
		}
		
	}
}