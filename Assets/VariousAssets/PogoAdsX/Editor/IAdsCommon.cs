namespace pogorock
{
	using UnityEngine;
	using System.Collections;

	public interface IPogoAdsXCommon
	{
		/// <summary>
		/// 初始化
		/// </summary>
		void Init ();

		/// <summary>
		/// 广告已准备好
		/// </summary>
		/// <returns><c>true</c> 准备好了; 否则, <c>false</c>.</returns>
		bool IsReady ();

		/// <summary>
		/// 触发显示广告
		/// </summary>
		void Show ();
	}
}