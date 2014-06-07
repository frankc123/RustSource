using Facepunch.Actor;
using System;
using uLink;
using UnityEngine;

[NGCAutoAddScript]
public class SleepingAvatar : DeployableObject, IServerSaveable
{
    protected const string ArmorConfigRPC = "SAAM";
    [NonSerialized]
    public ArmorDataBlock footArmor;
    public MeshFilter footMeshFilter;
    protected const string HasDiedNowRPC = "SAKL";
    [NonSerialized]
    public ArmorDataBlock headArmor;
    public MeshFilter headMeshFilter;
    private const string kPoseName = "sleep";
    [NonSerialized]
    public ArmorDataBlock legArmor;
    public MeshFilter legMeshFilter;
    [NonSerialized]
    private Ragdoll ragdollInstance;
    public Ragdoll ragdollPrefab;
    protected const string SettingLiveCharacterNowRPC = "SACH";
    [NonSerialized]
    public ArmorDataBlock torsoArmor;
    public MeshFilter torsoMeshFilter;

    private static bool BindArmorMap<TArmorModel>(ArmorDataBlock armor, ref ArmorModelMemberMap map) where TArmorModel: ArmorModel, new()
    {
        if (armor != null)
        {
            TArmorModel armorModel = armor.GetArmorModel<TArmorModel>();
            if (armorModel != null)
            {
                map.SetArmorModel<TArmorModel>(armorModel);
                return true;
            }
        }
        return false;
    }

    private static void BindRenderer<TArmorModel>(ArmorModelRenderer prefabRenderer, ArmorDataBlock armor, MeshFilter filter, MeshRenderer renderer) where TArmorModel: ArmorModel<TArmorModel>, new()
    {
        TArmorModel armorModel;
        if (armor != null)
        {
            armorModel = armor.GetArmorModel<TArmorModel>();
            if ((armorModel == null) && (prefabRenderer != null))
            {
                armorModel = prefabRenderer.GetArmorModel<TArmorModel>();
            }
        }
        else if (prefabRenderer != null)
        {
            armorModel = prefabRenderer.GetArmorModel<TArmorModel>();
        }
        else
        {
            return;
        }
        if (armorModel != null)
        {
            Mesh mesh;
            if (ArmorModelRenderer.Censored && (armorModel.censoredModel != null))
            {
                armorModel = armorModel.censoredModel;
            }
            if ((armorModel != null) && armorModel.actorMeshInfo.FindPose("sleep", out mesh))
            {
                filter.sharedMesh = mesh;
                renderer.sharedMaterials = armorModel.sharedMaterials;
            }
        }
    }

    private bool CreateRagdoll()
    {
        if (this.ragdollPrefab != null)
        {
            ArmorModelRenderer local = this.ragdollPrefab.GetLocal<ArmorModelRenderer>();
            if (local != null)
            {
                AnimationClip clip;
                float num;
                ActorRig actorRig = local.actorRig;
                if ((actorRig != null) && actorRig.FindPoseClip("sleep", out clip, out num))
                {
                    this.ragdollInstance = Object.Instantiate(this.ragdollPrefab, base.transform.position, base.transform.rotation) as Ragdoll;
                    this.ragdollInstance.sourceMain = this;
                    GameObject gameObject = this.ragdollInstance.gameObject;
                    Object.Destroy(gameObject, 80f);
                    gameObject.SampleAnimation(clip, num);
                    local = this.ragdollInstance.GetLocal<ArmorModelRenderer>();
                    ArmorModelMemberMap map = new ArmorModelMemberMap();
                    bool flag = false;
                    flag |= BindArmorMap<ArmorModelFeet>(this.footArmor, ref map);
                    flag |= BindArmorMap<ArmorModelLegs>(this.legArmor, ref map);
                    flag |= BindArmorMap<ArmorModelTorso>(this.torsoArmor, ref map);
                    if (flag | BindArmorMap<ArmorModelHead>(this.headArmor, ref map))
                    {
                        local.BindArmorModels(map);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public static void RebindAllRenderers()
    {
        foreach (Object obj2 in Object.FindObjectsOfType(typeof(SleepingAvatar)))
        {
            SleepingAvatar avatar = (SleepingAvatar) obj2;
            if (avatar != null)
            {
                avatar.RebindRenderers();
            }
        }
    }

    private void RebindRenderers()
    {
        ArmorModelRenderer prefabRenderer = (this.ragdollPrefab == null) ? null : this.ragdollPrefab.GetLocal<ArmorModelRenderer>();
        BindRenderer<ArmorModelFeet>(prefabRenderer, this.footArmor, this.footMeshFilter, this.footRenderer);
        BindRenderer<ArmorModelLegs>(prefabRenderer, this.legArmor, this.legMeshFilter, this.legRenderer);
        BindRenderer<ArmorModelTorso>(prefabRenderer, this.torsoArmor, this.torsoMeshFilter, this.torsoRenderer);
        BindRenderer<ArmorModelHead>(prefabRenderer, this.headArmor, this.headMeshFilter, this.headRenderer);
    }

    [NGCRPC]
    protected void SAAM(int footArmorUID, int legArmorUID, int torsoArmorUID, int headArmorUID)
    {
        if (footArmorUID == 0)
        {
            this.footArmor = null;
        }
        else
        {
            this.footArmor = (ArmorDataBlock) DatablockDictionary.GetByUniqueID(footArmorUID);
        }
        if (legArmorUID == 0)
        {
            this.legArmor = null;
        }
        else
        {
            this.legArmor = (ArmorDataBlock) DatablockDictionary.GetByUniqueID(legArmorUID);
        }
        if (torsoArmorUID == 0)
        {
            this.torsoArmor = null;
        }
        else
        {
            this.torsoArmor = (ArmorDataBlock) DatablockDictionary.GetByUniqueID(torsoArmorUID);
        }
        if (headArmorUID == 0)
        {
            this.headArmor = null;
        }
        else
        {
            this.headArmor = (ArmorDataBlock) DatablockDictionary.GetByUniqueID(headArmorUID);
        }
        this.RebindRenderers();
    }

    [NGCRPC]
    protected void SACH(NetEntityID makingForCharacterIDNow, NetworkMessageInfo info)
    {
        AudioSource audio = base.audio;
        if (audio != null)
        {
            audio.Play();
        }
    }

    [NGCRPC]
    protected void SAKL(NetworkMessageInfo info)
    {
        if (this.CreateRagdoll())
        {
            if (this.footMeshFilter != null)
            {
                this.footMeshFilter.renderer.enabled = false;
            }
            if (this.legMeshFilter != null)
            {
                this.legMeshFilter.renderer.enabled = false;
            }
            if (this.torsoMeshFilter != null)
            {
                this.torsoMeshFilter.renderer.enabled = false;
            }
            if (this.headMeshFilter != null)
            {
                this.headMeshFilter.renderer.enabled = false;
            }
        }
    }

    public MeshRenderer footRenderer
    {
        get
        {
            return ((this.footMeshFilter == null) ? null : (this.footMeshFilter.renderer as MeshRenderer));
        }
    }

    public MeshRenderer headRenderer
    {
        get
        {
            return ((this.headMeshFilter == null) ? null : (this.headMeshFilter.renderer as MeshRenderer));
        }
    }

    public MeshRenderer legRenderer
    {
        get
        {
            return ((this.legMeshFilter == null) ? null : (this.legMeshFilter.renderer as MeshRenderer));
        }
    }

    public MeshRenderer torsoRenderer
    {
        get
        {
            return ((this.torsoMeshFilter == null) ? null : (this.torsoMeshFilter.renderer as MeshRenderer));
        }
    }
}

