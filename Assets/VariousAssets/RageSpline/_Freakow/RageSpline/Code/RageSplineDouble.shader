Shader "RageSpline/Basic (Double)" {
	Properties {

	}

	Category {
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Lighting Off
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
		}
		
		SubShader {
			Pass {
				ZWrite Off
				Cull off
				Blend SrcAlpha OneMinusSrcAlpha
				SetTexture [_MainTex] {
					Combine primary DOUBLE, primary
				}
			}
		}
	}
}
