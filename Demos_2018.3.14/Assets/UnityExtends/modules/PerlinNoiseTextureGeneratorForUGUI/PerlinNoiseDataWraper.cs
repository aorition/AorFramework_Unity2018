using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{

    public enum PerlinNoiseDataBlendMethod 
    {
        Normal,
        Darken,
        Lighten,
        Multiply,
        Screen,
        ColorBurn,
        ColorDodge,
        LinearBurn,
        LinearDodge,
        Overlay,
        HardLight,
        SoftLight,
        VividLight,
        LinearLight,
        PinLight,
        HardMix,
        Difference,
        Exclusion
    }


    [Serializable]
    public class PerlinNoiseDataWraper 
    {

        public PerlinNoiseDataWraper(int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, bool R, bool G, bool B, bool A, PerlinNoiseDataBlendMethod method, bool enabled)
        {
            if(scale <= 0)
                scale = 0.0001f;

            this.enabled = enabled;

            this.seed = seed;
            this.scale = scale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;

            this.offset = offset;

            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
            this.blendMethod = method;

            buildingOctaveOffsets();
        }

        public PerlinNoiseDataWraper(int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, bool R, bool G, bool B, bool A, PerlinNoiseDataBlendMethod method)
        {
            if(scale <= 0)
                scale = 0.0001f;

            this.seed = seed;
            this.scale = scale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;

            this.offset = offset;

            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
            this.blendMethod = method;

            buildingOctaveOffsets();
        }

        public PerlinNoiseDataWraper(int seed)
        {
            this.seed = seed;
            buildingOctaveOffsets();
        }

        public bool enabled = true;

        public int seed;
        public float scale = 1f;
        public int octaves = 2;
        public float persistance = 0.95f;
        public float lacunarity = 1.12f;
        public Vector2 offset = Vector2.zero;

        public bool R = true;
        public bool G = true;
        public bool B = true;
        public bool A = false;

        public PerlinNoiseDataBlendMethod blendMethod = PerlinNoiseDataBlendMethod.Normal;

        public Vector2[] octaveOffsets;

        public PerlinNoiseDataWraper Copy()
        {
            return new PerlinNoiseDataWraper(seed, scale, octaves, persistance, lacunarity, offset, R, G, B, A, blendMethod);
        }

        private void buildingOctaveOffsets()
        {
            System.Random random = new System.Random(this.seed);
            octaveOffsets = new Vector2[octaves];
            for(int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + offset.x;
                float offsetY = random.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
        }

    }

}
