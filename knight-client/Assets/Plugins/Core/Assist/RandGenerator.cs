//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Core
{
    public class RandGenerator
    {
        public uint x;
        public uint y;
        public uint z;
        public uint w;
        
        public RandGenerator(uint seed = 0)
        {
            this.SetSeed(seed);
        }

        public uint GetSeed()
        {
            return x;
        }

        public float Range(float min, float max)
        {
            float t = this.GetFloat();
            t = min * t + (1.0F - t) * max;
            return t;
        }

        public int Range(int min, int max)
        {
            int dif;
            if (min < max)
            {
                dif = max - min;
                int t = (int)(this.Get() % dif);
                t += min;
                return t;
            }
            else if (min > max)
            {
                dif = min - max;
                int t = (int)(this.Get() % dif);
                t = min - t;
                return t;
            }
            else
            {
                return min;
            }
        }

        public float Range01()
        {
            return this.GetFloat();
        }

        private uint Get()
        {
            uint t = 0;
            t = x ^ (x << 11);
            x = y; y = z; z = w;
            return w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        }

        private float GetFloatFromInt(uint value)
        {
            return ((float)(value & 0x007FFFFF)) * (1.0f / 8388607.0f);
        }

        private uint GetByteFromInt(uint value)
        {
            return (value >> (23 - 8));
        }

        private float GetFloat()
        {
            return GetFloatFromInt(Get());
        }

        private float GetSignedFloat()
        {
            return GetFloat() * 2.0f - 1.0f;
        }

        private void SetSeed(uint seed)
        {
            x = seed;
            y = x * 1812433253U + 1;
            z = y * 1812433253U + 1;
            w = z * 1812433253U + 1;
        }
    }
}
