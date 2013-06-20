package components.control.central.product
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	import com.emeretail.emecrossapp.vo.DomainObject;
	
	import components.business.BusinessDelegate;
	import components.events.central.product.LoadProductEvent;
	import components.model.ModelLocator;
	import components.vo.central.BranchProperty;
	import components.vo.pricing.SupplierLight;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;

	public class LoadProductCommand implements ICommand, IResponder
	{
		[Bindable]
		public var model:ModelLocator=ModelLocator.getInstance();

		public function execute(event:CairngormEvent):void
		{
			var d:BusinessDelegate=new BusinessDelegate(this);
			var evet:LoadProductEvent=LoadProductEvent(event);
			d.loadProduct(evet.id);
		}

		public function result(data:Object):void
		{
			if (data.result != null)
			{
				model.centralModel.loadProduct=data.result;
				
				model.scanList = model.centralModel.loadProduct.basicData.scan.toArray();

				/*-----------------------------------------------------------------------------------*/
				/*									UNIDAD DE MEDIDA								 */
				/*-----------------------------------------------------------------------------------*/
				for (var i:Number=0; i < model.centralModel.acMeasurementUnits.length; i++)
				{
					//model.centralModel.acMeasurementUnits
					if (model.centralModel.acMeasurementUnits.getItemAt(i).id == 
						model.centralModel.loadProduct.basicData.unitMeasurement)
					{
						/*Alert.show(" model.centralModel.loadProduct.basicData.unitMeasurement "+
									model.centralModel.loadProduct.basicData.unitMeasurement);*/
						model.centralModel.selectIndexMeasurementUnit=i;
						break;
					}
				}
				/*-----------------------------------------------------------------------------------*/


				/*-----------------------------------------------------------------------------------*/
				/*										SICS DATA									 */
				/*-----------------------------------------------------------------------------------*/
				for (var i0:Number=0; i0 < model.centralModel.marcaList.length; i0++)
				{
					if (model.centralModel.marcaList.getItemAt(i0).id == 
						model.centralModel.loadProduct.basicData.marca)
					{
						model.centralModel.selectIndexMarca=i0;
						break;
					}
				}
				
				for (var ii:Number=0; ii < model.centralModel.envaseList.length; ii++)
				{
					if (model.centralModel.envaseList.getItemAt(ii).id == 
						model.centralModel.loadProduct.basicData.envase)
					{
						model.centralModel.selectIndexEnvase=ii;
						break;
					}
				}

				for (var iii:Number=0; iii < model.centralModel.procedenciaList.length; iii++)
				{
					if (model.centralModel.procedenciaList.getItemAt(iii).id == 
						model.centralModel.loadProduct.basicData.procedencia)
					{
						model.centralModel.selectIndexProcedencia=iii;
						break;
					}
				}

				for (var iiii:Number=0; iiii < model.centralModel.clarList.length; iiii++)
				{
					if (model.centralModel.clarList.getItemAt(iiii).id == 
						model.centralModel.loadProduct.basicData.categoria)
					{
						model.centralModel.selectIndexCategoria=iiii;
						break;
					}
				}

				if (model.centralModel.loadProduct.basicData.granel!=null) {				
					if (model.centralModel.loadProduct.basicData.granel=='SI'){
						model.centralModel.selectIndexGranel=0;
					}else{
						model.centralModel.selectIndexGranel=1;
					}
				}else{
					model.centralModel.selectIndexGranel=-1;
				}	
				/*-----------------------------------------------------------------------------------*/


				for (var indx:Number=0; indx < model.pricing.purchase.taxes.length; indx++)
				{
					if (model.pricing.purchase.taxes.getItemAt(indx).percent == model.centralModel.loadProduct.tax.alicuota)
					{
						model.centralModel.selectAlicuota=indx;
						break;
					}
				}

				model.pricing.suppliers.removeAll();
				for each (var item:SupplierLight in model.centralModel.loadProduct.basicData.suppliers)
				{
					item.check=true;
					model.pricing.suppliers.addItem(item);
					model.centralModel.acSupplier.put(item.id, item);
				}

				var t:String;
				var t2:String;
				var skuToSearch:String;
				var skuToSearchTemp:String;
				for each (var dom:Object in model.centralModel.categoryManagment.departments)
				{
					var domain:DomainObject=new DomainObject(dom);
					//if(model.centralModel.loadProduct.basicData.sku.toString().substr(0,1)==domain.id.toString().substr(0,1)){
					if (model.centralModel.loadProduct.basicData.sku.toString().charAt(0) == domain.id.toString().charAt(0))
					{
						//Debug
						t=domain.id.toString().charAt(0);
						t2=model.centralModel.loadProduct.basicData.sku.toString().charAt(0);
						skuToSearch=model.centralModel.loadProduct.basicData.sku.toString().charAt(0);
						model.centralModel.cbDepto=model.centralModel.categoryManagment.departments.getItemIndex(dom);
					}
				}

				skuToSearchTemp=skuToSearch;
				for each (var dom2:Object in model.centralModel.categoryManagment.sections)
				{
					var domain:DomainObject=new DomainObject(dom2);
					skuToSearchTemp=skuToSearch + domain.id.toString().charAt(1);
					if (model.centralModel.loadProduct.basicData.sku.toString().slice(0, 2) == skuToSearchTemp)
					{
						t=domain.id.toString().charAt(1);
						t2=model.centralModel.loadProduct.basicData.sku.toString().slice(0, 2);
						skuToSearch=skuToSearch + domain.id.toString().charAt(1);
						model.centralModel.cbSeccion=skuHelper(model.centralModel.categoryManagment.sections, 2, skuToSearch);
						break;
					}
					else
					{
						skuToSearchTemp="";
					}
				}

				skuToSearchTemp=skuToSearch;
				for each (var dom3:Object in model.centralModel.categoryManagment.bigFamilies)
				{
					var domain:DomainObject=new DomainObject(dom3);
					skuToSearchTemp=skuToSearch + domain.id.toString().charAt(2);
					if (model.centralModel.loadProduct.basicData.sku.toString().slice(0, 3) == skuToSearchTemp)
					{
						t=domain.id.toString().charAt(2);
						t2=model.centralModel.loadProduct.basicData.sku.toString().slice(0, 3);
						skuToSearch=skuToSearch + model.centralModel.loadProduct.basicData.sku.toString().charAt(2);
						model.centralModel.cbBigFamily=skuHelper(model.centralModel.categoryManagment.bigFamilies, 3, skuToSearch);
						break;
					}
					else
					{
						skuToSearchTemp="";
					}
				}
				skuToSearchTemp=skuToSearch;
				for each (var dom4:Object in model.centralModel.categoryManagment.families)
				{
					var domain:DomainObject=new DomainObject(dom4);
					skuToSearchTemp=skuToSearch + domain.id.toString().charAt(3);
					if (model.centralModel.loadProduct.basicData.sku.toString().slice(0, 4) == skuToSearchTemp)
					{
						t=domain.id.toString().charAt(3);
						t2=model.centralModel.loadProduct.basicData.sku.toString().slice(0, 4);
						skuToSearch=skuToSearch + domain.id.toString().charAt(3);
						model.centralModel.cbFamily=skuHelper(model.centralModel.categoryManagment.families, 4, skuToSearch);
						break;
					}
					else
					{
						skuToSearchTemp="";
					}
				}
				skuToSearchTemp=skuToSearch;
				for each (var dom5:Object in model.centralModel.categoryManagment.subFamilies)
				{
					var domain:DomainObject=new DomainObject(dom5);
					skuToSearchTemp=skuToSearch + domain.id.toString().slice(4, 6);
					if (model.centralModel.loadProduct.basicData.sku.toString().slice(0, 6) == skuToSearchTemp)
					{
						t=domain.id.toString().slice(0, 6);
						t2=model.centralModel.loadProduct.basicData.sku.toString().slice(0, 6);
						skuToSearch=skuToSearch + domain.id.toString().slice(4, 6);
						model.centralModel.cbSubFamily=skuHelper(model.centralModel.categoryManagment.subFamilies, 6, skuToSearch);
						break;
					}
				}

				model.centralModel.acLocales=model.centralModel.loadProduct.scope.acBranch;

				model.centralModel.attributes.itemInPackDescription=model.centralModel.loadProduct.atribuite.itemInPackDescription;
				model.centralModel.attributes.itemInPackId=model.centralModel.loadProduct.atribuite.itemInPackId;
				model.centralModel.attributes.itemInPackPlu=model.centralModel.loadProduct.atribuite.itemInPackPlu;
				model.centralModel.attributes.packId=model.centralModel.loadProduct.atribuite.packId;
				model.centralModel.attributes.packQuantity=model.centralModel.loadProduct.atribuite.packQuantity;
				if (model.centralModel.attributes.packId > 0)
				{
					model.centralModel.loadProduct.atribuite.isPack=true;
				}

				for each (var itemCheck:BranchProperty in model.centralModel.loadProduct.scope.acBranch)
				{
					if (model.centralModel.acBranch.getValue(itemCheck.id) != null)
					{
						var branchProperty:BranchProperty=model.centralModel.acBranch.getValue(itemCheck.id);
						branchProperty.idBranch=itemCheck.idBranch;
						branchProperty.uxb=itemCheck.uxb;
						branchProperty.stockMax=itemCheck.stockMax;
						branchProperty.stockMin=itemCheck.stockMin;
						branchProperty.criteriaReplenishment=0;
						branchProperty.tipeReposition=itemCheck.tipeReposition;

						model.centralModel.acBranch.put(itemCheck.id, branchProperty);
					}
					else
					{
						var branchProperty:BranchProperty=new BranchProperty();
						branchProperty.idBranch=itemCheck.idBranch;
						branchProperty.uxb=itemCheck.uxb;
						branchProperty.stockMax=itemCheck.stockMax;
						branchProperty.stockMin=itemCheck.stockMin;
						branchProperty.criteriaReplenishment=0;
						branchProperty.tipeReposition=itemCheck.tipeReposition;

						model.centralModel.acBranch.put(itemCheck.id, branchProperty);
					}
				}
			}
			else
			{
				Alert.show("No se encontraron datos.", "Error.");
			}
		}

		public function fault(info:Object):void
		{
			Alert.show("Se produjo un error al buscar el producto: \n"
						+ (info as FaultEvent).message
						, "Error.");
		}



		private function skuHelper(arr:ArrayCollection, offset:Number, skuToFind:String):Number
		{
			var obj:Object;
			for (var i:Number=0; i < arr.length; i++)
			{
				obj=arr.getItemAt(i);
				if (obj.id.toString().slice(0, offset) == skuToFind.slice(0, offset))
				{
					return i;
				}
			}
			return null;
		}

	}
}