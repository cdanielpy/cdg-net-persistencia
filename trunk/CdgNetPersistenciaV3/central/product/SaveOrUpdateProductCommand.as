package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	import components.modules.central.products.modificaMasivo.ModificacionMasivaProductos;
	import components.modules.crossapp.GenericAlertPanel;
	import components.vo.central.ProductResultPager;
	import components.vo.crossapp.CampoDeTablaVO;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class SaveOrUpdateProductCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var ev:SaveOrUpdateProductEvent; 
		
		
		public function execute(event:CairngormEvent):void
		{
			//tomamos la refencia al evento
			this.ev= SaveOrUpdateProductEvent(event);
			
			//instanciamos el delegado
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			//evaluamos el tipo de evento
			switch(this.ev.cTipo)
			{
				case SaveOrUpdateProductEvent.ACTUALIZACION_MASIVA:
					delegate.actualizarMasivamente((this.ev.oVisualizador as ModificacionMasivaProductos).acScannings
													, (this.ev.data as CampoDeTablaVO).nombreDeCampo
													, (this.ev.data as CampoDeTablaVO).nuevoValorBool); 
					break;
				
				default:
					//por defecto, lo que hacia originalmente
					delegate.saveOrUpdateProduct(ev.product);
					break;
			}
		}
		
		public function result(event:Object):void
		{
			//casteamos el resultado a su tipo
			var eResultado:ResultEvent = event as ResultEvent;
			
			//evaluamos el tipo de evento
			switch(this.ev.cTipo)
			{
				case SaveOrUpdateProductEvent.ACTUALIZACION_MASIVA:
					//tomamos la coleccion devuelta
					var acResultado:ArrayCollection = eResultado.result as ArrayCollection;
					
					var comp:GenericAlertPanel=GenericAlertPanel(PopUpManager.createPopUp(this.ev.oVisualizador, GenericAlertPanel, false));
					
					//lo evaluamos
					if(acResultado != null && acResultado.length > 0)
					{
						//si fue exitoso
						if(Number(acResultado.getItemAt(0)) == 1)
						{
							//lanzamos una notificacion
							comp.textComp.text="Articulos actualizados correctamente!!";
							
							//reseteamos la coleccion de productos del modelo, para liberar la grilla y memoria
							this.model.centralModel.acActualizacionProductos.removeAll();
							this.model.centralModel.acActualizacionProductos.refresh();
						}
						else
						{
							//lanzamos una notificacion
							comp.textComp.text="Error actualizando articulos:\n\n" + acResultado.getItemAt(1).toString();
						}
					}
					else
					{
						//sino
						comp.textComp.text="No se recibio respuesta alguna desdes el Servidor!";
					}
					
					//establecemos el estilo
					comp.styleName='panelFlow';
					
					break;
				
				default:
					//por defecto, lo que hacia originalmente... NADA
					break;
			}
		}
		
		public function fault(info:Object):void
		{
			
			var eError:FaultEvent = info as FaultEvent;
			
			//evaluamos el tipo de evento
			switch(this.ev.cTipo)
			{
				case SaveOrUpdateProductEvent.ACTUALIZACION_MASIVA:
					Alert.show("Error actualizando articulos: \n\n" + eError.message,"Error.");
					break;
				
				default:
					//por defecto, lo que hacia originalmente
					Alert.show("No se pudo crear o actualizar el Producto","Error.");
					break;
			}
		}
		
	}
}