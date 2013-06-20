package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.jicm.controls.FaultInfoHandler;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.GetProductEvent;
	import components.model.ModelLocator;
	import components.vo.central.ProductLigh;
	import components.vo.central.ProductResultPager;
	
	import flash.display.DisplayObject;
	
	import flashx.textLayout.elements.BreakElement;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.core.Application;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class GetProductCommand implements ICommand, IResponder
	{
		private var filter:Boolean;
		[Bindable]
		private var model:ModelLocator = ModelLocator.getInstance();
		
		//Agregado -- CDGS - 06/03/2013 - para recuperar datos de productos a partir de una lista de scannings
		private var evet:GetProductEvent;
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new BusinessDelegate(this);
			
			this.evet = event as GetProductEvent;
			
			filter=evet.filterResult;
			
			//d.getProduct(evet.pager,evet.search,evet.supplierId,evet.sectorId,evet.goForActivesOnly);
			d.getProduct(
				evet.pager,
				evet.search,
				evet.supplierId,
				evet.sectorId,
				evet.goForActivesOnly, 
				evet.activosVenta, 
				evet.activosCompra, 
				evet.soloEstacionales,
				evet.acScannings
			);
		}
		
		public function result(data:Object):void
		{
			//si hay datos devueltos
			if(data.result!= null)
			{
				
				//evaluamos el tipo de evento esperado
				switch(this.evet.cTipoAsignacion)
				{
					case GetProductEvent.ASIGNACION_MASIVA:
						ModelLocator.getInstance().centralModel.srpUpdateArticulos=ProductResultPager(ResultEvent(data).result);
						ModelLocator.getInstance().centralModel.acAsignacionMasiva.removeAll();
						ModelLocator.getInstance().centralModel.acAsignacionMasiva=new ArrayCollection(ModelLocator.getInstance().centralModel.srpUpdateArticulos.resultPart);
						ModelLocator.getInstance().centralModel.srpUpdateArticulos.resultPart=new Array();
						break;
					
					case GetProductEvent.ACTUALIZACION_MASIVA:
						ModelLocator.getInstance().centralModel.srpActualizacionProductos = ProductResultPager(ResultEvent(data).result);
						ModelLocator.getInstance().centralModel.acActualizacionProductos.removeAll();
						ModelLocator.getInstance().centralModel.acActualizacionProductos = new ArrayCollection(ModelLocator.getInstance().centralModel.srpActualizacionProductos.resultPart);
						ModelLocator.getInstance().centralModel.srpActualizacionProductos.resultPart = new Array();
						break;
					
					default:
						//por defecto, como estaba originalmente
						
						ModelLocator.getInstance().centralModel.srpGetProduct = ProductResultPager(ResultEvent(data).result);
						ModelLocator.getInstance().centralModel.acGetProduct.removeAll();
						ModelLocator.getInstance().centralModel.acGetProduct = new ArrayCollection(ModelLocator.getInstance().centralModel.srpGetProduct.resultPart);
						ModelLocator.getInstance().centralModel.srpGetProduct.resultPart = new Array();
						
						if(filter){
							remove();					
						}
						break;
				}
			}
			else{
				//sino, notificacion
				Alert.show("No se encontro información","Advertencia");
			}
		}
		
		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			showMessg(evento.fault.rootCause + "\r\nDetalles tecnicos:\r\n" + evento.fault.faultString, 'Atencion'); 
		}
 
		private function showMessg(msg:String, titulo:String):void
		{
			var msgPopUp:FaultInfoHandler =
				FaultInfoHandler(
					PopUpManager.createPopUp(
						Application.application as DisplayObject, 
						FaultInfoHandler, 
						true));
			msgPopUp.title = titulo;
			msgPopUp.txt.text = msg; 
			PopUpManager.centerPopUp(msgPopUp);
		}
		
		
		private function remove():void{
			for each(var addedProduct:ProductLigh in model.centralModel.productsToDistribute){
				for(var i:Number=0;i<model.centralModel.acGetProduct.length;i++){
					if(model.centralModel.acGetProduct.getItemAt(i).scan == addedProduct.scan){
						model.centralModel.acGetProduct.removeItemAt(i);
						break;
					}
				}
				
			}
		}
	}
}
