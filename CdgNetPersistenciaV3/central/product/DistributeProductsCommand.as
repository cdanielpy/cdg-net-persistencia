package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.emeretail.emecrossapp.utils.export.GenericAlertPanel;
	import com.jicm.controls.FaultInfoHandler;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.DistributeProductsEvent;
	import components.model.ModelLocator;
	
	import flash.display.DisplayObject;
	
	import mx.core.Application;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class DistributeProductsCommand implements ICommand, IResponder
	{
		public function execute(event:CairngormEvent):void
		{
			var bd:BusinessDelegate=new BusinessDelegate(this);
			var ev:DistributeProductsEvent=DistributeProductsEvent(event);
			bd.distributeProducts(ev.idProductos, ev.idsLocales, ev.commonInformation);
		}

		public function result(data:Object):void
		{
			var comp2:GenericAlertPanel=GenericAlertPanel(PopUpManager.createPopUp(DisplayObject(Application.application), GenericAlertPanel, false));
			comp2.textComp.text="Se han asignado correctamente los articulos";
			comp2.styleName='panelCentral';
		}

		public function fault(event:Object):void
		{
			var evento:FaultEvent=event as FaultEvent;
			var faultPopUp:FaultInfoHandler =
				
				FaultInfoHandler(
					PopUpManager.createPopUp(ModelLocator.getInstance().crossapp.display, FaultInfoHandler, true));
			faultPopUp.txt.text = evento.fault.rootCause + "\r\n " + 
								  "Detalles: \r\n "  + evento.fault.faultString; 
			PopUpManager.centerPopUp(faultPopUp);
		}

	}
}