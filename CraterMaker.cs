using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CraterMaker : MonoBehaviour
{
    public int insidetextureindex;
    public Terrain MyTerrain;

    public void Create(Vector2 position, float radius, float depth, float noise)
    {
        base.StartCoroutine(this.RealCreate(position, radius, depth, noise));
    }

    public void Create(Vector3 position, float radius, float depth, float noise)
    {
        this.Create(new Vector2(position.x, position.z), radius, depth, noise);
    }

    [DebuggerHidden]
    public IEnumerator RealCreate(Vector2 position, float radius, float depth, float noise)
    {
        return new <RealCreate>c__Iterator4F { position = position, radius = radius, depth = depth, noise = noise, <$>position = position, <$>radius = radius, <$>depth = depth, <$>noise = noise, <>f__this = this };
    }

    [CompilerGenerated]
    private sealed class <RealCreate>c__Iterator4F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>depth;
        internal float <$>noise;
        internal Vector2 <$>position;
        internal float <$>radius;
        internal CraterMaker <>f__this;
        internal float[,] <heights>__7;
        internal float <heightscale>__8;
        internal int <i>__14;
        internal int <i>__9;
        internal int <j>__10;
        internal int <j>__15;
        internal float <mod>__11;
        internal float <mod>__16;
        internal Vector3 <pos>__2;
        internal int <s>__17;
        internal int <s>__19;
        internal int <s>__20;
        internal float <scale>__3;
        internal Vector3 <size>__1;
        internal int <splats>__13;
        internal float <sum>__18;
        internal TerrainData <tdata>__0;
        internal float[,,] <textures>__12;
        internal int <width>__4;
        internal int <xpos>__5;
        internal int <ypos>__6;
        internal float depth;
        internal float noise;
        internal Vector2 position;
        internal float radius;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<tdata>__0 = this.<>f__this.MyTerrain.terrainData;
                    this.<size>__1 = this.<tdata>__0.size;
                    this.<pos>__2 = this.<>f__this.MyTerrain.transform.position;
                    this.position.x -= this.<pos>__2.x;
                    this.position.y -= this.<pos>__2.y;
                    this.<scale>__3 = ((float) this.<tdata>__0.heightmapResolution) / this.<size>__1.x;
                    this.<width>__4 = (int) Mathf.Floor(this.radius * this.<scale>__3);
                    this.<xpos>__5 = (int) Mathf.Floor((this.position.x - this.radius) * this.<scale>__3);
                    this.<ypos>__6 = (int) Mathf.Floor((this.position.y - this.radius) * this.<scale>__3);
                    this.<heights>__7 = this.<tdata>__0.GetHeights(this.<xpos>__5, this.<ypos>__6, this.<width>__4 * 2, this.<width>__4 * 2);
                    this.<heightscale>__8 = this.depth / (this.<size>__1.y * 2f);
                    this.<i>__9 = 0;
                    while (this.<i>__9 < (this.<width>__4 * 2))
                    {
                        this.<j>__10 = 0;
                        while (this.<j>__10 < (this.<width>__4 * 2))
                        {
                            this.<mod>__11 = Mathf.SmoothStep(1f, 0f, Mathf.Abs((float) (this.<width>__4 - this.<i>__9)) / ((float) this.<width>__4)) * Mathf.SmoothStep(1f, 0f, Mathf.Abs((float) (this.<width>__4 - this.<j>__10)) / ((float) this.<width>__4));
                            this.<mod>__11 *= this.<heightscale>__8;
                            if (this.noise > 0f)
                            {
                                this.<mod>__11 += (((this.<mod>__11 * this.<heightscale>__8) * this.depth) * Random.value) * this.noise;
                            }
                            float single1 = this.<heights>__7[this.<i>__9, this.<j>__10];
                            single1[0] -= this.<mod>__11;
                            this.<j>__10++;
                        }
                        this.<i>__9++;
                    }
                    this.<tdata>__0.SetHeights(this.<xpos>__5, this.<ypos>__6, this.<heights>__7);
                    this.$current = new WaitForFixedUpdate();
                    this.$PC = 1;
                    goto Label_0623;

                case 1:
                    this.$current = new WaitForFixedUpdate();
                    this.$PC = 2;
                    goto Label_0623;

                case 2:
                    this.<scale>__3 = ((float) this.<tdata>__0.alphamapResolution) / this.<size>__1.x;
                    this.<width>__4 = (int) Mathf.Floor(this.radius * this.<scale>__3);
                    this.<xpos>__5 = (int) Mathf.Floor((this.position.x - this.radius) * this.<scale>__3);
                    this.<ypos>__6 = (int) Mathf.Floor((this.position.y - this.radius) * this.<scale>__3);
                    this.<textures>__12 = this.<tdata>__0.GetAlphamaps(this.<xpos>__5, this.<ypos>__6, this.<width>__4 * 2, this.<width>__4 * 2);
                    this.<splats>__13 = this.<textures>__12.Length / ((this.<width>__4 * this.<width>__4) * 4);
                    this.<i>__14 = 0;
                    while (this.<i>__14 < (this.<width>__4 * 2))
                    {
                        this.<j>__15 = 0;
                        while (this.<j>__15 < (this.<width>__4 * 2))
                        {
                            this.<mod>__16 = Mathf.SmoothStep(1f, 0f, Mathf.Abs((float) (this.<width>__4 - this.<i>__14)) / ((float) this.<width>__4)) * Mathf.SmoothStep(1f, 0f, Mathf.Abs((float) (this.<width>__4 - this.<j>__15)) / ((float) this.<width>__4));
                            float single2 = this.<textures>__12[this.<i>__14, this.<j>__15, this.<>f__this.insidetextureindex];
                            single2[0] += this.<mod>__16;
                            this.<s>__17 = 0;
                            while (this.<s>__17 < this.<splats>__13)
                            {
                                if (this.<s>__17 == this.<>f__this.insidetextureindex)
                                {
                                    float single3 = this.<textures>__12[this.<i>__14, this.<j>__15, this.<s>__17];
                                    single3[0] += this.<mod>__16;
                                }
                                else
                                {
                                    float single4 = this.<textures>__12[this.<i>__14, this.<j>__15, this.<s>__17];
                                    single4[0] -= this.<textures>__12[this.<i>__14, this.<j>__15, this.<s>__17] * this.<mod>__16;
                                }
                                this.<s>__17++;
                            }
                            this.<sum>__18 = 0f;
                            this.<s>__19 = 0;
                            while (this.<s>__19 < this.<splats>__13)
                            {
                                this.<sum>__18 += this.<textures>__12[this.<i>__14, this.<j>__15, this.<s>__19];
                                this.<s>__19++;
                            }
                            this.<s>__20 = 0;
                            while (this.<s>__20 < this.<splats>__13)
                            {
                                float single5 = this.<textures>__12[this.<i>__14, this.<j>__15, this.<s>__20];
                                single5[0] *= 1f / this.<sum>__18;
                                this.<s>__20++;
                            }
                            this.<j>__15++;
                        }
                        this.<i>__14++;
                    }
                    this.<tdata>__0.SetAlphamaps(this.<xpos>__5, this.<ypos>__6, this.<textures>__12);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0623:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

