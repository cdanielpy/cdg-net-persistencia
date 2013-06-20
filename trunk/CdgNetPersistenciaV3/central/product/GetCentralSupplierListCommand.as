package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import mx.rpc.IResponder;
	import com.adobe.cairngorm.commands.ICommand;
	import components.model.ModelLocator;
	import components.business.BusinessDelegate;
	import components.events.central.product.GetCentralSupplierListEvent;
	import components.events.crossapp.ShowErrorEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import mx.collections.ArrayCollection;
	import components.vo.crossapp.Supplier;

	public class GetCentralSupplierListCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			var e:GetCentralSupplierListEvent = GetCentralSupplierListEvent(event);
				d.GetCentralSupplierList(e.idBranch);

		}
		
		public function result(event:Object):void
		{
			model.centralModel.acCentralSuppliers = event.result;
		}
		
		public function fault(event:Object):void
		{
			var showErrorEvent:ShowErrorEvent = new ShowErrorEvent("Se produjo un error al buscar datos","Error");
			CairngormEventDispatcher.getInstance().dispatchEvent(showErrorEvent);
		}
		
	}
		
}