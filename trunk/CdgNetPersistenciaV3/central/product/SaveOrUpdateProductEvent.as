package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.control.Controller;
	import components.vo.flow.stock.Product;
	
	import flash.display.DisplayObject;

	public class SaveOrUpdateProductEvent extends CairngormEvent
	{
		public static const ACTUALIZACION_MASIVA:String = "ACTUALIZACION_MASIVA";
		
		public var product:Product;
		public var cTipo:String = "";
		public var oVisualizador:DisplayObject;
		
		public function SaveOrUpdateProductEvent(pruduct:Product)
		{
			super(Controller.EVENT_SAVE_OR_UPDATE_PRODUCT);
			this.product = pruduct;
		}
	}
}