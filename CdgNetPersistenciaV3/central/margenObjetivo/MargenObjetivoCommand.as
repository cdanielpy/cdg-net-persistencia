package components.control.central.margenObjetivo
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class MargenObjetivoCommand implements ICommand, IResponder
	{

		private var model:ModelLocator=ModelLocator.getInstance();

		public function execute(event:CairngormEvent):void
		{
			new BusinessDelegate(this).margenObjetivo(Number(event.data.categoryCode),Number(event.data.margenPorc));
		}

		public function result(data:Object):void
		{
			Alert.show("Datos Actualizados Correctamente: \r\n", "Atencion");	
		}

		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			Alert.show("Error al grabar Margen Objetivo.\r\n Detalles: \r\n" + evento.fault.message, "Atencion");

		}
	}
}