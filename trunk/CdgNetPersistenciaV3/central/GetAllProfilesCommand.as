package components.control.central
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.emeretail.emecrossapp.vo.Profile;
	
	import components.business.BusinessDelegate;
	import components.model.ModelLocator;
	
	import mx.controls.Alert;
	import mx.rpc.IResponder;

	public class GetAllProfilesCommand implements ICommand, IResponder
	{	
		private var model:ModelLocator = ModelLocator.getInstance();

		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new BusinessDelegate(this);
			d.getAllProfiles();
		}
		
		public function result(data:Object):void
		{
			var p:Profile = new Profile();
			p.description = "Todos";
			p.id = -1;
			data.result.addItemAt(p,0);
			model.centralModel.acUsersProfiles = data.result;
		
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Error trayendo todos los perfiles");
		}
		
	}
}