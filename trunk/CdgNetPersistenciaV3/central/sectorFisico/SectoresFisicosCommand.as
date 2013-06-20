package components.control.central.sectorFisico
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.sectorFisico.SectoresFisicosEvent;
	import components.model.ModelLocator;
	import components.vo.central.sectoresFisicos.SectorFisicoDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;
	
	
	public class SectoresFisicosCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		private var evento:SectoresFisicosEvent;
		
		public function execute(event:CairngormEvent):void
		{
			//tomamos la instancia del evento
			this.evento = event as SectoresFisicosEvent;
			
			//referenciamos al delegado
			var d:BusinessDelegate = new BusinessDelegate(this);
			
			//evaluamos el tipo de evento
			switch(this.evento.nTipo)
			{
				case SectoresFisicosEvent.PERSISTIR_SECTOR:
					d.persistirSectorFisico(this.evento.data);
					break;
				
				default:
					d.listarSectoresFisicos();
					break;
				
			}
			
		}
		
		public function result(data:Object):void
		{
			//casteamos el resultado a su tipo
			var oResultado:ResultEvent = data as ResultEvent;
			
			//evaluamos el tipo de evento
			switch(this.evento.nTipo)
			{
				case SectoresFisicosEvent.PERSISTIR_SECTOR:
					this.model.centralModel.sectorFisicoEditando = SectorFisicoDto(oResultado.result);
					break;
				
				default:
					this.model.centralModel.acSectoresFisicos = oResultado.result as ArrayCollection;
					this.model.centralModel.acSectoresFisicos.refresh();
					break;
				
			}
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Error recuperando Sectores Fisicos: " + evento.fault.rootCause 
						+ "\r\nDetalles tecnicos:\r\n"
						+ evento.fault.faultString, 'Atencion'
						);
		}
	}
}