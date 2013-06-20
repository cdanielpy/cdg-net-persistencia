package components.control.central.user
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.user.SaveOrUpdateUserEvent;
	import components.model.ModelLocator;
	import components.modules.central.users.PopUpDuplicateUser;
	import com.emeretail.emecrossapp.vo.User;
	
	import mx.controls.Alert;
	import mx.managers.PopUpManager;
	import mx.rpc.IResponder;
	import mx.rpc.events.ResultEvent;
	import mx.core.IFlexDisplayObject;
	import mx.core.Application;
	import flash.display.DisplayObject;
	import components.events.central.user.GetUsersEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;

	public class SaveOrUpdateUserCommand implements ICommand, IResponder
	{
		private var model : ModelLocator = ModelLocator.getInstance();
		private var serviceName:String;
		//private var modelType:String;
		
		public function execute(event:CairngormEvent):void
		{
			var ev:SaveOrUpdateUserEvent = SaveOrUpdateUserEvent(event);
			//modelType = ev.modelType;
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			var usr:User = ev.user;
			usr.id = 0;
			usr.ttl = 0;
			usr.sessionID = 0;
			delegate.saveOrUpdateUser(usr, ev.status);
		}
		
		public function result(event:Object):void
		{
			var e:ResultEvent = ResultEvent(event);
			if (-1 == event.result)
			{
				model.centralModel.callPopUpDuplicateUser = true;
				var popUp:IFlexDisplayObject = PopUpManager.createPopUp(Application.application as DisplayObject,PopUpDuplicateUser);
				PopUpManager.centerPopUp(popUp);
			}else{
				var ev:GetUsersEvent = new GetUsersEvent();
				CairngormEventDispatcher.getInstance().dispatchEvent(ev);
				//model.centralModel.acUsers.refresh();
			}
				

		}
		
		public function fault(event:Object):void
		{
			Alert.show("No se pudo crear o actualizar el usuario","Error.");
		}
		
	}
}