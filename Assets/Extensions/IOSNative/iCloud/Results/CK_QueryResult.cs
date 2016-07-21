using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CK_QueryResult : ISN_Result {


	//private List
	private CK_Database _Database;
	private List<CK_Record> _Records =  new List<CK_Record>();


	public CK_QueryResult(List<CK_Record> records):base(true) {
		_Records = records;
	}


	public CK_QueryResult(string errorData):base(errorData) {

	}

	public void SetDatabase(CK_Database database) {
		_Database = database;
	}



	public CK_Database Database {
		get {
			return _Database;
		}
	}

	public List<CK_Record> Records {
		get {
			return _Records;
		}
	}
}
