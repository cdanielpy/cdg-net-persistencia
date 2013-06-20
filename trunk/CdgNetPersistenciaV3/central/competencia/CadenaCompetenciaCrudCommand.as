 package components.control.central.competencia
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.competencia.CentralCadenaCompetenciaCrudEvent;
	import components.model.ModelLocator;
	import components.modules.central.competencia.sucursal.SucursalCompetenciaCrudPopUp;
	import components.vo.central.competencia.CadenaCompetenciaDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.core.UIComponent;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class CadenaCompetenciaCrudCommand implements ICommand, IResponder
	{	
		
		var modelo:ModelLocator = ModelLocator.getInstance();
	
		public var evento:CentralCadenaCompetenciaCrudEvent;
		
		//metodo de ejecucion de procesos del comando
		public function execute(event:CairngormEvent):void
		{
			// tomamos una copia del evento
			this.evento= event as CentralCadenaCompetenciaCrudEvent;
			
			//creamos una instancia del delegado local
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			// evaluamos la accion seleccionada
			switch(this.evento.accion){
				
				case CentralCadenaCompetenciaCrudEvent.AGREGAR:
					delegate.upsertCadenaCompetencia(modelo.centralModel.cadenaCompetenciaEditando);
					break;
				
				case CentralCadenaCompetenciaCrudEvent.EDITAR:
					delegate.upsertCadenaCompetencia(modelo.centralModel.cadenaCompetenciaEditando);
					break;
				
				default:
					// invocamos al metodo de recuperacion de lista de cadenas de competencias
					delegate.getListaCadenaCompetencia();
					break;
					
			}
			
			
		}
		
		//manejador de resultado exitoso
      	public function result(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:ResultEvent = data as ResultEvent;
			
			// evaluamos la accion ejecutando y llamamos al metodo correspondiente
			switch(this.evento.accion){
				case CentralCadenaCompetenciaCrudEvent.AGREGAR:
					this.resultadoUpsert(resultado.result as CadenaCompetenciaDto);
					break;
					
				case CentralCadenaCompetenciaCrudEvent.EDITAR:
					this.resultadoUpsert(resultado.result as CadenaCompetenciaDto);
					break;
					
				default:
					this.listarCadenas(resultado.result as ArrayCollection);
					break;
			}
			
		}
		
		// metodo manejador de listado de cadcenas de competencia
		private function listarCadenas(acCadenas:ArrayCollection):void{
			
			//this.evento.visualizador.dgDepoCrudDto.dataProvider = aCadenas;
			modelo.centralModel.acCadenasCompetencias = acCadenas;
			
			/*Alert.show("Cadenas de competencia recuperadas -> " + modelo.centralModel.acCadenasCompetencias.length.toString()
						,"Comando de Cadenas");*/
			
		}
		
		// metodo manejador de insercion/actualizacion de datos de cadena de competencia
		private function resultadoUpsert(cadenaResultado:CadenaCompetenciaDto):void{
			
			// pasamos el elemento devuelto a la instancia en memoria
			modelo.centralModel.cadenaCompetenciaEditando = cadenaResultado;
			
			switch(this.evento.accion){
				case CentralCadenaCompetenciaCrudEvent.AGREGAR:
					// agregamos el nuevo elemento a la lista en memoria
					modelo.centralModel.acCadenasCompetencias.addItem(cadenaResultado);
					modelo.centralModel.acCadenasCompetencias.refresh();
					
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Cadena Actualizados Correctamente","Comando de Cadenas");
					break; 
				
				case CentralCadenaCompetenciaCrudEvent.EDITAR:
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Cadena Actualizados Correctamente","Comando de Cadenas");
					break;
					
				case CentralCadenaCompetenciaCrudEvent.LISTAR:
					//Alert.show("Datos de Cadena listados Correctamente","Comando de Cadenas");
					break;
			}
		}
		
		// manejador de errores
		public function fault(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:FaultEvent = data as FaultEvent;
			Alert.show("Error recupertando cadenas de competencia:\n" 
						+ resultado.message
						,"Comando de Cadenas");
		}
	}
}