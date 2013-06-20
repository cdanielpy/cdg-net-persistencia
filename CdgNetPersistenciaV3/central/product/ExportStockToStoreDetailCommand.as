package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	import components.modules.management.flow.transfer.utils.DownloadFile;
	
	import mx.controls.Alert;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;

	public class ExportStockToStoreDetailCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator= ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var bd:BusinessDelegate = new BusinessDelegate(this);
			bd.exportStockToStoreDetail(event.data); 
		}
		
		public function result(data:Object):void
		{
			var url:String = data.result;
			if(url != "")
            	{
	            	var pop:DownloadFile= DownloadFile(PopUpManager.createPopUp(model.crossapp.display,DownloadFile,false));	
	            	pop.set_url(url);
	            	PopUpManager.centerPopUp(pop);
            	}
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error exportando los detalles de stock");
		}
		
	}
}