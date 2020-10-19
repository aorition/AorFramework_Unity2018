using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    
    public class PerlinNoiseTexGenerator :Texture2DCacheForRawImage
    {

        public int BlurSize = 0;

        public Vector2 Offset;
        public List<PerlinNoiseDataWraper> dataWraperList;
        
        public void ClearTexture()
        {
            ClearCache();
            texture = null;
        }

        public void BuildingPerlinNoise(Texture2D texture2D)
        {

            int w = texture2D.width;
            int h = texture2D.height;

            for(int v = 0; v < h; v++)
            {
                for(int u = 0; u < w; u++)
                {
                    Color cur = getPerlinNoiseColor(u, v);
                    texture2D.SetPixel(u, v, cur);
                }
            }

            if(BlurSize > 0)
            {
                BlurTexture(texture2D, BlurSize);
            }
            else
            {
                texture2D.Apply();
                SaveTexToCache(texture2D);
            }
        }

        public void BuildingPerlinNoise(Texture2D texture2D, int soloInfoIndex)
        {

            int w = texture2D.width;
            int h = texture2D.height;

            for(int v = 0; v < h; v++)
            {
                for(int u = 0; u < w; u++)
                {
                    Color cur = getPerlinNoiseColor(u, v, soloInfoIndex);
                    texture2D.SetPixel(u, v, cur);
                }
            }

            if(BlurSize > 0)
            {
                BlurTexture(texture2D, BlurSize);
            }
            else
            {
                texture2D.Apply();
                SaveTexToCache(texture2D);
            }
        }

        private void BlurTexture(Texture2D texture2D, int blurSize)
        {

            int u, v;
            int w = texture2D.width;
            int h = texture2D.height;
            Color tmpColor;
            Color[,] nColors = new Color[w, h];
            float r, g, b, a;
            int count;

            for(v = 0; v < h; v++)
            {
                for(u = 0; u < w; u++)
                {
                    Color curColor = texture2D.GetPixel(u, v);
                    r = curColor.r;
                    g = curColor.g;
                    b = curColor.b;
                    a = curColor.a;
                    count = 1;
                    for(int y = -blurSize; y < blurSize; y++)
                    {
                        for(int x = -blurSize; x < blurSize; x++)
                        {
                            int u2 = u + x;
                            int v2 = v + y;
                            if(u2 >= 0 && u2 < w && v2 >= 0 && v2 < h)
                            {
                                tmpColor = texture2D.GetPixel(u2, v2);
                                r += tmpColor.r;
                                g += tmpColor.g;
                                b += tmpColor.b;
                                a += tmpColor.a;
                                count++;
                            }
                        }
                    }
                    r /= count;
                    g /= count;
                    b /= count;
                    a /= count;
                    nColors[u, v] = new Color(r, g, b, a);
                }
            }

            for(v = 0; v < h; v++)
            {
                for(u = 0; u < w; u++)
                {
                    texture2D.SetPixel(u, v, nColors[u, v]);
                }
            }

            texture2D.Apply();
            SaveTexToCache(texture2D);
        }

        private Color getPerlinNoiseColor(int u, int v)
        {
            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            float noiseHeight = 0;

            for(int j = 0; j < dataWraperList.Count; j++)
            {

                if(!dataWraperList[j].enabled)
                    continue;

                PerlinNoiseDataWraper dataWraper = dataWraperList[j];

                float amplitude = 1;
                float frequency = 1;

                for(int i = 0; i < dataWraper.octaves; i++)
                {
                    float sampleX = (u + Offset.x) / dataWraper.scale * frequency + dataWraper.octaveOffsets[i].x;
                    float sampleY = (v + Offset.y) / dataWraper.scale * frequency + dataWraper.octaveOffsets[i].y;

                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= dataWraper.persistance;
                    frequency *= dataWraper.lacunarity;

                }
                noiseHeight /= dataWraper.octaves;

                //color ...
                if(dataWraper.R)
                {
                    if(j == 0)
                        r = noiseHeight;
                    else
                    {
                        ccValueByBlendMethod(dataWraper.blendMethod, ref r, noiseHeight);
                    }

                }
                if(dataWraper.G)
                {
                    if(j == 0)
                        g = noiseHeight;
                    else
                    {
                        ccValueByBlendMethod(dataWraper.blendMethod, ref g, noiseHeight);
                    }

                }
                if(dataWraper.B)
                {

                    if(j == 0)
                        b = noiseHeight;
                    else
                    {
                        ccValueByBlendMethod(dataWraper.blendMethod, ref b, noiseHeight);
                    }

                }
                if(dataWraper.A)
                {
                    if(j == 0)
                        a = noiseHeight;
                    else
                    {
                        ccValueByBlendMethod(dataWraper.blendMethod, ref a, noiseHeight);
                    }
                }else if(j == 0)
                {
                    a = 1.0f;
                }
            }

            return new Color(r, g, b, a);
        }

        private void ccValueByBlendMethod(PerlinNoiseDataBlendMethod blendMethod, ref float A, float B)
        {
            switch(blendMethod)
            {
                case PerlinNoiseDataBlendMethod.Darken:
                    {
                        A = (B <= A ? B : A);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.Lighten:
                    {
                        A = (B <= A ? A : B);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.Multiply:
                    {
                        A = A * B;
                    }
                    break;
                case PerlinNoiseDataBlendMethod.Screen:
                    {
                        A = 1.0f - (1.0f - A) * (1.0f - B);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.ColorBurn:
                    {
                        A = A - (1.0f - A) * (1.0f - B) / B;
                    }
                    break;
                case PerlinNoiseDataBlendMethod.ColorDodge:
                    {
                        A = A + A * B / (1.0f - B);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.LinearBurn:
                    {
                        A = A + B - 1.0f;
                    }
                    break;
                case PerlinNoiseDataBlendMethod.LinearDodge:
                    {
                        A = A + B;
                    }
                break;
                case PerlinNoiseDataBlendMethod.Overlay:
                    {
                        A = (A <= 0.5f ? A * B : 1.0f - (1.0f - A) * (1.0f - B) / 0.5f);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.HardLight:
                    {
                        A = (B <= 0.5f ? A * B / 0.5f : 1.0f - (1.0f - A) * (1.0f - B) / 0.5f);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.SoftLight:
                    {
                        A = (B <= 0.5f ? A * B / 0.5f + A * A * (1.0f - 2 * B) : A * (1.0f - B) / 0.5f + Mathf.Sqrt(A) * 2 * B - 1.0f);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.VividLight:
                    {
                        A = (B <= 0.5f ? A - (1.0f - A) * (1.0f - 2 * B) : A + A * (2 * B - 1.0f) / 2 * (1.0f - B));
                    }
                    break;
                case PerlinNoiseDataBlendMethod.LinearLight:
                    {
                        A = A + 2 * B - 1.0f;
                    }
                    break;
                case PerlinNoiseDataBlendMethod.PinLight:
                    {
                        A = (B <= 0.5f ? Mathf.Min(A, 2 * B) : Mathf.Min(A, 2 * B - 1.0f));
                    }
                    break;
                case PerlinNoiseDataBlendMethod.HardMix:
                    {
                        A = (A + B >= 1.0f ? 1.0f : A + B);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.Difference:
                    {
                        A = Mathf.Abs(A - B);
                    }
                    break;
                case PerlinNoiseDataBlendMethod.Exclusion:
                    {
                        A = A + B - A * B / 0.5f;
                    }
                    break;
                default:
                    {
                        A = Mathf.Lerp(A, B, 0.5f);
                    }
                break;
            }
        }

        private Color getPerlinNoiseColor(int u, int v, int soloInfoIndex)
        {

            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            float noiseHeight = 0;

            float amplitude = 1;
            float frequency = 1;
            PerlinNoiseDataWraper dataWraper = dataWraperList[soloInfoIndex];
            for(int i = 0; i < dataWraper.octaves; i++)
            {
                float sampleX = (u + Offset.x) / dataWraper.scale * frequency + dataWraper.octaveOffsets[i].x;
                float sampleY = (v + Offset.y) / dataWraper.scale * frequency + dataWraper.octaveOffsets[i].y;

                //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinValue * amplitude;
                amplitude *= dataWraper.persistance;
                frequency *= dataWraper.lacunarity;

            }
            noiseHeight /= dataWraper.octaves;

            //color ...
            if(dataWraper.R)
                r = noiseHeight;
            if(dataWraper.G)
                g = noiseHeight;
            if(dataWraper.B)
                b = noiseHeight;
            if(dataWraper.A)
                a = noiseHeight;
            else
                a = 1.0f;
            return new Color(r, g, b, a);
        }

    }

}
