package components.control.central.user
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.control.CairngormEventDispatcher;
	import com.emeretail.emecrossapp.vo.Profile;
	
	import components.business.BusinessDelegate;
	import components.events.crossapp.ShowErrorEvent;
	import components.model.ModelLocator;
	
	import mx.collections.ArrayCollection;
	import mx.rpc.IResponder;
	import mx.rpc.events.ResultEvent;

	public class GetProfilesListCommand implements ICommand, IResponder
	{
		private var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			delegate.getProfilesList();
		}
		
		public function result(event:Object):void
		{
			var def:Profile = new Profile();
			def.id = -1;
			def.description = "Seleccione";
			var acProfiles:ArrayCollection = new ArrayCollection();
			acProfiles.addItem(def);
			for each (var item:Object in ResultEvent(event).result) {
				acProfiles.addItem(new Profile(item));
			}		

  			model.centralModel.acProfilesList = acProfiles;
		}
		
		public function fault(event:Object):void
		{
			var showErrorEvent:ShowErrorEvent = new ShowErrorEvent("Se produjo un error al buscar datos","Error");
			CairngormEventDispatcher.getInstance().dispatchEvent(showErrorEvent);
		}
		
	}
}