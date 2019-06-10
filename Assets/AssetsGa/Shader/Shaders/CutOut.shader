
Shader "Cecly/CutOut" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	Category 
	{
		Tags { "Queue"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha
    
		SubShader 
		{
			Pass 
			{
				SetTexture [_MainTex] 
			}
		}
	}
}