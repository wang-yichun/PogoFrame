namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;

	public enum PogoAdsxShowResult
	{
		Finished,
		Skipped,
		Failed
	}

	public interface IPogoAdsXCommon
	{
		string Key{ get; }

		string IOSAppId { get; }

		string AndroidAppId{ get; }

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

		/// <summary>
		/// 播放广告情况的回调
		/// </summary>
		/// <value>The show result callback.</value>
		Action<string, PogoAdsxShowResult> ShowResultCallback { get; }
	}
}