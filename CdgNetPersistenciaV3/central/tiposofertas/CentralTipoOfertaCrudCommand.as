package components.control.central.tiposofertas
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import com.emeretail.emecrossapp.utils.export.GenericAlertPanel;
	
	import components.business.BusinessDelegate;
	import components.events.central.tiposofertas.CentralTiposOfertasCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.tiposofertas.TipoOfertaDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralTipoOfertaCrudCommand implements ICommand, IResponder
	{
		private var ev:CentralTiposOfertasCrudEvent;
		private var model : ModelLocator = ModelLocator.getInstance();
		private var filter:String ='';
		
		public function execute(event:CairngormEvent):void
		{
			this.ev = event as CentralTiposOfertasCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:TipoOfertaDto = ev.dto;
			
			//Alert.show('DEBUG: this.ev.op_status ' + this.ev.op_status );
			
			switch (this.ev.op_status)
			{
				case CentralTiposOfertasCrudEvent.DELETE:
					delegate.deleteTiposOfertasCRUD(item);
					break;
				case CentralTiposOfertasCrudEvent.UPDATE:
					delegate.saveOrUpdateTiposOfertasCRUD(item);
					break;
				case CentralTiposOfertasCrudEvent.SAVE:
					delegate.saveOrUpdateTiposOfertasCRUD(item);
					break;
				case CentralTiposOfertasCrudEvent.FINDALL:
					delegate.findAllTiposOfertasCRUD();
					break;
			}		
		}
		
		public function result(data:Object):void
		{
			switch (this.ev.op_status)
			{
				case CentralTiposOfertasCrudEvent.FINDALL:
				
					model.pricing.price.offerTypes = ArrayCollection(data.result);
					model.centralModel.tiposOfertasCrudDtoList = ArrayCollection(data.result);
					
					if ( this.ev.findAllShowTodos ) {
						var todos:TipoOfertaDto = new TipoOfertaDto();
						todos.id = -1;
						todos.description = 'Todos';
						model.centralModel.tiposOfertasCrudDtoList.addItemAt(todos, 0);
					}
					
					
					model.centralModel.tiposOfertasCrudDtoList.refresh();
					break;
				default:
					var compMssg:GenericAlertPanel=GenericAlertPanel(PopUpManager.createPopUp(ModelLocator.getInstance().crossapp.display, GenericAlertPanel, false));
					compMssg.textComp.text="Operacion realizada con exito.";
					compMssg.styleName='panelFlow';
					var evtLista:CentralTiposOfertasCrudEvent = new CentralTiposOfertasCrudEvent(CentralTiposOfertasCrudEvent.FINDALL,null); 
					CairngormEventDispatcher.getInstance().dispatchEvent(evtLista);
					break;
			}		
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Se produjo un error al buscar datos.\r\n Detalles:" + 
						evento.fault.message, 
						"Atencion");
		}
	}
}