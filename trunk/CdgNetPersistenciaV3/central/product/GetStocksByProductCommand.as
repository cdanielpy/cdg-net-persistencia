package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.jicm.controls.FaultInfoHandler;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	import components.vo.central.StockDTO;
	
	import flash.display.DisplayObject;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.core.FlexGlobals;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class GetStocksByProductCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var bd:BusinessDelegate = new BusinessDelegate(this);
			bd.getStocksByProduct(event.data);
		}
		
		public function result(data:Object):void
		{
			var tmp:ArrayCollection = data.result;
			
			for each(var stock:StockDTO in tmp){
				for(var i:Number=0;i<model.centralModel.acRepositionType.length;i++){
					var obj:Object =model.centralModel.acRepositionType.getItemAt(i);
					if(obj.id ==stock.tipoReposicionId){
						stock.selectedTipoReposicionIndex =i;
						break;
					}
				}				
			}
			
			model.centralModel.stocksByProduct = tmp;			
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			showMessg(evento.fault.rootCause + "\r\nDetalles tecnicos:\r\n" + evento.fault.faultString, 'Atencion'); 
		}
		
		private function showMessg(msg:String, titulo:String):void
		{
			var msgPopUp:FaultInfoHandler =
				FaultInfoHandler(
					PopUpManager.createPopUp(
						FlexGlobals.topLevelApplication as DisplayObject, 
						FaultInfoHandler, 
						true));
			msgPopUp.title = titulo;
			msgPopUp.txt.text = msg; 
			PopUpManager.centerPopUp(msgPopUp);
		}
		
	}
}