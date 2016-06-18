Shader "RageSpline/3D AA" {
	Properties {

	}

	Category {
		Tags {"RenderType"="Transparent" "Queue"="Transparent+1"}
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

