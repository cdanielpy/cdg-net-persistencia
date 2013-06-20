package components.control.central.enterprise
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.emeretail.emecrossapp.vo.Enterprise;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class GetCentralEnterprisesDataCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.getListaEmpresas();
		}
		
		public function result(event:Object):void
		{
			var ac:ArrayCollection = new ArrayCollection();
			for each (var item:Object in ResultEvent(event).result) {
				ac.addItem(new Enterprise(item));
			}
			model.centralModel.empresas = ac;
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Error al obtener datos.\r\n Detalles: \r\n" + 
						evento.fault.faultString, 
						"Atencion");

		}
		
	}
}