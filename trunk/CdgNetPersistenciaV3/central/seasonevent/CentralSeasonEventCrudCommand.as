package components.control.central.seasonevent
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.seasonevent.CentralSeasonEventCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.seasonevent.SeasonEventCrudDto;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralSeasonEventCrudCommand implements ICommand, IResponder
	{
		private var ev:CentralSeasonEventCrudEvent;
		private var model : ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			this.ev = event as CentralSeasonEventCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:SeasonEventCrudDto = ev.seasonevent;
			
			//Alert.show('DEBUG: this.ev.op_status ' + this.ev.op_status );
			
			switch (this.ev.op_status)
			{
				case CentralSeasonEventCrudEvent.DELETE:
					delegate.deleteSeasonEventCRUD(item);
					break;
				case CentralSeasonEventCrudEvent.UPDATE:
					delegate.saveOrUpdateSeasonEventCRUD(item);
					break;
				case CentralSeasonEventCrudEvent.SAVE:
					delegate.saveOrUpdateSeasonEventCRUD(item)
					break;
				case CentralSeasonEventCrudEvent.FINDALL:
					delegate.findAllSeasonEventCRUD();
					break;
			}
		}
		
		public function result(data:Object):void
		{
			switch (this.ev.op_status)
			{
				case CentralSeasonEventCrudEvent.FINDALL:
					model.centralModel.seasonEventsCrudDtoList=data.result;
					break;
				default: // Obs. al terminar un crear, actualizar, borrar se dispara un buscar automaticamente
					var e: CentralSeasonEventCrudEvent=new CentralSeasonEventCrudEvent();
					CairngormEventDispatcher.getInstance().dispatchEvent(e);
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
		
		
       public function filterFunc(item:Object):Boolean {
           	return false;
        }		
	}
}