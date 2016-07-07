using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCalc
{
	public static class NCalcExtend
	{
		public static Expression NCalc (this JToken jt)
		{
			string exp_str = jt.Value<string> ();
			exp_str = exp_str.Replace ("TRUE", "true");
			exp_str = exp_str.Replace ("True", "true");
			exp_str = exp_str.Replace ("FALSE", "false");
			exp_str = exp_str.Replace ("False", "false");
			return new Expression (exp_str);
		}
	}
}