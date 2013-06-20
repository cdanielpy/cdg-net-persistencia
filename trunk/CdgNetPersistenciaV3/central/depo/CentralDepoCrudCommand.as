package components.control.central.depo
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	
	import components.business.BusinessDelegate;
	import components.events.central.depo.CentralDepoCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.depo.DepoCrudDto;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class CentralDepoCrudCommand implements ICommand, IResponder
	{
		private var ev:CentralDepoCrudEvent;
		private var model : ModelLocator = ModelLocator.getInstance();
		private var filter:String ='';
		
		public function execute(event:CairngormEvent):void
		{
			this.ev = event as CentralDepoCrudEvent;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var item:DepoCrudDto = ev.depo;
			
			//Alert.show('DEBUG: this.ev.op_status ' + this.ev.op_status );
			
			switch (this.ev.op_status)
			{
				case CentralDepoCrudEvent.DELETE:
					delegate.deleteDepoCRUD(item);
					break;
				case CentralDepoCrudEvent.UPDATE:
					delegate.saveOrUpdateDepoCRUD(item);
					break;
				case CentralDepoCrudEvent.SAVE:
					delegate.saveOrUpdateDepoCRUD(item)
					break;
				case CentralDepoCrudEvent.FINDALL:
					delegate.findAllDepoCRUD();
					break;
				case CentralDepoCrudEvent.GETDESTINATIONS:
					delegate.getDestinationForDepoCRUD();
					break;
			}
		}
		
		public function result(data:Object):void
		{
			switch (this.ev.op_status)
			{
				case CentralDepoCrudEvent.FINDALL:
					var localIDString:String = this.ev.filter;
					model.centralModel.deposCrudDtoList=data.result;
					if ( localIDString!=null && localIDString.length>0 ) {           	
						model.centralModel.deposCrudDtoList.filterFunction=filterFunc;
					} else {
						model.centralModel.deposCrudDtoList.filterFunction=null;
					}
					break;
				case CentralDepoCrudEvent.GETDESTINATIONS:
					model.centralModel.deposCrudDtoDestinationList=data.result;
					break;
				default:
					var e: CentralDepoCrudEvent=new CentralDepoCrudEvent();
					e.filter=this.ev.filter;
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
         	var dItem:DepoCrudDto = new DepoCrudDto(item);
         	var localIDString:String = this.ev.filter;
           	var value:Boolean = false;
           	if(dItem.description.toString().length >= localIDString.length){
           		value = dItem.destination.toString().toLowerCase() == localIDString.toLowerCase();
	  		}
           	return value;
        }		
	}
}