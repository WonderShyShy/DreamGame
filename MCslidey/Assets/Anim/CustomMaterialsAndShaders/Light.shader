Shader "Custom/Light" {
    Properties {
        //定义一个贴图
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {} 
    }
    
    SubShader 
    {       
        Tags
        {
            "RenderType" = "Transparent"
            "IGNOREPROJECTOR" = "TRUE"
            "QUEUE" = "Transparent"
        }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha One
            
            //给材质设置 贴图 
            SetTexture [_MainTex] 
            {
                combine texture * primary double
            }
        }
    }
    Fallback "Diffuse"
}