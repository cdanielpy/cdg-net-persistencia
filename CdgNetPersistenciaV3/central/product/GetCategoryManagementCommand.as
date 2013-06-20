package components.control.central.product
{
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.adobe.cairngorm.commands.ICommand;
	import mx.rpc.IResponder;
	import components.business.BusinessDelegate;
	import mx.controls.Alert;
	import components.vo.central.CategoryManagmentLight;
	import components.model.ModelLocator;

	public class GetCategoryManagementCommand implements ICommand, IResponder
	{
		[Bindable] public var model:ModelLocator = ModelLocator.getInstance();
		
		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate = new  BusinessDelegate(this);
			d.getCategoryManagement();
		}
		
		public function result(data:Object):void
		{
			var category:CategoryManagmentLight = CategoryManagmentLight(data.result);
			model.centralModel.categoryManagment.departments = category.departments;
			model.centralModel.categoryManagment.sections = category.sections;
			model.centralModel.categoryManagment.bigFamilies = category.bigFamilies;
			model.centralModel.categoryManagment.families = category.families;
			model.centralModel.categoryManagment.subFamilies = category.subFamilies;
		}
		
		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error al buscar las categorias.","Error.");
		}
		
	}
}