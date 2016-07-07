////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


public abstract class SA_NonMonoSingleton<T>  where T : SA_NonMonoSingleton<T>, new() {

	private static T _Instance = null;
	
	public static T Instance {
		get {
			if (_Instance == null) {
				_Instance = new T();
			}

			return _Instance;
		}

	}
	

}
