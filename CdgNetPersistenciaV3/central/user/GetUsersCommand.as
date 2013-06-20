package components.control.central.user
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import com.emeretail.emecrossapp.vo.User;
	
	import components.business.BusinessDelegate;
	import components.events.central.user.GetUsersEvent;
	import components.events.crossapp.ShowErrorEvent;
	import components.model.ModelLocator;
	
	import mx.collections.ArrayCollection;
	import mx.rpc.IResponder;
	import mx.rpc.events.ResultEvent;

	public class GetUsersCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.getUsers();
		}
		
		public function result(event:Object):void
		{
/* 			var arrUsers:ArrayCollection = new ArrayCollection();
			for each (var item:Object in ResultEvent(event).result) {
				arrUsers.addItem(new User(item));
			}
 */			
			model.centralModel.acUsers = event.result;
			model.centralModel.acUsers.refresh();		
			
		}
		
		public function fault(event:Object):void
		{
			var showErrorEvent:ShowErrorEvent = new ShowErrorEvent("Se produjo un error al buscar datos","Error");
			CairngormEventDispatcher.getInstance().dispatchEvent(showErrorEvent);
		}
		
	}
}