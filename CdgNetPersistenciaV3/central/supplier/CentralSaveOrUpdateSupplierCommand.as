package components.control.central.supplier
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import com.emeretail.emecrossapp.utils.export.GenericAlertPanel;
	import com.jicm.controls.FaultInfoHandler;
	
	import components.business.BusinessDelegate;
	import components.events.central.supplier.CentralGetSupplierEvent;
	import components.events.central.supplier.CentralSaveOrUpdateSupplierEvent;
	import components.model.ModelLocator;
	import components.vo.central.supplierCrud.SupplierDto;
	
	import flash.display.DisplayObject;
	
	import mx.core.FlexGlobals;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralSaveOrUpdateSupplierCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var serviceName:String;
		private var filter:String ='';
		private var ev:CentralSaveOrUpdateSupplierEvent;
		
		public function execute(event:CairngormEvent):void
		{
			ev = event as CentralSaveOrUpdateSupplierEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:SupplierDto = ev.supplier;
			this.filter = ev.filter;
			
			if(ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_SAVE || 
			   ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_UPDATE )
			{
				delegate.saveOrUpdateSupplierCRUD(item);
			}
			
			if(ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_COPY_MATRIX)
			{
				delegate.copySupplierMatrix(ev.data.origin,ev.data.destiny)
			}		
			
			if(ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_DTO_GET_BY_ID)
			{
				delegate.getSupplierDtoByID(ev.data);
			}	
		}
		
		public function result(event:Object):void
		{
			if(ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_COPY_MATRIX)
			{
					var comp:GenericAlertPanel=
					GenericAlertPanel(PopUpManager.createPopUp(
						(FlexGlobals.topLevelApplication as DisplayObject), 
						GenericAlertPanel, 
						false));
					comp.textComp.text="Datos copiados.";
					comp.styleName='panelFlow';
			} else if (ev.op_status==CentralSaveOrUpdateSupplierEvent.SUPPLIER_DTO_GET_BY_ID) {
				model.centralModel.supplierDtoSelected = event.result;
			} else {		
				//Obs.: Carga de proveedores segun PAGER -- JICM
				var e:CentralGetSupplierEvent=new CentralGetSupplierEvent();
				e.pager=model.centralModel.srpGetSupplierDto;
				e.filter=this.filter;
				CairngormEventDispatcher.getInstance().dispatchEvent(e);
			}	
		}
		
		public function fault(event:Object):void
		{
			model.management.isLoadingCardex = false;
			model.management.isLoadingMermaNatural=false;
			model.management.isLoadingActualPrice=false;
			
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