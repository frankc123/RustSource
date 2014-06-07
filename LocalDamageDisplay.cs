using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class LocalDamageDisplay : IDLocalCharacterAddon
{
    private static bool adminObjectShow;
    [NonSerialized]
    public Texture2D damageOverlay;
    [NonSerialized]
    public Texture2D damageOverlay2;
    private const int kDamageOverlayIndex = 0;
    private const int kDamageOverlayPass = 3;
    private const int kImpactOverlayIndex = 1;
    private const int kImpactOverlayPass = 1;
    protected const IDLocalCharacterAddon.AddonFlags kRequiredAddonFlags = IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake;
    [NonSerialized]
    public float lastHealthPercent;
    private int lastShowFlags;
    [NonSerialized]
    public float lastTakeDamageTime;
    [NonSerialized]
    public BobEffect meleeBob;
    private static int mode;
    private const int mode_count = 2;
    private const int SHOW_DAMAGE_OVERLAY = 1;
    private const int SHOW_IMPACT_OVERLAY = 2;
    [NonSerialized]
    public BobEffect takeDamageBob;

    public LocalDamageDisplay() : this(IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake)
    {
    }

    protected LocalDamageDisplay(IDLocalCharacterAddon.AddonFlags addonFlags) : base((IDLocalCharacterAddon.AddonFlags) ((byte) (addonFlags | IDLocalCharacterAddon.AddonFlags.FireOnAddonAwake)))
    {
        this.lastHealthPercent = 1f;
    }

    private static void DrawLabel(Vector3 point, string label)
    {
        Vector3? nullable = CameraFX.World2Screen(point);
        if (nullable.HasValue)
        {
            Vector3 screenPoint = nullable.Value;
            if (screenPoint.z > 0f)
            {
                Vector2 vector2 = GUIUtility.ScreenToGUIPoint(screenPoint);
                vector2.y = Screen.height - (vector2.y + 1f);
                GUI.color = Color.white;
                GUI.Label(new Rect(vector2.x - 64f, vector2.y - 12f, 128f, 24f), label);
            }
        }
    }

    public void Hurt(float percent, GameObject attacker)
    {
        if (percent >= 0.05f)
        {
            this.lastTakeDamageTime = Time.time;
            if (CameraMount.current != null)
            {
                HeadBob component = CameraMount.current.GetComponent<HeadBob>();
                if (component == null)
                {
                    Debug.Log("no camera headbob");
                }
                if (component != null)
                {
                    bool flag;
                    if (attacker != null)
                    {
                        Controllable controllable = attacker.GetComponent<Controllable>();
                        flag = (controllable != null) && (controllable.npcName == "zombie");
                        if (!flag)
                        {
                            flag = attacker.GetComponent<BasicWildLifeAI>() != null;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    component.AddEffect(!flag ? this.takeDamageBob : this.meleeBob);
                }
            }
        }
    }

    private void LateUpdate()
    {
        float num;
        float num2;
        GameFullscreen instance = ImageEffectManager.GetInstance<GameFullscreen>();
        int num3 = this.UpdateFadeValues(out num, out num2);
        int num4 = num3 ^ this.lastShowFlags;
        this.lastShowFlags = num3;
        if (num4 != 0)
        {
            if ((num4 & 1) == 1)
            {
                if ((num3 & 1) == 1)
                {
                    instance.overlays[0].texture = this.damageOverlay;
                    instance.overlays[0].pass = 3;
                }
                else
                {
                    instance.overlays[0].texture = null;
                }
            }
            if ((num4 & 2) == 2)
            {
                if ((num3 & 2) == 2)
                {
                    instance.overlays[1].texture = this.damageOverlay2;
                    instance.overlays[1].pass = 3;
                }
                else
                {
                    instance.overlays[1].texture = null;
                }
            }
        }
        if ((num3 & 1) == 1)
        {
            instance.overlays[0].alpha = num;
        }
        if ((num3 & 2) == 2)
        {
            instance.overlays[1].alpha = num2;
        }
    }

    protected override void OnAddonAwake()
    {
        CharacterOverlayTrait trait = base.GetTrait<CharacterOverlayTrait>();
        this.damageOverlay = trait.damageOverlay;
        this.damageOverlay2 = trait.damageOverlay2;
        this.takeDamageBob = trait.takeDamageBob as BobEffect;
        this.meleeBob = trait.meleeBob as BobEffect;
    }

    private void OnDisable()
    {
        GameFullscreen instance = ImageEffectManager.GetInstance<GameFullscreen>();
        int lastShowFlags = this.lastShowFlags;
        this.lastShowFlags = 0;
        if ((lastShowFlags & 1) == 1)
        {
            instance.overlays[0].texture = null;
        }
        if ((lastShowFlags & 2) == 2)
        {
            instance.overlays[1].texture = null;
        }
    }

    private void OnGUI()
    {
        if ((Event.current.type == EventType.Repaint) && adminObjectShow)
        {
            GUI.color = Color.white;
            GUI.Box(new Rect(5f, 5f, 128f, 24f), (mode != 0) ? "showing selection" : "showing characters");
            if (mode == 0)
            {
                foreach (Object obj2 in Object.FindObjectsOfType(typeof(Character)))
                {
                    if (obj2 != null)
                    {
                        Character character = (Character) obj2;
                        if (character.gameObject != this)
                        {
                            DrawLabel(character.origin, character.name);
                        }
                    }
                }
            }
        }
    }

    public void SetNewHealthPercent(float newHealthPercent, GameObject attacker)
    {
        if (newHealthPercent < this.lastHealthPercent)
        {
            this.Hurt(newHealthPercent, attacker);
        }
        this.lastHealthPercent = newHealthPercent;
    }

    private void Update()
    {
        if (DebugInput.GetKeyDown(KeyCode.O))
        {
            adminObjectShow = !adminObjectShow;
            if (adminObjectShow)
            {
                Debug.Log("shown object overlay", this);
            }
            else
            {
                Debug.Log("hid object overlay", this);
            }
        }
        if (adminObjectShow && DebugInput.GetKeyDown(KeyCode.L))
        {
            mode = (mode + 1) % 2;
        }
    }

    private int UpdateFadeValues(out float alpha, out float impactAlpha)
    {
        alpha = 1f - this.lastHealthPercent;
        float num = Mathf.Abs(Mathf.Sin(Time.time * 6f));
        int num2 = 0;
        if ((this.lastHealthPercent <= 0.6f) && (alpha > 0f))
        {
            num2 |= 1;
            alpha = ((alpha - 0.6f) * 2.5f) * num;
        }
        impactAlpha = 1f - Mathf.Clamp01((Time.time - this.lastTakeDamageTime) / 0.5f);
        impactAlpha *= 1f;
        if (impactAlpha > 0f)
        {
            num2 |= 2;
        }
        return num2;
    }
}

