using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class ManagedLeakDetector : MonoBehaviour
{
    private Vector2 scroll;

    private static bool CheckRelation(Type a, Type b)
    {
        return (a.IsAssignableFrom(b) || b.IsAssignableFrom(a));
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (Camera.main == null)
            {
                GUI.Box(new Rect(-5f, -5f, (float) (Screen.width + 10), (float) (Screen.height + 10)), GUIContent.none);
            }
            ReadResult result = new ReadResult();
            result.Read();
            Counter[] counters = result.counters;
            float width = Screen.width - 10;
            this.scroll = GUI.BeginScrollView(new Rect(5f, 5f, width, (float) (Screen.height - 10)), this.scroll, new Rect(0f, 0f, width, (float) (counters.Length * 20)));
            int num2 = 0;
            foreach (Counter counter in counters)
            {
                GUI.Label(new Rect(0f, (float) num2, width, 20f), string.Format("{0:000} [{1:0000}] {2}", counter.actualInstanceCount, counter.derivedInstanceCount, counter.type));
                num2 += 20;
            }
        }
    }

    public static string Poll()
    {
        return Poll(typeof(Object));
    }

    public static string Poll(Type searchType)
    {
        return Poll(searchType, typeof(Object));
    }

    public static string Poll(Type searchType, Type minType)
    {
        return new ReadResult(searchType, minType).ToString();
    }

    private class Counter
    {
        public int actualInstanceCount;
        public int derivedInstanceCount;
        public int enabledCount;
        public Type type;
    }

    private class ReadResult
    {
        [CompilerGenerated]
        private static Comparison<ManagedLeakDetector.Counter> <>f__am$cache13;
        public bool complete;
        public ManagedLeakDetector.Counter[] counters;
        public readonly Type minType;
        public readonly Type searchType;
        public ManagedLeakDetector.SumEnable sumAnimation;
        public ManagedLeakDetector.SumEnable sumAnimationClip;
        public ManagedLeakDetector.SumEnable sumAudioClip;
        public ManagedLeakDetector.SumEnable sumBehaviour;
        public ManagedLeakDetector.SumEnable sumCloth;
        public ManagedLeakDetector.SumEnable sumCollider;
        public ManagedLeakDetector.SumEnable sumComponent;
        public ManagedLeakDetector.SumEnable sumGameObject;
        public ManagedLeakDetector.SumEnable sumMaterial;
        public ManagedLeakDetector.SumEnable sumMesh;
        public ManagedLeakDetector.SumEnable sumParticleEmitter;
        public ManagedLeakDetector.SumEnable sumParticleSystem;
        public ManagedLeakDetector.SumEnable sumRenderer;
        public ManagedLeakDetector.SumEnable sumScriptableObject;
        public ManagedLeakDetector.SumEnable sumTexture;

        public ReadResult() : this(typeof(Object))
        {
        }

        public ReadResult(Type searchType) : this(searchType, typeof(Object))
        {
        }

        public unsafe ReadResult(Type searchType, Type minType)
        {
            Type expressionStack_19_0;
            ManagedLeakDetector.ReadResult expressionStack_19_1;
            ManagedLeakDetector.ReadResult expressionStack_E_1;
            Type expressionStack_31_0;
            ManagedLeakDetector.ReadResult expressionStack_31_1;
            ManagedLeakDetector.ReadResult expressionStack_26_1;
            ref ManagedLeakDetector.SumEnable expressionStack_157_0;
            int expressionStack_16A_0;
            ref ManagedLeakDetector.SumEnable expressionStack_16A_1;
            ref ManagedLeakDetector.SumEnable expressionStack_185_0;
            int expressionStack_198_0;
            ref ManagedLeakDetector.SumEnable expressionStack_198_1;
            ref ManagedLeakDetector.SumEnable expressionStack_1B3_0;
            int expressionStack_1C6_0;
            ref ManagedLeakDetector.SumEnable expressionStack_1C6_1;
            ref ManagedLeakDetector.SumEnable expressionStack_1E1_0;
            int expressionStack_1F4_0;
            ref ManagedLeakDetector.SumEnable expressionStack_1F4_1;
            ref ManagedLeakDetector.SumEnable expressionStack_20F_0;
            int expressionStack_222_0;
            ref ManagedLeakDetector.SumEnable expressionStack_222_1;
            ref ManagedLeakDetector.SumEnable expressionStack_23D_0;
            int expressionStack_250_0;
            ref ManagedLeakDetector.SumEnable expressionStack_250_1;
            ref ManagedLeakDetector.SumEnable expressionStack_26B_0;
            int expressionStack_27E_0;
            ref ManagedLeakDetector.SumEnable expressionStack_27E_1;
            if (minType != null)
            {
                expressionStack_19_1 = this;
                expressionStack_19_0 = minType;
                goto Label_0019;
            }
            else
            {
                expressionStack_E_1 = this;
                Type expressionStack_E_0 = minType;
            }
            expressionStack_19_1 = expressionStack_E_1;
            expressionStack_19_0 = typeof(Object);
        Label_0019:
            expressionStack_19_1.minType = expressionStack_19_0;
            if (searchType != null)
            {
                expressionStack_31_1 = this;
                expressionStack_31_0 = searchType;
                goto Label_0031;
            }
            else
            {
                expressionStack_26_1 = this;
                Type expressionStack_26_0 = searchType;
            }
            expressionStack_31_1 = expressionStack_26_1;
            expressionStack_31_0 = typeof(Object);
        Label_0031:
            expressionStack_31_1.searchType = expressionStack_31_0;
            this.sumComponent.name = "Components";
            this.sumBehaviour.name = "Behaviours";
            this.sumRenderer.name = "Renderers";
            this.sumCollider.name = "Colliders";
            this.sumCloth.name = "Cloths";
            this.sumGameObject.name = "Game Objects";
            this.sumScriptableObject.name = "Scriptable Objects";
            this.sumMaterial.name = "Materials";
            this.sumTexture.name = "Textures";
            this.sumAnimation.name = "Animations";
            this.sumMesh.name = "Meshes";
            this.sumAudioClip.name = "Audio Clips";
            this.sumAnimationClip.name = "Animation Clips";
            this.sumParticleEmitter.name = "Particle Emitters (Legacy)";
            this.sumParticleSystem.name = "Particle Systems";
            this.sumComponent.check = ManagedLeakDetector.CheckRelation(searchType, typeof(Component));
            if (!this.sumComponent.check)
            {
                expressionStack_16A_1 = (ManagedLeakDetector.SumEnable) &this.sumBehaviour;
                expressionStack_16A_0 = 0;
                goto Label_016A;
            }
            else
            {
                expressionStack_157_0 = (ManagedLeakDetector.SumEnable) &this.sumBehaviour;
            }
            expressionStack_16A_1 = expressionStack_157_0;
            expressionStack_16A_0 = (int) ManagedLeakDetector.CheckRelation(typeof(Behaviour), searchType);
        Label_016A:
            expressionStack_16A_1.check = (bool) expressionStack_16A_0;
            if (!this.sumComponent.check)
            {
                expressionStack_198_1 = (ManagedLeakDetector.SumEnable) &this.sumRenderer;
                expressionStack_198_0 = 0;
                goto Label_0198;
            }
            else
            {
                expressionStack_185_0 = (ManagedLeakDetector.SumEnable) &this.sumRenderer;
            }
            expressionStack_198_1 = expressionStack_185_0;
            expressionStack_198_0 = (int) ManagedLeakDetector.CheckRelation(typeof(Renderer), searchType);
        Label_0198:
            expressionStack_198_1.check = (bool) expressionStack_198_0;
            if (!this.sumComponent.check)
            {
                expressionStack_1C6_1 = (ManagedLeakDetector.SumEnable) &this.sumCollider;
                expressionStack_1C6_0 = 0;
                goto Label_01C6;
            }
            else
            {
                expressionStack_1B3_0 = (ManagedLeakDetector.SumEnable) &this.sumCollider;
            }
            expressionStack_1C6_1 = expressionStack_1B3_0;
            expressionStack_1C6_0 = (int) ManagedLeakDetector.CheckRelation(typeof(Collider), searchType);
        Label_01C6:
            expressionStack_1C6_1.check = (bool) expressionStack_1C6_0;
            if (!this.sumComponent.check)
            {
                expressionStack_1F4_1 = (ManagedLeakDetector.SumEnable) &this.sumCloth;
                expressionStack_1F4_0 = 0;
                goto Label_01F4;
            }
            else
            {
                expressionStack_1E1_0 = (ManagedLeakDetector.SumEnable) &this.sumCloth;
            }
            expressionStack_1F4_1 = expressionStack_1E1_0;
            expressionStack_1F4_0 = (int) ManagedLeakDetector.CheckRelation(typeof(Cloth), searchType);
        Label_01F4:
            expressionStack_1F4_1.check = (bool) expressionStack_1F4_0;
            if (!this.sumComponent.check)
            {
                expressionStack_222_1 = (ManagedLeakDetector.SumEnable) &this.sumParticleSystem;
                expressionStack_222_0 = 0;
                goto Label_0222;
            }
            else
            {
                expressionStack_20F_0 = (ManagedLeakDetector.SumEnable) &this.sumParticleSystem;
            }
            expressionStack_222_1 = expressionStack_20F_0;
            expressionStack_222_0 = (int) ManagedLeakDetector.CheckRelation(typeof(ParticleSystem), searchType);
        Label_0222:
            expressionStack_222_1.check = (bool) expressionStack_222_0;
            if (!this.sumBehaviour.check)
            {
                expressionStack_250_1 = (ManagedLeakDetector.SumEnable) &this.sumAnimation;
                expressionStack_250_0 = 0;
                goto Label_0250;
            }
            else
            {
                expressionStack_23D_0 = (ManagedLeakDetector.SumEnable) &this.sumAnimation;
            }
            expressionStack_250_1 = expressionStack_23D_0;
            expressionStack_250_0 = (int) ManagedLeakDetector.CheckRelation(typeof(Animation), searchType);
        Label_0250:
            expressionStack_250_1.check = (bool) expressionStack_250_0;
            if (!this.sumComponent.check)
            {
                expressionStack_27E_1 = (ManagedLeakDetector.SumEnable) &this.sumParticleEmitter;
                expressionStack_27E_0 = 0;
                goto Label_027E;
            }
            else
            {
                expressionStack_26B_0 = (ManagedLeakDetector.SumEnable) &this.sumParticleEmitter;
            }
            expressionStack_27E_1 = expressionStack_26B_0;
            expressionStack_27E_0 = (int) ManagedLeakDetector.CheckRelation(typeof(ParticleEmitter), searchType);
        Label_027E:
            expressionStack_27E_1.check = (bool) expressionStack_27E_0;
            this.sumGameObject.check = ManagedLeakDetector.CheckRelation(typeof(GameObject), searchType);
            this.sumScriptableObject.check = ManagedLeakDetector.CheckRelation(typeof(ScriptableObject), searchType);
            this.sumMaterial.check = ManagedLeakDetector.CheckRelation(typeof(Material), searchType);
            this.sumTexture.check = ManagedLeakDetector.CheckRelation(typeof(Texture), searchType);
            this.sumMesh.check = ManagedLeakDetector.CheckRelation(typeof(Mesh), searchType);
            this.sumAudioClip.check = ManagedLeakDetector.CheckRelation(typeof(AudioClip), searchType);
            this.sumAnimationClip.check = ManagedLeakDetector.CheckRelation(typeof(AnimationClip), searchType);
        }

        private static void Print(StringBuilder sb, ref ManagedLeakDetector.SumEnable en)
        {
            if (en.check)
            {
                if (en.enabled != 0)
                {
                    sb.AppendFormat("{0} {1} ({2})\r\n", en.name, en.total, en.enabled);
                }
                else if (en.total != 0)
                {
                    sb.AppendFormat("{0} {1}\r\n", en.name, en.total);
                }
            }
        }

        public void Read()
        {
            this.Read(false);
        }

        public void Read(bool forceUpdate)
        {
            if (!this.complete || forceUpdate)
            {
                Dictionary<Type, ManagedLeakDetector.Counter> dictionary = new Dictionary<Type, ManagedLeakDetector.Counter>();
                ManagedLeakDetector.Counter counter = new ManagedLeakDetector.Counter {
                    type = this.minType
                };
                dictionary.Add(this.minType, counter);
                this.sumComponent.Reset();
                this.sumBehaviour.Reset();
                this.sumRenderer.Reset();
                this.sumCollider.Reset();
                this.sumCloth.Reset();
                this.sumGameObject.Reset();
                this.sumScriptableObject.Reset();
                this.sumMaterial.Reset();
                this.sumTexture.Reset();
                this.sumAnimation.Reset();
                this.sumMesh.Reset();
                this.sumAudioClip.Reset();
                this.sumAnimationClip.Reset();
                this.sumParticleSystem.Reset();
                this.sumParticleEmitter.Reset();
                this.sumComponent.check = ManagedLeakDetector.CheckRelation(this.searchType, typeof(Component));
                this.sumBehaviour.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(Behaviour), this.searchType);
                this.sumRenderer.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(Renderer), this.searchType);
                this.sumCollider.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(Collider), this.searchType);
                this.sumCloth.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(Cloth), this.searchType);
                this.sumParticleSystem.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(ParticleSystem), this.searchType);
                this.sumAnimation.check = this.sumBehaviour.check && ManagedLeakDetector.CheckRelation(typeof(Animation), this.searchType);
                this.sumParticleEmitter.check = this.sumComponent.check && ManagedLeakDetector.CheckRelation(typeof(ParticleEmitter), this.searchType);
                this.sumGameObject.check = ManagedLeakDetector.CheckRelation(typeof(GameObject), this.searchType);
                this.sumScriptableObject.check = ManagedLeakDetector.CheckRelation(typeof(ScriptableObject), this.searchType);
                this.sumMaterial.check = ManagedLeakDetector.CheckRelation(typeof(Material), this.searchType);
                this.sumTexture.check = ManagedLeakDetector.CheckRelation(typeof(Texture), this.searchType);
                this.sumMesh.check = ManagedLeakDetector.CheckRelation(typeof(Mesh), this.searchType);
                this.sumAudioClip.check = ManagedLeakDetector.CheckRelation(typeof(AudioClip), this.searchType);
                this.sumAnimationClip.check = ManagedLeakDetector.CheckRelation(typeof(AnimationClip), this.searchType);
                foreach (Object obj2 in Object.FindObjectsOfType(this.searchType))
                {
                    ManagedLeakDetector.Counter counter2;
                    ManagedLeakDetector.Counter counter3;
                    Type key = obj2.GetType();
                    if (dictionary.TryGetValue(key, out counter2))
                    {
                        counter2.actualInstanceCount++;
                    }
                    else
                    {
                        counter3 = new ManagedLeakDetector.Counter {
                            type = key,
                            actualInstanceCount = 1
                        };
                        dictionary.Add(key, counter2 = counter3);
                    }
                    if (this.sumComponent.check && typeof(Component).IsAssignableFrom(key))
                    {
                        this.sumComponent.total++;
                        if (this.sumBehaviour.check && typeof(Behaviour).IsAssignableFrom(key))
                        {
                            if (((Behaviour) obj2).enabled)
                            {
                                this.sumComponent.enabled++;
                                counter2.enabledCount++;
                                this.sumBehaviour.enabled++;
                                this.sumBehaviour.total++;
                                if (this.sumAnimation.check && typeof(Animation).IsAssignableFrom(key))
                                {
                                    this.sumAnimation.enabled++;
                                    this.sumAnimation.total++;
                                }
                            }
                            else if (this.sumAnimation.check && typeof(Animation).IsAssignableFrom(key))
                            {
                                this.sumAnimation.total++;
                            }
                        }
                        else if (this.sumRenderer.check && typeof(Renderer).IsAssignableFrom(key))
                        {
                            this.sumRenderer.total++;
                            if (((Renderer) obj2).enabled)
                            {
                                this.sumComponent.enabled++;
                                this.sumRenderer.enabled++;
                                counter2.enabledCount++;
                            }
                        }
                        else if (this.sumCollider.check && typeof(Collider).IsAssignableFrom(key))
                        {
                            this.sumCollider.total++;
                            if (((Collider) obj2).enabled)
                            {
                                this.sumComponent.enabled++;
                                this.sumCollider.enabled++;
                                counter2.enabledCount++;
                            }
                        }
                        else if (this.sumParticleSystem.check && typeof(ParticleSystem).IsAssignableFrom(key))
                        {
                            this.sumParticleSystem.total++;
                            if (((ParticleSystem) obj2).IsAlive())
                            {
                                counter2.enabledCount++;
                                this.sumParticleSystem.enabled++;
                                this.sumComponent.enabled++;
                            }
                        }
                        else if (this.sumCloth.check && typeof(Cloth).IsAssignableFrom(key))
                        {
                            this.sumCloth.total++;
                            if (((Cloth) obj2).enabled)
                            {
                                counter2.enabledCount++;
                                this.sumComponent.enabled++;
                                this.sumCloth.enabled++;
                            }
                        }
                        else if (this.sumParticleEmitter.check && typeof(ParticleEmitter).IsAssignableFrom(key))
                        {
                            this.sumParticleEmitter.total++;
                            if (((ParticleEmitter) obj2).enabled)
                            {
                                counter2.enabledCount++;
                                this.sumParticleEmitter.enabled++;
                                this.sumComponent.enabled++;
                            }
                        }
                    }
                    else if (this.sumGameObject.check && typeof(GameObject).IsAssignableFrom(key))
                    {
                        this.sumGameObject.total++;
                        if (((GameObject) obj2).activeInHierarchy)
                        {
                            this.sumGameObject.enabled++;
                            counter2.enabledCount++;
                        }
                    }
                    else if (this.sumMaterial.check && typeof(Material).IsAssignableFrom(key))
                    {
                        this.sumMaterial.total++;
                    }
                    else if (this.sumTexture.check && typeof(Texture).IsAssignableFrom(key))
                    {
                        this.sumTexture.total++;
                    }
                    else if (this.sumAudioClip.check && typeof(AudioClip).IsAssignableFrom(key))
                    {
                        this.sumAudioClip.total++;
                    }
                    else if (this.sumAnimationClip.check && typeof(AnimationClip).IsAssignableFrom(key))
                    {
                        this.sumAnimationClip.total++;
                    }
                    else if (this.sumMesh.check && typeof(Mesh).IsAssignableFrom(key))
                    {
                        this.sumMesh.total++;
                    }
                    else if (this.sumScriptableObject.check && typeof(ScriptableObject).IsAssignableFrom(key))
                    {
                        this.sumScriptableObject.total++;
                    }
                    if (key != this.minType)
                    {
                        for (key = key.BaseType; key != typeof(Object); key = key.BaseType)
                        {
                            if (dictionary.TryGetValue(key, out counter2))
                            {
                                counter2.derivedInstanceCount++;
                            }
                            else
                            {
                                counter3 = new ManagedLeakDetector.Counter {
                                    type = key,
                                    derivedInstanceCount = 1
                                };
                                dictionary.Add(key, counter3);
                            }
                        }
                        counter.derivedInstanceCount++;
                    }
                }
                List<ManagedLeakDetector.Counter> list = new List<ManagedLeakDetector.Counter>(dictionary.Values);
                if (<>f__am$cache13 == null)
                {
                    <>f__am$cache13 = delegate (ManagedLeakDetector.Counter firstPair, ManagedLeakDetector.Counter nextPair) {
                        int num = nextPair.actualInstanceCount.CompareTo(firstPair.actualInstanceCount);
                        if (num == 0)
                        {
                            return nextPair.derivedInstanceCount.CompareTo(firstPair.derivedInstanceCount);
                        }
                        return num;
                    };
                }
                list.Sort(<>f__am$cache13);
                this.counters = list.ToArray();
                this.complete = true;
            }
        }

        public override string ToString()
        {
            this.Read();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Instances, Deriving Instances, Type, (# Enabled [if not shown 0] )");
            foreach (ManagedLeakDetector.Counter counter in this.counters)
            {
                if (counter.enabledCount != 0)
                {
                    object[] args = new object[] { counter.actualInstanceCount, counter.derivedInstanceCount, counter.type, counter.enabledCount };
                    sb.AppendFormat("{0,8} [{1,8}] {2} ({3} enabled)\r\n", args);
                }
                else
                {
                    sb.AppendFormat("{0,8} [{1,8}] {2}\r\n", counter.actualInstanceCount, counter.derivedInstanceCount, counter.type);
                }
            }
            sb.AppendLine("basic counters: if not there, there is none.");
            Print(sb, ref this.sumComponent);
            Print(sb, ref this.sumBehaviour);
            Print(sb, ref this.sumRenderer);
            Print(sb, ref this.sumCollider);
            Print(sb, ref this.sumCloth);
            Print(sb, ref this.sumGameObject);
            Print(sb, ref this.sumScriptableObject);
            Print(sb, ref this.sumMaterial);
            Print(sb, ref this.sumTexture);
            Print(sb, ref this.sumAnimation);
            Print(sb, ref this.sumMesh);
            Print(sb, ref this.sumAudioClip);
            Print(sb, ref this.sumAnimationClip);
            Print(sb, ref this.sumParticleSystem);
            Print(sb, ref this.sumParticleEmitter);
            sb.AppendFormat("Count done for search {0} (min:{1})", this.searchType, this.minType);
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SumEnable
    {
        public bool check;
        public int total;
        public int enabled;
        public string name;
        public Type type;
        public int disabled
        {
            get
            {
                return (this.total - this.enabled);
            }
        }
        public void Reset()
        {
            this.total = 0;
            this.enabled = 0;
        }
    }
}

