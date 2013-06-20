package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;


	public class GetInternalWeightablePluCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			d.getInternalWeightablePluCommand();
		}
		
		public function result(data:Object):void
		{
			model.internalWeightablePluAndVD = data.result;
			//Alert.show( model.internalWeightablePluAndVD.toString() );
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show('Error.\r\n Detalles: \r\n' + 
						evento.fault.faultString, 
						'Atencion');

		}
		
	}
}