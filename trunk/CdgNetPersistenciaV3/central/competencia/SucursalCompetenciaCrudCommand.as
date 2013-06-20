package components.control.central.competencia
{
	import com.adobe.cairngorm.commands.ICommand;
	import com.adobe.cairngorm.control.CairngormEvent;
	
	import components.business.BusinessDelegate;
	import components.events.central.competencia.CentralSucursalCompetenciaCrudEvent;
	import components.model.ModelLocator;
	import components.vo.central.competencia.SucursalCompetenciaDto;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.rpc.IResponder;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;

	public class SucursalCompetenciaCrudCommand implements ICommand, IResponder
	{			 
		
		var modelo:ModelLocator = ModelLocator.getInstance();
		
		
		public var evento:CentralSucursalCompetenciaCrudEvent;
		
		
		//metodo de ejecucion de procesos del comando
		public function execute(event:CairngormEvent):void
		{
			// tomamos una copia del evento
			this.evento= event as CentralSucursalCompetenciaCrudEvent;
			
			//creamos una instancia del delegado local
			var delegate:BusinessDelegate = new BusinessDelegate(this);
			
			// evaluamos la accion seleccionada
			switch(this.evento.accion){
				
				case CentralSucursalCompetenciaCrudEvent.AGREGAR:
					delegate.upsertSucursalCompetencia(modelo.centralModel.sucursalCompetenciaEditando);
					break;
				
				case CentralSucursalCompetenciaCrudEvent.EDITAR:
					delegate.upsertSucursalCompetencia(modelo.centralModel.sucursalCompetenciaEditando);
					break;
				
				default:
					// invocamos al metodo de recuperacion de lista de cadenas de competencias
					delegate.getListaSucursalCompetencia();
					break;
					
			}
			
			
		}
		
		//manejador de resultado exitoso
      	public function result(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:ResultEvent = data as ResultEvent;
			
			/*Alert.show("Sucursales de competencia ACCION EJECUTADA -> " + this.evento.accion.toString()
						,"Comando de Sucursales Comp.");
			*/
			
			// evaluamos la accion ejecutando y llamamos al metodo correspondiente
			switch(this.evento.accion){
				case CentralSucursalCompetenciaCrudEvent.AGREGAR:
					this.resultadoUpsert(resultado.result as SucursalCompetenciaDto);
					break;
					
				case CentralSucursalCompetenciaCrudEvent.EDITAR:
					this.resultadoUpsert(resultado.result as SucursalCompetenciaDto);
					break;
					
				default:
					this.listarSucursalCompetencia(resultado.result as ArrayCollection);
					break;
			}
			
		}
		
		
		
		// metodo manejador de listado de cadcenas de competencia
		private function listarSucursalCompetencia(acSucursal:ArrayCollection):void{
			
			//this.evento.visualizador.dgDepoCrudDto.dataProvider = aCadenas;
			
			modelo.centralModel.acSucursalesCompetencia = acSucursal;
			
			/*Alert.show("Sucursales de competencia recuperadas -> " + modelo.centralModel.acSucursalesCompetencia.length.toString()
						,"Comando de Sucursales Comp.");*/
			
		}
		
		// metodo manejador de insercion/actualizacion de datos de cadena de competencia
		private function resultadoUpsert(cadenaResultado:SucursalCompetenciaDto):void{
			
			// pasamos el elemento devuelto a la instancia en memoria
			modelo.centralModel.sucursalCompetenciaEditando = cadenaResultado;
			
			switch(this.evento.accion){
				
				case CentralSucursalCompetenciaCrudEvent.AGREGAR:
					// agregamos el nuevo elemento a la lista en memoria
					modelo.centralModel.acSucursalesCompetencia.addItem(cadenaResultado);
					modelo.centralModel.acSucursalesCompetencia.refresh();
					
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Sucursal Actualizados Correctamente","Comando de Sucursal");
					break; 
				
				case CentralSucursalCompetenciaCrudEvent.EDITAR:
					// mensaje de notificacion de exito
			 		Alert.show("Datos de Sucursal Actualizados Correctamente","Comando de Sucursal");
					break;
					
				default:
					Alert.show("Datos de Sucursal listados Correctamente","Comando de Sucursal");
					break;
			}
			
		}
		
		// manejador de errores
		public function fault(data:Object):void
		{
			// casteamos el resultado a su tipo
			var resultado:FaultEvent = data as FaultEvent;
			Alert.show("Error recupertando Sucursal Competencia:\n" 
						+ resultado.message
						,"Comando de Cadenas");
		}
	}
}