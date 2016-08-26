//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace AWSSDK.Examples
{
	public class LowLevelTableExample : DynamoDbBaseExample
	{

		public Button backButton;
		public Button createTableButton;
		public Button listTableButton;
		public Button updateTableButton;
		public Button describeTableButton;
		public Button deleteTableButton;
		public Text resultText;

		public Button exampleOp_1_Button;
		public Button exampleOp_2_Button;
		public Button exampleOp_3_Button;

		// Use this for initialization
		void Start ()
		{
			backButton.onClick.AddListener (BackListener);
			createTableButton.onClick.AddListener (CreateTableListener);
			listTableButton.onClick.AddListener (ListTableListener);
			updateTableButton.onClick.AddListener (UpdateTableListener);
			describeTableButton.onClick.AddListener (DescribeTableListener);
			deleteTableButton.onClick.AddListener (DeleteTableListener);

			exampleOp_1_Button.onClick.AddListener (ExampleOp_1);
			exampleOp_2_Button.onClick.AddListener (ExampleOp_2);
			exampleOp_3_Button.onClick.AddListener (ExampleOp_3);
		}

		void ExampleOp_1 ()
		{
			Debug.Log ("ExampleOp_1");

			UpdateItemRequest request = new UpdateItemRequest () {
				Key = new Dictionary<string, AttributeValue> () {
					{ "Id", new AttributeValue{ N = "101" } }
				},
				ExpressionAttributeNames = new Dictionary<string, string> () {
//					{ "#I", "ISBN" },
//					{ "#A", "Authors" },
//					{ "#P", "PlayFabIds" },
					{ "#PR", "Price" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue> () {
//					{ ":n_isbn", new AttributeValue{ S = "123-1000000000" } },
//					{ ":n_playfabids", new AttributeValue { L = new List<AttributeValue> (){ new AttributeValue{ S = "pf_004" } } } }
//					{ ":auth", new AttributeValue { SS = { "Author xx", "Author ZZ" } } }
					{ ":Pr_one", new AttributeValue{ N = "1" } }
				},
//				UpdateExpression = "SET #I = :n_isbn",
//				UpdateExpression = "SET #P = :n_playfabids",
//				UpdateExpression = "ADD #A :auth",
				UpdateExpression = "SET #PR = #PR + :Pr_one",
				TableName = "ProductCatalog",
				ReturnValues = "ALL_NEW"
			};

			Client.UpdateItemAsync (request, (responseObject) => {
				Debug.Log ("ContentLength: " + responseObject.Response.ContentLength);
				if (responseObject.Exception != null) {
					Debug.Log ("responseObject: " + responseObject.Exception.Message);
				}
			});
		}

		void ExampleOp_2 ()
		{
			Debug.Log ("ExampleOp_2");
			GetItemRequest request = new GetItemRequest {
				TableName = "StageInfo",
				Key = new Dictionary<string, AttributeValue> () {
					{ "playfab_id", new AttributeValue { S = "6ACA5F711FD85CA4" } },
					{ "stage_id", new AttributeValue { S = "level_001" } }
				},
//				ProjectionExpression = "playfab_id, stage_id, stage_status",
				ConsistentRead = true
			};
			Client.GetItemAsync (request, (responseObject) => {
				if (responseObject.Exception != null) {
					Debug.Log (responseObject.Exception.Message);
				}

				var attributeList = responseObject.Response.Item; // attribute list in the response.
//				Debug.Log ("\nPrinting item after retrieving it ............");
				Debug.Log (JsonConvert.SerializeObject (attributeList, Formatting.Indented));
//				PrintItem (attributeList);
			});
		}

		void ExampleOp_3 ()
		{
			Debug.Log ("ExampleOp_3");
			UpdateItemRequest request = new UpdateItemRequest () {
				Key = new Dictionary<string, AttributeValue> () {
					{ "playfab_id", new AttributeValue { S = "6ACA5F711FD85CA4" } },
					{ "stage_id", new AttributeValue { S = "level_001" } }
				},
				ExpressionAttributeNames = new Dictionary<string, string> () {
					{ "#L", "like" },
					{ "#S3K", "replay_s3_key" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue> () {
					{ ":inc", new AttributeValue { N = "1" } },
					{ ":key", new AttributeValue { S = "user2/6ACA5F711FD85CA4/rec/level_001/2d5b83ad-3cb5-455c-9ea9-6f1132770438" } }
				},
				UpdateExpression = "SET stat_info[0].#L = stat_info[0].#L + :inc",
				ConditionExpression = "stat_info[0].#S3K = :key",
				TableName = "StageInfo",
				ReturnValues = "ALL_NEW"
			};

			Client.UpdateItemAsync (request, (responseObject) => {
				if (responseObject.Exception != null) {
					Debug.Log (responseObject.Exception.Message);
				}
				Debug.Log ("ContentLength: " + responseObject.Response.ContentLength);
			});
		}

		private static void PrintItem (Dictionary<string, AttributeValue> attributeList)
		{
			foreach (KeyValuePair<string, AttributeValue> kvp in attributeList) {
				string attributeName = kvp.Key;
				AttributeValue value = kvp.Value;

				Debug.Log (
					attributeName + " " +
					(value.S == null ? "" : "S=[" + value.S + "]") +
					(value.N == null ? "" : "N=[" + value.N + "]") +
					(value.SS == null ? "" : "SS=[" + string.Join (",", value.SS.ToArray ()) + "]") +
					(value.NS == null ? "" : "NS=[" + string.Join (",", value.NS.ToArray ()) + "]")
				);
			}
			Debug.Log ("************************************************");
		}

		void CreateTableListener ()
		{
			resultText.text = @"\n Creating table";

			var productCatalogTableRequest = new CreateTableRequest {
				AttributeDefinitions = new List<AttributeDefinition> () {
					new AttributeDefinition {
						AttributeName = "Id",
						AttributeType = "N"
					}
				},
				KeySchema = new List<KeySchemaElement> {
					new KeySchemaElement {
						AttributeName = "Id",
						KeyType = "HASH"
					}
				},
				ProvisionedThroughput = new ProvisionedThroughput {
					ReadCapacityUnits = 10,
					WriteCapacityUnits = 5
				},
				TableName = "ProductCatalog"
			};

			Client.CreateTableAsync (productCatalogTableRequest, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var tableDescription = result.Response.TableDescription;
				resultText.text += String.Format ("Created {1}: {0}\nReadsPerSec: {2} \nWritesPerSec: {3}\n",
					tableDescription.TableStatus,
					tableDescription.TableName,
					tableDescription.ProvisionedThroughput.ReadCapacityUnits,
					tableDescription.ProvisionedThroughput.WriteCapacityUnits);
				resultText.text += (result.Request.TableName + "-" + tableDescription.TableStatus + "\n");
				resultText.text += ("Allow a few seconds for changes to reflect...");
			});


			var forumTableRequest = new CreateTableRequest {
				AttributeDefinitions = new List<AttributeDefinition> () {
					new AttributeDefinition {
						AttributeName = "Name",
						AttributeType = "S"
					}
				},
				KeySchema = new List<KeySchemaElement> {
					new KeySchemaElement {
						AttributeName = "Name",
						KeyType = "HASH"
					}
				},
				ProvisionedThroughput = new ProvisionedThroughput {
					ReadCapacityUnits = 10,
					WriteCapacityUnits = 5
				},
				TableName = "Forum"
			};

			Client.CreateTableAsync (forumTableRequest, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var tableDescription = result.Response.TableDescription;
				resultText.text += String.Format ("Created {1}: {0}\nReadsPerSec: {2} \nWritesPerSec: {3}\n",
					tableDescription.TableStatus,
					tableDescription.TableName,
					tableDescription.ProvisionedThroughput.ReadCapacityUnits,
					tableDescription.ProvisionedThroughput.WriteCapacityUnits);
				resultText.text += (result.Request.TableName + "-" + tableDescription.TableStatus + "\n");
				resultText.text += ("Allow a few seconds for changes to reflect...");
			});

			var threadTableRequest = new CreateTableRequest {
				AttributeDefinitions = new List<AttributeDefinition> () {
					new AttributeDefinition {
						AttributeName = "ForumName",
						AttributeType = "S"
					},
					new AttributeDefinition {
						AttributeName = "Subject",
						AttributeType = "S"
					}
				},
				KeySchema = new List<KeySchemaElement> {
					new KeySchemaElement {
						AttributeName = "ForumName",
						KeyType = "HASH"
					},
					new KeySchemaElement {
						AttributeName = "Subject",
						KeyType = "RANGE"
					}
				},
				ProvisionedThroughput = new ProvisionedThroughput {
					ReadCapacityUnits = 10,
					WriteCapacityUnits = 5
				},
				TableName = "Thread"
			};

			Client.CreateTableAsync (threadTableRequest, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var tableDescription = result.Response.TableDescription;
				resultText.text += String.Format ("Created {1}: {0}\nReadsPerSec: {2} \nWritesPerSec: {3}\n",
					tableDescription.TableStatus,
					tableDescription.TableName,
					tableDescription.ProvisionedThroughput.ReadCapacityUnits,
					tableDescription.ProvisionedThroughput.WriteCapacityUnits);
				resultText.text += (result.Request.TableName + "-" + tableDescription.TableStatus + "\n");
				resultText.text += ("Allow a few seconds for changes to reflect...");
			});

			var replyTableRequest = new CreateTableRequest {
				AttributeDefinitions = new List<AttributeDefinition> () {
					new AttributeDefinition {
						AttributeName = "Id",
						AttributeType = "S"
					},
					new AttributeDefinition {
						AttributeName = "ReplyDateTime",
						AttributeType = "S"
					},
					new AttributeDefinition {
						AttributeName = "PostedBy",
						AttributeType = "S"
					}
				},
				KeySchema = new List<KeySchemaElement> {
					new KeySchemaElement {
						AttributeName = "Id",
						KeyType = KeyType.HASH
					},
					new KeySchemaElement {
						AttributeName = "ReplyDateTime",
						KeyType = KeyType.RANGE
					}
				},
				ProvisionedThroughput = new ProvisionedThroughput {
					ReadCapacityUnits = 10,
					WriteCapacityUnits = 5
				},
				LocalSecondaryIndexes = new List<LocalSecondaryIndex> () {
					new LocalSecondaryIndex () {
						IndexName = "PostedBy-index",
						KeySchema = new List<KeySchemaElement> () {
							new KeySchemaElement () {
								AttributeName = "Id",
								KeyType = KeyType.HASH
							},
							new KeySchemaElement {
								AttributeName = "PostedBy",
								KeyType = KeyType.RANGE
							}
						},
						Projection = new Projection () {
							ProjectionType = ProjectionType.KEYS_ONLY
						}
					}
				},
				TableName = "Reply"
			};

			Client.CreateTableAsync (replyTableRequest, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var tableDescription = result.Response.TableDescription;
				resultText.text += String.Format ("Created {1}: {0}\nReadsPerSec: {2} \nWritesPerSec: {3}\n",
					tableDescription.TableStatus,
					tableDescription.TableName,
					tableDescription.ProvisionedThroughput.ReadCapacityUnits,
					tableDescription.ProvisionedThroughput.WriteCapacityUnits);
				resultText.text += (result.Request.TableName + "-" + tableDescription.TableStatus + "\n");
				resultText.text += ("Allow a few seconds for changes to reflect...");
			});
		}

		void ListTableListener ()
		{
			resultText.text = "\n*** listing tables ***";
			string lastTableNameEvaluated = null;

			var request = new ListTablesRequest {
				Limit = 2,
				ExclusiveStartTableName = lastTableNameEvaluated
			};

			Client.ListTablesAsync (request, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}

				resultText.text += "ListTable response : \n";
				var response = result.Response;
				foreach (string name in response.TableNames)
					resultText.text += name + "\n";

				// repeat request to fetch more results
				lastTableNameEvaluated = response.LastEvaluatedTableName;
			});
		}

		void UpdateTableListener ()
		{
			resultText.text = ("\n*** Updating table ***\n");
			var request = new UpdateTableRequest () {
				TableName = @"ProductCatalog",
				ProvisionedThroughput = new ProvisionedThroughput () {
					ReadCapacityUnits = 10,
					WriteCapacityUnits = 10
				}
			};
			Client.UpdateTableAsync (request, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var response = result.Response;
				var table = response.TableDescription;
				resultText.text += ("Table " + table.TableName + " Updated ! \n Allow a few seconds to reflect !");
			});
		}

		void DescribeTableListener ()
		{
			resultText.text = ("\n*** Retrieving table information ***\n");
			var request = new DescribeTableRequest {
				TableName = @"ProductCatalog"
			};
			Client.DescribeTableAsync (request, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					Debug.Log (result.Exception);
					return;
				}
				var response = result.Response;
				TableDescription description = response.Table;
				resultText.text += ("Name: " + description.TableName + "\n");
				resultText.text += ("# of items: " + description.ItemCount + "\n");
				resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.ReadCapacityUnits + "\n");
				resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.WriteCapacityUnits + "\n");

			}, null);
		}

		void DeleteTableListener ()
		{
			resultText.text = ("\n*** Deleting table ***\n");
			var request = new DeleteTableRequest {
				TableName = @"ProductCatalog"
			};
			Client.DeleteTableAsync (request, (result) => {
				if (result.Exception != null) {
					resultText.text += result.Exception.Message;
					return;
				}
				var response = result.Response;

				resultText.text += ("Table " + response.TableDescription.TableName + " is being deleted...!");
			});
		}
	}
}
