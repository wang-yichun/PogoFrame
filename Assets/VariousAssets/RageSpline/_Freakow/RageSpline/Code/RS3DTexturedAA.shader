Shader "RageSpline/3D Textured AA" {
	Properties {
		_MainTex ("Texture1 (RGB)", 2D) = "white" {}
	}

	Category {
		Tags {"RenderType"="Transparent" "Queue"="Transparent+1"}
		Lighting Off
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "Texcoord", Texcoord
		}
		
		SubShader {
			Pass {
				ZWrite On
				Cull off
				Blend SrcAlpha OneMinusSrcAlpha
				SetTexture [_MainTex] {
					Combine texture * primary, primary
				}
			}
		}
	}
}
