xof 0303txt 0032
template XSkinMeshHeader {
 <3cf169ce-ff7c-44ab-93c0-f78f62d172e2>
 WORD nMaxSkinWeightsPerVertex;
 WORD nMaxSkinWeightsPerFace;
 WORD nBones;
}

template VertexDuplicationIndices {
 <b8d65549-d7c9-4995-89cf-53a9a8b031e3>
 DWORD nIndices;
 DWORD nOriginalVertices;
 array DWORD indices[nIndices];
}

template SkinWeights {
 <6f0d123b-bad2-4167-a0d0-80224f25fabb>
 STRING transformNodeName;
 DWORD nWeights;
 array DWORD vertexIndices[nWeights];
 array FLOAT weights[nWeights];
 Matrix4x4 matrixOffset;
}

template FVFData {
 <b6e70a0e-8ef9-4e83-94ad-ecc8b0c04897>
 DWORD dwFVF;
 DWORD nDWords;
 array DWORD data[nDWords];
}

template EffectInstance {
 <e331f7e4-0559-4cc2-8e99-1cec1657928f>
 STRING EffectFilename;
 [...]
}

template EffectParamFloats {
 <3014b9a0-62f5-478c-9b86-e4ac9f4e418b>
 STRING ParamName;
 DWORD nFloats;
 array FLOAT Floats[nFloats];
}

template EffectParamString {
 <1dbc4c88-94c1-46ee-9076-2c28818c9481>
 STRING ParamName;
 STRING Value;
}

template EffectParamDWord {
 <e13963bc-ae51-4c5d-b00f-cfa3a9d97ce5>
 STRING ParamName;
 DWORD Value;
}


Frame Box01 {
 

 FrameTransformMatrix {
  0.701410,0.000000,0.000000,0.000000,0.000000,0.701410,0.000000,0.000000,0.000000,0.000000,0.701410,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 Frame {
  

  FrameTransformMatrix {
   1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,-25.000002,0.000000,1.000000;;
  }

  Mesh  {
   20;
   -25.000000;0.000000;-25.000000;,
   25.000000;0.000000;-25.000000;,
   -25.000000;0.000000;25.000000;,
   25.000000;0.000000;25.000000;,
   -25.000000;50.000000;-25.000000;,
   25.000000;50.000000;-25.000000;,
   -25.000000;50.000000;25.000000;,
   25.000000;50.000000;25.000000;,
   -25.000000;0.000000;-25.000000;,
   25.000000;50.000000;-25.000000;,
   25.000000;0.000000;-25.000000;,
   -25.000000;50.000000;-25.000000;,
   25.000000;0.000000;25.000000;,
   25.000000;50.000000;-25.000000;,
   25.000000;0.000000;25.000000;,
   -25.000000;50.000000;25.000000;,
   -25.000000;0.000000;25.000000;,
   25.000000;50.000000;25.000000;,
   -25.000000;0.000000;25.000000;,
   -25.000000;50.000000;-25.000000;;
   12;
   3;0,3,2;,
   3;3,0,1;,
   3;4,7,5;,
   3;7,4,6;,
   3;8,9,10;,
   3;9,8,11;,
   3;1,7,12;,
   3;7,1,13;,
   3;14,15,16;,
   3;15,14,17;,
   3;18,19,0;,
   3;19,18,6;;

   MeshNormals  {
    6;
    0.000000;-1.000000;0.000000;,
    0.000000;1.000000;0.000000;,
    0.000000;0.000000;-1.000000;,
    1.000000;0.000000;0.000000;,
    0.000000;0.000000;1.000000;,
    -1.000000;0.000000;0.000000;;
    12;
    3;0,0,0;,
    3;0,0,0;,
    3;1,1,1;,
    3;1,1,1;,
    3;2,2,2;,
    3;2,2,2;,
    3;3,3,3;,
    3;3,3,3;,
    3;4,4,4;,
    3;4,4,4;,
    3;5,5,5;,
    3;5,5,5;;
   }

   MeshMaterialList  {
    1;
    12;
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0;

    Material {
     0.109804;0.349020;0.694118;1.000000;;
     0.000000;
     0.109804;0.349020;0.694118;;
     0.000000;0.000000;0.000000;;
    }
   }

   MeshTextureCoords  {
    20;
    1.000000;1.000000;,
    0.000000;1.000000;,
    1.000000;0.000000;,
    0.000000;0.000000;,
    0.000000;1.000000;,
    1.000000;1.000000;,
    0.000000;0.000000;,
    1.000000;0.000000;,
    0.000000;1.000000;,
    1.000000;0.000000;,
    1.000000;1.000000;,
    0.000000;0.000000;,
    1.000000;1.000000;,
    0.000000;0.000000;,
    0.000000;1.000000;,
    1.000000;0.000000;,
    1.000000;1.000000;,
    0.000000;0.000000;,
    0.000000;1.000000;,
    1.000000;0.000000;;
   }
  }
 }
}