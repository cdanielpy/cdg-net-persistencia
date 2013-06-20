package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.commands.ICommand;
	import mx.rpc.IResponder;
	import components.business.BusinessDelegate;
	import com.emeretail.emecrossapp.model.Model;
	import components.events.flow.SearchStockEvent;
	import components.model.ModelLocator;
	import mx.collections.ArrayCollection;
	import components.vo.flow.stock.Stock;
	import mx.rpc.events.ResultEvent;
	import components.vo.flow.stock.Product;
	import mx.controls.Alert;
	import components.views.utils.UpdateControll;
	import components.vo.flow.pager.StockResultPager;
	import mx.utils.ObjectUtil;
	import mx.rpc.events.FaultEvent;
	import components.events.crossapp.ShowErrorEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import components.events.flow.transfer.GetSotckByArticuloIDEvent;
	import components.events.central.product.GetCentralProductByArticleIDEvent;

	public class GetCentralProductByArticleIDCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		
		
		public function execute(event:CairngormEvent):void
		{
			//UpdateControll.getInstance().dispatchAgain(event);
			var d:BusinessDelegate = new  BusinessDelegate(this);
			var e:GetCentralProductByArticleIDEvent = GetCentralProductByArticleIDEvent(event);
			d.getCentralProductByArticleID(e.idArticle, e.idBranch);
		}
		
		public function result(event:Object):void
		{
			if (event.result != null){
				model.centralModel.product = new  Product(event.result);
				model.centralModel.validProd = true;
			}
			else
			{
				//TODO:Que se hace si no hay stock disponible
				var showErrorEvent:ShowErrorEvent = new ShowErrorEvent("Articulo ingresado no existe","Warning");
				CairngormEventDispatcher.getInstance().dispatchEvent(showErrorEvent);
				
			}
		}
		
		public function fault(event:Object):void
		{
			var showErrorEvent:ShowErrorEvent = new ShowErrorEvent("Se produjo un error al buscar datos","Error");
			CairngormEventDispatcher.getInstance().dispatchEvent(showErrorEvent);

		}
	}
}