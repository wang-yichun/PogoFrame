using UnityEngine;
using System.Collections;

public class CloudKitUseExample : BaseIOSFeaturePreview {





	void OnGUI() {

		UpdateToStartPos();

		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Cloud Kit", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Create Record")) {

			CK_RecordID recordId =  new CK_RecordID("1");

			CK_Record record =  new CK_Record(recordId, "Post");
			record.SetObject("PostText", "Sample point of interest");
			record.SetObject("PostTitle", "My favorite point of interest");


			CK_Database database = ISN_CloudKit.Instance.PrivateDB;
			database.SaveRecrod(record);

			database.ActionRecordSaved += Database_ActionRecordSaved;
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Delete Record")) {
			CK_RecordID recordId =  new CK_RecordID("1");
			CK_Database database = ISN_CloudKit.Instance.PrivateDB;

			database.DeleteRecordWithID(recordId);
			database.ActionRecordDeleted += Database_ActionRecordDeleted;

		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Fetch Record")) {
			CK_RecordID recordId =  new CK_RecordID("1");
			CK_Database database = ISN_CloudKit.Instance.PrivateDB;

			database.FetchRecordWithID(recordId);
			database.ActionRecordFetchComplete += Database_ActionRecordFetchComplete;
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Fetch And Update")) {
			CK_RecordID recordId =  new CK_RecordID("1");
			CK_Database database = ISN_CloudKit.Instance.PrivateDB;

			database.FetchRecordWithID(recordId);
			database.ActionRecordFetchComplete += Database_ActionRecordFetchForUpdateComplete;
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Query")) {
			CK_Database database = ISN_CloudKit.Instance.PrivateDB;
			CK_Query query =  new CK_Query("TRUEPREDICATE", "Post");


			database.ActionQueryComplete += Database_ActionQueryComplete;
			database.PerformQuery(query);

		}


	}



	void Database_ActionQueryComplete (CK_QueryResult res) {
		if(res.IsSucceeded) {
			ISN_Logger.Log("Database_ActionQueryComplete, total records found: " + res.Records.Count);
			foreach(CK_Record r  in res.Records) {
				Debug.Log(r.Id.Name);
				ISN_Logger.Log("Post Title: "  + r.GetObject("PostTitle"));
			}
		} else {
			ISN_Logger.Log("Database_ActionRecordFetchComplete, Error: " + res.Error.Code + " / " + res.Error.Description);
		}
	}

	void Database_ActionRecordFetchComplete (CK_RecordResult res) {
		res.Database.ActionRecordFetchComplete -= Database_ActionRecordFetchComplete;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Database_ActionRecordFetchComplete:");
			ISN_Logger.Log("Post Title: "  + res.Record.GetObject("PostTitle"));
		} else {
			ISN_Logger.Log("Database_ActionRecordFetchComplete, Error: " + res.Error.Code + " / " + res.Error.Description);
		}
	}


	void Database_ActionRecordFetchForUpdateComplete (CK_RecordResult res) {
		res.Database.ActionRecordFetchComplete -= Database_ActionRecordFetchForUpdateComplete;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Database_ActionRecordFetchComplete:");
			ISN_Logger.Log("Post Title: "  + res.Record.GetObject("PostTitle"));
			ISN_Logger.Log("Updatting Title: ");

			CK_Record record = res.Record;
			record.SetObject("PostTitle", "My favorite point of interest - updated");

			ISN_CloudKit.Instance.PrivateDB.SaveRecrod(record);
			ISN_CloudKit.Instance.PrivateDB.ActionRecordSaved += Database_ActionRecordSaved;


		} else {
			ISN_Logger.Log("Database_ActionRecordFetchComplete, Error: " + res.Error.Code + " / " + res.Error.Description);
		}
	}


	void Database_ActionRecordDeleted (CK_RecordDeleteResult res) {
		res.Database.ActionRecordDeleted -= Database_ActionRecordDeleted;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Database_ActionRecordDeleted, Success: ");
		} else {
			ISN_Logger.Log("Database_ActionRecordDeleted, Error: " + res.Error.Code + " / " + res.Error.Description);
		}
	}

	void Database_ActionRecordSaved (CK_RecordResult res) {

		res.Database.ActionRecordSaved -= Database_ActionRecordSaved;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Database_ActionRecordSaved:");
			ISN_Logger.Log("Post Title: "  + res.Record.GetObject("PostTitle"));
		} else {
			ISN_Logger.Log("Database_ActionRecordSaved, Error: " + res.Error.Code + " / " + res.Error.Description);
		}
	}

}
