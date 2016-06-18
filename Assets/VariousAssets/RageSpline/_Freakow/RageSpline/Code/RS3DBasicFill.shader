Shader "RageSpline/3D Basic Fill" {
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
				ZWrite On
				Cull Off
				Blend SrcAlpha OneMinusSrcAlpha
			}
		}
	}
}
