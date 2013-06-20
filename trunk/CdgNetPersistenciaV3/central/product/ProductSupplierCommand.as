package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.ProductSupplierEvent;
	import components.model.ModelLocator;
	import components.modules.management.providers.espaciosAdicionales.EspacioAdicionalColectores;
	import components.modules.opd.Proveedores;
	import components.vo.flow.suppliers.EspacioAdicional;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;
	
	public class ProductSupplierCommand implements ICommand, IResponder
	{
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		private var evento:ProductSupplierEvent;
		
		public function execute(event:CairngormEvent):void
		{
			//tomamos la instancia del evento
			this.evento = event as ProductSupplierEvent;
			
			//referenciamos al delegado
			var d:BusinessDelegate = new BusinessDelegate(this);
			
			//evaluamos el tipo de evento
			switch(this.evento.nTipo)
			{
				case ProductSupplierEvent.GET_PROVEEDORES_X_PRODUCTOS:
					d.completarLecturaEspacioAdicional(this.evento.data);
					break;
			}
		}
		
		public function result(e:Object):void
		{
			var exito:ResultEvent = (e as ResultEvent);
			model.management.acLecturasEspaciosAdicionales = exito.result as ArrayCollection;
			
			//evaluamos el tipo de evento
			switch(this.evento.nTipo)
			{
				case ProductSupplierEvent.GET_PROVEEDORES_X_PRODUCTOS:
					//si el lanzador es la grilla de descarga de datos de colectores
					if(evento.oLanzador != null) evento.oLanzador.dispatchEvent(new Event("REFRESH"));
					break;
			}
		}
		
		public function fault(e:Object):void
		{
			var falla:FaultEvent = (e as FaultEvent);
			
			Alert.show("Error recuperando Proveedores de Productos: " + falla.fault.rootCause 
				+ "\r\nDetalles tecnicos:\r\n"
				+ falla.fault.faultString, 'Atencion'
			);
		}
		
	}
}