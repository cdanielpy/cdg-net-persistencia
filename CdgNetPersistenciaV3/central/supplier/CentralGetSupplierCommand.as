package components.control.central.supplier
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.supplier.CentralGetSupplierEvent;
	import components.model.ModelLocator;
	import components.vo.central.SupplierLight;
	import components.vo.central.supplierCrud.SupplierDtoResultPager;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class CentralGetSupplierCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		private var evento:CentralGetSupplierEvent = null;
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			this.evento = CentralGetSupplierEvent(event);
			
			switch(this.evento.tipo)
			{
				case CentralGetSupplierEvent.BURCAR_X_ID:
					delegate.getSupplierByID(evento.data.id);
					break;
				
				default:
					delegate.getSuppliersForCRUD(this.evento.pager, this.evento.filter);
					break;
			}
			
		}
		
		public function result(event:Object):void
		{
			if(event.result != null){
				switch(this.evento.tipo)
				{
					case CentralGetSupplierEvent.BURCAR_X_ID:
						model.centralModel.supplierLightParent = new SupplierLight(event.result);
						break;
					
					default:
						model.centralModel.srpGetSupplierDto = 
							SupplierDtoResultPager(ResultEvent(event).result);
						model.centralModel.supplierDtoList.removeAll();
						model.centralModel.supplierDtoList = 
							new ArrayCollection(ModelLocator.getInstance().centralModel.srpGetSupplierDto.resultPart);
						model.centralModel.srpGetSupplierDto.resultPart = 
							new Array();
						break;
				}
			}
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error al buscar datos.\r\n Detalles:" + 
						evento.fault.faultString, 
						"Atencion");
		}
		
	}
}