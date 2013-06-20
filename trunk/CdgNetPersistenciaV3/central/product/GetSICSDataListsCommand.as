package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class GetSICSDataListsCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			d.getSICSDataLists();
		}
		
		public function result(data:Object):void
		{
			if(data.result != null)
			{
				var tmpArray:ArrayCollection = data.result; 
				if  (tmpArray.length > 0){ // cocampos
					model.centralModel.marcaList = tmpArray[0] as ArrayCollection;
					model.centralModel.envaseList = tmpArray[1] as ArrayCollection;
					model.centralModel.procedenciaList = tmpArray[2] as ArrayCollection;
					model.centralModel.clarList = tmpArray[3] as ArrayCollection;
				}
			}
			else{
				Alert.show("No se encontro información (SICS Data)","Advertencia");
			}
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error.\r\n"+evento.fault.faultString,"Error.");
		}
	}
}
