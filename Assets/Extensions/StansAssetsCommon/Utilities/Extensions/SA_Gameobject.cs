using UnityEngine;
using System.Collections;

public static class SA_Gameobject  {

	public static Sprite ToSprite(this Texture2D texture) {
		return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)); 
	}
}
