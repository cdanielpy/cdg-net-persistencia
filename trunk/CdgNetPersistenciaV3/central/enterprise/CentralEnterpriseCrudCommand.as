package components.control.central.enterprise
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import com.emeretail.emecrossapp.utils.export.GenericAlertPanel;
	
	import components.business.BusinessDelegate;
	import components.events.central.enterprise.CentralEnterpriseCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.enterprise.EnterpriseCrudDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralEnterpriseCrudCommand implements ICommand, IResponder
	{
		private var ev:CentralEnterpriseCrudEvent;
		private var model : ModelLocator = ModelLocator.getInstance();
		private var filter:String ='';
		
		public function execute(event:CairngormEvent):void
		{
			this.ev = event as CentralEnterpriseCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:EnterpriseCrudDto = ev.dto;
			
			//Alert.show('DEBUG: this.ev.op_status ' + this.ev.op_status );
			
			switch (this.ev.op_status)
			{
				case CentralEnterpriseCrudEvent.DELETE:
					delegate.deleteEmpreCRUD(item);
					break;
				case CentralEnterpriseCrudEvent.UPDATE:
					delegate.saveOrUpdateEmpreCRUD(item);
					break;
				case CentralEnterpriseCrudEvent.SAVE:
					delegate.saveOrUpdateEmpreCRUD(item);
					break;
				case CentralEnterpriseCrudEvent.FINDALL:
					delegate.findAllEmpreCRUD();
					break;
			}
		}
		
		public function result(data:Object):void
		{
			switch (this.ev.op_status)
			{
				case CentralEnterpriseCrudEvent.FINDALL:
					model.centralModel.empresas = ArrayCollection(data.result);
					model.centralModel.empresas.refresh();
					break;
				default:
					var compMssg:GenericAlertPanel=GenericAlertPanel(PopUpManager.createPopUp(ModelLocator.getInstance().crossapp.display, GenericAlertPanel, false));
					compMssg.textComp.text="Operacion realizada con exito.";
					compMssg.styleName='panelFlow';
					var evtLista:CentralEnterpriseCrudEvent = new CentralEnterpriseCrudEvent(CentralEnterpriseCrudEvent.FINDALL,null); 
					CairngormEventDispatcher.getInstance().dispatchEvent(evtLista);
					break;
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