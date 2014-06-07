using Facepunch;
using Facepunch.Cursor;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class RPOS : MonoBehaviour
{
    public RPOSInvCellManager _belt;
    public RPOSBumper _bumper;
    [NonSerialized]
    public RPOSInventoryCell _clickedItemCell;
    public GameObject _closeButton;
    public UISlider _foodProgress;
    public UISlider _healthProgress;
    [NonSerialized]
    private readonly ContextClientWorkingCallback _onContextMenuVisible_;
    public GameObject _optionsButton;
    public RPOSPlaqueManager _plaqueManager;
    [SerializeField]
    private UILabel _useHoverLabel;
    [SerializeField]
    private UIPanel _useHoverPanel;
    public UILabel actionLabel;
    public UIPanel actionPanel;
    public UISlider actionProgress;
    private bool awaking;
    public GameObject bottomCenterAnchor;
    public UILabel calorieLabel;
    private RPOSLimitFlags currentLimitFlags;
    public UISprite fadeSprite;
    private bool forceHideInventory;
    private bool forceHideSprites;
    private bool forceHideUseHoverTextCaseContextMenu;
    private bool forceHideUseHoverTextCaseLimitFlags;
    private bool forceOff;
    public static RPOS g_RPOS;
    public UILabel healthLabel;
    public GameObject InfoPanelPrefab;
    public UIPanel[] keepBottom;
    public UIPanel[] keepTop;
    public const RPOSLimitFlags kNoControllableLimitFlags = (RPOSLimitFlags.HideSprites | RPOSLimitFlags.HideContext | RPOSLimitFlags.HideInventory | RPOSLimitFlags.KeepOff);
    private int lastScreenHeight;
    private int lastScreenWidth;
    private Controllable lastUseHoverControllable;
    private IContextRequestablePointText lastUseHoverPointText;
    private IContextRequestableText lastUseHoverText;
    private IContextRequestableUpdatingText lastUseHoverUpdatingText;
    public GameObject LootPanelPrefab;
    [HideInInspector]
    public Color nextFadeColor;
    [HideInInspector]
    public float nextFadeDuration;
    private Controllable observedPlayer;
    private Vector3 pointUseHoverOrigin;
    private Plane pointUseHoverPlane;
    private bool queuedUseHoverText;
    public UILabel radLabel;
    public UISprite radSprite;
    public RPOSItemRightClickMenu rightClickMenu;
    private bool rposModeLock;
    private bool RPOSOn;
    private UnlockCursorNode unlocker;
    private AABBox useHoverLabelBounds;
    private string useHoverText;
    private bool useHoverTextPanelVisible;
    private bool useHoverTextPoint;
    private Vector3? useHoverTextScreenPoint;
    private bool useHoverTextUpdatable;
    public GameObject windowAnchor;
    public List<RPOSWindow> windowList;
    public GameObject WorkbenchPanelPrefab;

    public RPOS()
    {
        this._onContextMenuVisible_ = new ContextClientWorkingCallback(this.OnContextMenuVisible);
    }

    private void Awake()
    {
        this.actionPanel.enabled = false;
        g_RPOS = this;
        try
        {
            this.awaking = true;
            this._bumper.Populate();
            this.unlocker = LockCursorManager.CreateCursorUnlockNode(false, UnlockCursorFlags.AllowSubsetOfKeys, "RPOS UNLOCKER");
            this.SetRPOSModeNoChecks(false);
            UIEventListener listener1 = UIEventListener.Get(this._closeButton);
            listener1.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener1.onClick, new UIEventListener.VoidDelegate(this.OnCloseButtonClicked));
            UIEventListener listener3 = UIEventListener.Get(this._optionsButton);
            listener3.onClick = (UIEventListener.VoidDelegate) Delegate.Combine(listener3.onClick, new UIEventListener.VoidDelegate(this.OnOptionsButtonClicked));
            TweenColor component = this.fadeSprite.GetComponent<TweenColor>();
            component.eventReceiver = base.gameObject;
            component.callWhenFinished = "FadeFinished";
            if (this._onContextMenuVisible_ != null)
            {
                Context.OnClientWorking += this._onContextMenuVisible_;
            }
            this.UseHoverTextInitialize();
        }
        finally
        {
            this.awaking = false;
        }
        using (TempList<RPOSWindow> list = TempList<RPOSWindow>.New(g_windows.allWindows))
        {
            foreach (RPOSWindow window in list)
            {
                InitWindow(window);
            }
        }
    }

    internal static void BeforeRPOSRender_Internal(UICamera uicamera)
    {
        if (g_RPOS != null)
        {
            g_RPOS.UIUpdate(uicamera);
        }
    }

    internal static void BeforeSceneRender_Internal(Camera sceneCamera)
    {
        if (g_RPOS != null)
        {
            g_RPOS.SceneUpdate(sceneCamera);
        }
    }

    public static bool BringToFront(RPOSWindow window)
    {
        window.EnsureAwake<RPOSWindow>();
        g_windows.front = window;
        return g_windows.lastPropertySetSuccess;
    }

    public static void ChangeRPOSMode(bool enable)
    {
        g_RPOS.SetRPOSMode(enable);
    }

    private void CheckUseHoverTextEnabled()
    {
        this.SetHoverTextState(!this.forceHideUseHoverText && this.queuedUseHoverText, this.useHoverText);
    }

    public static void ClearFade()
    {
        g_RPOS.fadeSprite.enabled = false;
        g_RPOS.CancelInvoke("DoFade");
    }

    private void ClearInjury()
    {
        this._plaqueManager.SetPlaqueActive("PlaqueInjury", false);
    }

    public static void CloseLootWindow()
    {
        foreach (RPOSWindow window in g_windows.allWindows)
        {
            if ((window != null) && (window is RPOSLootWindow))
            {
                ((RPOSLootWindow) window).LootClosed();
                break;
            }
        }
    }

    public static void CloseOptions()
    {
    }

    public static void CloseWindowByName(string name)
    {
        using (TempList<RPOSWindow> list = AllWindows)
        {
            foreach (RPOSWindow window in list)
            {
                if ((window != null) && (window.title == name))
                {
                    window.ExternalClose();
                }
            }
        }
    }

    public static void CloseWorkbenchWindow()
    {
        CloseWindowByName("Workbench");
    }

    public void DoFade(float duration, Color col)
    {
        this.fadeSprite.enabled = true;
        TweenColor.Begin(this.fadeSprite.gameObject, duration, col);
    }

    public static void DoFade(float delay, float duration, Color col)
    {
        if (delay <= 0f)
        {
            DoFadeNow(duration, col);
        }
        else
        {
            g_RPOS.nextFadeColor = col;
            g_RPOS.nextFadeDuration = duration;
            g_RPOS.Invoke("Internal_DoFade", delay);
        }
    }

    public static void DoFadeNow(float duration, Color col)
    {
        g_RPOS.DoFade(duration, col);
    }

    [Obsolete("Use RPOS.Hide()")]
    public void DoHide()
    {
        if (this.RPOSOn)
        {
            this.DoToggle();
        }
    }

    private void DoInjuryUpdate()
    {
        FallDamage component = ObservedPlayer.GetComponent<FallDamage>();
        this._plaqueManager.SetPlaqueActive("PlaqueInjury", component.GetLegInjury() > 0f);
    }

    private void DoMetabolismUpdate()
    {
        Metabolism component = ObservedPlayer.GetComponent<Metabolism>();
        this.calorieLabel.text = component.GetCalorieLevel().ToString("N0");
        this.radLabel.text = component.GetRadLevel().ToString("N0");
        this._foodProgress.sliderValue = Mathf.Clamp01(component.GetCalorieLevel() / 3000f);
        this._plaqueManager.SetPlaqueActive("PlaqueHunger", component.GetCalorieLevel() < 500f);
        this._plaqueManager.SetPlaqueActive("PlaqueCold", component.IsCold());
        this._plaqueManager.SetPlaqueActive("PlaqueWarm", component.IsWarm());
        this._plaqueManager.SetPlaqueActive("PlaqueRadiation", component.HasRadiationPoisoning());
        this._plaqueManager.SetPlaqueActive("PlaquePoison", component.IsPoisoned());
        if (component.GetCalorieLevel() < 500f)
        {
            this.calorieLabel.color = Color.red;
        }
        else
        {
            this.calorieLabel.color = Color.white;
        }
    }

    [Obsolete("Use RPOS.Show()")]
    public void DoShow()
    {
        if (!this.RPOSOn)
        {
            this.DoToggle();
        }
    }

    [Obsolete("Use RPOS.Toggle()")]
    public void DoToggle()
    {
        this.SetRPOSMode(!this.RPOSOn);
    }

    private void DoTossItem(byte slot)
    {
        InventoryHolder component = ObservedPlayer.GetComponent<InventoryHolder>();
        if (component != null)
        {
            component.TossItem(slot);
        }
        GUIHeldItem.Get().ClearHeldItem();
    }

    [Obsolete("Use RPOS.SetEquipmentDirty()")]
    public void EquipmentDirty()
    {
        GetWindowByName<RPOSArmorWindow>("Armor").ForceUpdate();
    }

    public void FadeFinished()
    {
        if (this.fadeSprite.color.a == 0f)
        {
            this.fadeSprite.enabled = false;
        }
    }

    public static bool FocusArmor()
    {
        return FocusListedWindow("Armor");
    }

    public static bool FocusInventory()
    {
        return FocusListedWindow("Inventory");
    }

    public static bool FocusListedWindow(string name)
    {
        if (g_RPOS == null)
        {
            return false;
        }
        if (g_RPOS.forceHideInventory)
        {
            return false;
        }
        bool flag = false;
        foreach (RPOSWindow window in g_RPOS.windowList)
        {
            if ((window != null) && (window.title == name))
            {
                if (!g_RPOS.RPOSOn)
                {
                    g_RPOS.SetRPOSMode(true);
                    if (!g_RPOS.RPOSOn)
                    {
                        return false;
                    }
                }
                window.zzz___INTERNAL_FOCUS();
                flag = true;
            }
        }
        return flag;
    }

    [Obsolete("Avoid using this", true)]
    public static RPOS Get()
    {
        return g_RPOS;
    }

    public static IEnumerable<RPOSWindow> GetBumperWindowList()
    {
        RPOS rpos = g_RPOS;
        if (rpos == null)
        {
            Object[] objArray = Object.FindObjectsOfType(typeof(RPOS));
            if (objArray.Length <= 0)
            {
                return new RPOSWindow[0];
            }
            rpos = (RPOS) objArray[0];
        }
        return rpos.windowList;
    }

    public static int GetIndex2D(int x, int y, int width)
    {
        return (x + (y * width));
    }

    [Obsolete("Use var player = RPOS.ObservedPlayer", true)]
    public Controllable GetObservedPlayer()
    {
        return this.observedPlayer;
    }

    public static bool GetObservedPlayerComponent<TComponent>(out TComponent component) where TComponent: Component
    {
        if (g_RPOS != null)
        {
            Controllable observedPlayer = g_RPOS.observedPlayer;
            if (observedPlayer != null)
            {
                component = observedPlayer.GetComponent<TComponent>();
                return (TComponent) component;
            }
        }
        component = null;
        return false;
    }

    public static RPOSItemRightClickMenu GetRightClickMenu()
    {
        return g_RPOS.rightClickMenu;
    }

    internal static RPOSWindow GetWindowAbove(RPOSWindow window)
    {
        RPOSWindow window2;
        return (!GetWindowAbove(window, out window2) ? null : window2);
    }

    internal static bool GetWindowAbove(RPOSWindow window, out RPOSWindow fill)
    {
        if (window == null)
        {
            throw new ArgumentNullException("window");
        }
        int order = window.order;
        if ((order + 1) == WindowCount)
        {
            fill = null;
            return false;
        }
        fill = g_windows.allWindows[order + 1];
        return true;
    }

    internal static RPOSWindow GetWindowBelow(RPOSWindow window)
    {
        RPOSWindow window2;
        return (!GetWindowAbove(window, out window2) ? null : window2);
    }

    internal static bool GetWindowBelow(RPOSWindow window, out RPOSWindow fill)
    {
        if (window == null)
        {
            throw new ArgumentNullException("window");
        }
        int order = window.order;
        if (order == 0)
        {
            fill = null;
            return false;
        }
        fill = g_windows.allWindows[order - 1];
        return true;
    }

    public static RPOSWindow GetWindowByName(string name)
    {
        if (g_RPOS != null)
        {
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if ((window != null) && (window.title == name))
                {
                    RPOSWindow.EnsureAwake(window);
                    return window;
                }
            }
            Debug.Log("GetWindowByName returning null");
        }
        return null;
    }

    public static TRPOSWindow GetWindowByName<TRPOSWindow>(string name) where TRPOSWindow: RPOSWindow
    {
        if (g_RPOS != null)
        {
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if (((window != null) && (window is TRPOSWindow)) && (window.title == name))
                {
                    RPOSWindow.EnsureAwake(window);
                    return (TRPOSWindow) window;
                }
            }
        }
        return null;
    }

    public static void HealthUpdate(float value)
    {
        if (g_RPOS != null)
        {
            g_RPOS.UpdateHealth(value);
        }
    }

    public static void Hide()
    {
        if (g_RPOS != null)
        {
            g_RPOS.DoHide();
        }
    }

    private static void InitWindow(RPOSWindow window)
    {
        if (window != null)
        {
            window.RPOSReady();
            window.CheckDisplay();
        }
    }

    public static void InjuryUpdate()
    {
        g_RPOS.DoInjuryUpdate();
    }

    public void Internal_DoFade()
    {
        this.DoFade(this.nextFadeDuration, this.nextFadeColor);
    }

    public static bool IsObservedPlayer(Controllable controllable)
    {
        return (((g_RPOS != null) && (controllable != null)) && (g_RPOS.observedPlayer == controllable));
    }

    public static void Item_CellDragBegin(RPOSInventoryCell begin)
    {
        if (g_RPOS != null)
        {
            g_RPOS.ItemCellDragBegin(begin);
        }
    }

    public static void Item_CellDragEnd(RPOSInventoryCell begin, RPOSInventoryCell end)
    {
        if (g_RPOS != null)
        {
            g_RPOS.ItemCellDragEnd(begin, end);
        }
    }

    public static void Item_CellDrop(RPOSInventoryCell cell)
    {
        if (g_RPOS != null)
        {
            g_RPOS.ItemCellDrop(cell);
        }
    }

    public static void Item_CellReset()
    {
        if (g_RPOS != null)
        {
            g_RPOS.ItemCellReset();
        }
    }

    public static bool Item_IsClickedCell(RPOSInventoryCell cell)
    {
        return (((g_RPOS != null) && (g_RPOS._clickedItemCell != null)) && (g_RPOS._clickedItemCell == cell));
    }

    public static void ItemCellAltClicked(RPOSInventoryCell cell)
    {
    }

    [Obsolete("Use RPOS.Item_CellClicked()")]
    public void ItemCellClicked(RPOSInventoryCell cell)
    {
        RPOSInventoryCell cell2 = cell;
        bool flag = false;
        byte slot = 0;
        byte num2 = 0;
        Inventory inventory = null;
        Inventory inventory2 = null;
        IInventoryItem item = null;
        IInventoryItem item2 = null;
        if (this._clickedItemCell != null)
        {
            inventory = this._clickedItemCell._displayInventory;
            slot = this._clickedItemCell._mySlot;
            inventory.GetItem(slot, out item);
        }
        inventory2 = cell2._displayInventory;
        num2 = cell2._mySlot;
        inventory2.GetItem(num2, out item2);
        if ((item == null) && (item2 == null))
        {
            Debug.Log("wtf");
        }
        if ((item == null) && (item2 != null))
        {
            this._clickedItemCell = cell;
            item = cell._myDisplayItem;
            flag = true;
        }
        else if ((item != null) && (item2 != null))
        {
            bool shift = Event.current.shift;
            NetEntityID fromInvID = NetEntityID.Get((MonoBehaviour) inventory);
            NetEntityID toInvID = NetEntityID.Get((MonoBehaviour) inventory2);
            if (shift)
            {
                Inventory.ItemCombinePredicted(fromInvID, toInvID, slot, num2);
            }
            else
            {
                Inventory.ItemMergePredicted(fromInvID, toInvID, slot, num2);
            }
            item = null;
            this._clickedItemCell = null;
        }
        else if ((item != null) && (item2 == null))
        {
            NetEntityID yid3 = NetEntityID.Get((MonoBehaviour) inventory2);
            Inventory.ItemMovePredicted(NetEntityID.Get((MonoBehaviour) inventory), yid3, slot, num2);
            this._clickedItemCell = null;
            item = null;
            flag = true;
        }
        if (item != GUIHeldItem.CurrentItem())
        {
            if (item != null)
            {
                if (!flag || !GUIHeldItem.Get().SetHeldItem(cell))
                {
                    GUIHeldItem.Get().SetHeldItem(item);
                }
            }
            else if (flag && (cell != null))
            {
                GUIHeldItem.Get().ClearHeldItem(cell);
            }
            else
            {
                GUIHeldItem.Get().ClearHeldItem();
            }
        }
    }

    [Obsolete("Use RPOS.Item_CellDragBegin()")]
    public void ItemCellDragBegin(RPOSInventoryCell cell)
    {
        this.ItemCellReset();
        this.ItemCellClicked(cell);
    }

    [Obsolete("Use RPOS.Item_CellDragEnd()")]
    public void ItemCellDragEnd(RPOSInventoryCell begin, RPOSInventoryCell end)
    {
        if (end != null)
        {
            GUIHeldItem.Get().ClearHeldItem(end);
        }
        this.ItemCellReset();
        if (((begin != end) && (end != null)) && (begin != null))
        {
            this._clickedItemCell = begin;
            this.ItemCellClicked(end);
        }
    }

    [Obsolete("Use RPOS.Item_CellDrop()")]
    public void ItemCellDrop(RPOSInventoryCell cell)
    {
        if (this._clickedItemCell != null)
        {
            this.ItemCellClicked(cell);
        }
    }

    [Obsolete("Use RPOS.Item_CellReset()")]
    public void ItemCellReset()
    {
        if (this._clickedItemCell != null)
        {
            GUIHeldItem.Get().ClearHeldItem(this._clickedItemCell);
            this._clickedItemCell._displayInventory.MarkSlotDirty(this._clickedItemCell._mySlot);
        }
        else
        {
            GUIHeldItem.Get().ClearHeldItem();
        }
        this._clickedItemCell = null;
    }

    private void LimitInventory(bool limit)
    {
        this.forceHideInventory = limit;
        using (TempList<RPOSWindow> list = AllWindows)
        {
            bool flag = !limit;
            foreach (RPOSWindow window in list)
            {
                if ((window != null) && window.isInventoryRelated)
                {
                    window.bumpersEnabled = flag;
                }
            }
            foreach (RPOSWindow window2 in list)
            {
                if (window2 != null)
                {
                    window2.inventoryHide = limit;
                }
            }
        }
        if (this._belt != null)
        {
            this._belt.GetComponent<UIPanel>().enabled = !limit;
        }
    }

    public static void LocalInventoryModified()
    {
        GetWindowByName("Crafting").GetComponent<RPOSCraftWindow>().LocalInventoryModified();
        SetPlaqueActive("PlaqueCrafting", g_RPOS.observedPlayer.GetComponent<CraftingInventory>().isCrafting);
    }

    public static void MetabolismUpdate()
    {
        g_RPOS.DoMetabolismUpdate();
    }

    public static bool MoveDown(RPOSWindow window)
    {
        return g_windows.MoveDown(window.EnsureAwake<RPOSWindow>());
    }

    public static bool MoveUp(RPOSWindow window)
    {
        return g_windows.MoveUp(window.EnsureAwake<RPOSWindow>());
    }

    public void OnCloseButtonClicked(GameObject go)
    {
        this.SetRPOSMode(false);
    }

    private void OnContextMenuVisible(bool visible)
    {
        this.forceHideUseHoverTextCaseContextMenu = visible;
        this.CheckUseHoverTextEnabled();
    }

    private void OnDestroy()
    {
        if (this.unlocker != null)
        {
            this.unlocker.Dispose();
            this.unlocker = null;
        }
        if (this._onContextMenuVisible_ != null)
        {
            Context.OnClientWorking -= this._onContextMenuVisible_;
        }
    }

    public void OnOptionsButtonClicked(GameObject go)
    {
        OpenOptions();
    }

    public static void OpenInfoWindow(ItemDataBlock itemdb)
    {
    }

    public static void OpenLootWindow(LootableObject lootObj)
    {
        if (g_RPOS != null)
        {
            CloseWindowByName("Crafting");
            Vector3 localPosition = g_RPOS.LootPanelPrefab.transform.localPosition;
            GameObject prefab = null;
            if (lootObj.lootWindowOverride != null)
            {
                prefab = lootObj.lootWindowOverride.gameObject;
            }
            else
            {
                prefab = g_RPOS.LootPanelPrefab;
            }
            GameObject obj3 = NGUITools.AddChild(g_RPOS.bottomCenterAnchor, prefab);
            obj3.GetComponent<RPOSLootWindow>().SetLootable(lootObj, true);
            obj3.transform.localPosition = localPosition;
            BringToFront(obj3.GetComponent<RPOSWindow>());
            g_RPOS.SetRPOSMode(true);
        }
    }

    public static void OpenOptions()
    {
    }

    public static void OpenWorkbenchWindow(WorkBench workbenchObj)
    {
        if (g_RPOS != null)
        {
            GameObject obj2 = NGUITools.AddChild(g_RPOS.windowAnchor, g_RPOS.WorkbenchPanelPrefab);
            obj2.GetComponent<RPOSWorkbenchWindow>().SetWorkbench(workbenchObj);
            BringToFront(obj2.GetComponent<RPOSWindow>());
            g_RPOS.SetRPOSMode(true);
        }
    }

    internal static void RegisterWindow(RPOSWindow window)
    {
        if (window.zzz__index == -1)
        {
            window.zzz__index = g_windows.allWindows.Count;
            g_windows.allWindows.Add(window);
            if ((g_RPOS != null) && !g_RPOS.awaking)
            {
                InitWindow(window);
            }
            g_windows.orderChanged = true;
        }
    }

    private void SceneUpdate(Camera camera)
    {
        this.UseHoverTextThink(camera);
    }

    public static bool SendToBack(RPOSWindow window)
    {
        window.EnsureAwake<RPOSWindow>();
        g_windows.back = window;
        return g_windows.lastPropertySetSuccess;
    }

    public static void SetActionProgress(bool show, string label, float progress)
    {
        if (show)
        {
            if (!string.IsNullOrEmpty(label))
            {
                g_RPOS.actionLabel.text = label;
                g_RPOS.actionLabel.enabled = true;
            }
            else
            {
                g_RPOS.actionLabel.enabled = false;
            }
            g_RPOS.actionProgress.sliderValue = progress;
            g_RPOS.actionPanel.enabled = true;
        }
        else
        {
            g_RPOS.actionPanel.enabled = false;
        }
    }

    public static void SetCurrentFade(Color col)
    {
        g_RPOS.fadeSprite.color = col;
        TweenColor component = g_RPOS.fadeSprite.GetComponent<TweenColor>();
        component.from = col;
        component.to = col;
        component.isFullscreen = true;
        g_RPOS.fadeSprite.enabled = true;
    }

    public static void SetEquipmentDirty()
    {
        if (g_RPOS != null)
        {
            g_RPOS.EquipmentDirty();
        }
    }

    private void SetHoverTextState(bool enable, string text)
    {
        if (this._useHoverLabel != null)
        {
            if (enable && !string.IsNullOrEmpty(text))
            {
                bool flag = false;
                this._useHoverLabel.enabled = true;
                if (this._useHoverLabel.text != text)
                {
                    this._useHoverLabel.text = text;
                    flag = true;
                }
                if (this._useHoverPanel != null)
                {
                    this.useHoverTextPanelVisible = (bool) this.lastUseHoverControllable;
                    if (flag || !this._useHoverPanel.enabled)
                    {
                        this._useHoverPanel.enabled = true;
                        this._useHoverPanel.ManualPanelUpdate();
                        if (flag)
                        {
                            this.useHoverLabelBounds = NGUIMath.CalculateRelativeWidgetBounds(this._useHoverPanel.transform, this._useHoverLabel.transform);
                        }
                    }
                }
            }
            else
            {
                this.useHoverTextPanelVisible = false;
                if (this._useHoverPanel != null)
                {
                    this._useHoverPanel.enabled = false;
                }
                else
                {
                    this._useHoverLabel.enabled = false;
                }
            }
        }
    }

    [Obsolete("Use RPOS.ObservedPlayer = player")]
    public void SetObservedPlayer(Controllable player)
    {
        this.observedPlayer = player;
        RPOSWindow windowByName = GetWindowByName("Inventory");
        if (windowByName != null)
        {
            windowByName.GetComponentInChildren<RPOSInvCellManager>().SetInventory(player.GetComponent<Inventory>(), false);
        }
        PlayerInventory component = player.GetComponent<PlayerInventory>();
        this._belt.CellIndexStart = 30;
        this._belt.SetInventory(component, false);
        RPOSInvCellManager componentInChildren = GetWindowByName("Armor").GetComponentInChildren<RPOSInvCellManager>();
        componentInChildren.CellIndexStart = 0x24;
        componentInChildren.SetInventory(component, false);
        this.SetRPOSMode(false);
        InjuryUpdate();
    }

    public static void SetPlaqueActive(string plaqueName, bool on)
    {
        g_RPOS._plaqueManager.SetPlaqueActive(plaqueName, on);
    }

    private void SetRPOSMode(bool enable)
    {
        if ((enable != this.RPOSOn) && (!this.forceOff || !enable))
        {
            this.SetRPOSModeNoChecks(enable);
        }
    }

    private void SetRPOSModeNoChecks(bool enable)
    {
        if (this.rposModeLock)
        {
            if (enable != this.RPOSOn)
            {
                throw new InvalidOperationException(!enable ? "You cannot turn OFF RPOS while its being turned ON-- check callstack" : "You cannot turn ON RPOS while its being turned OFF-- check callstack");
            }
        }
        else
        {
            try
            {
                this.rposModeLock = true;
                if (this.observedPlayer == null)
                {
                    enable = false;
                }
                bool flag = this.RPOSOn != enable;
                this.RPOSOn = enable;
                using (TempList<RPOSWindow> list = TempList<RPOSWindow>.New(g_windows.allWindows))
                {
                    if (enable)
                    {
                        foreach (RPOSWindow window in list)
                        {
                            if (window != null)
                            {
                                window.RPOSOn();
                            }
                        }
                    }
                    foreach (RPOSWindow window2 in list)
                    {
                        if (window2 != null)
                        {
                            window2.CheckDisplay();
                        }
                    }
                    if (!enable)
                    {
                        foreach (RPOSWindow window3 in list)
                        {
                            if (window3 != null)
                            {
                                window3.RPOSOff();
                            }
                        }
                        this._clickedItemCell = null;
                        GUIHeldItem item = GUIHeldItem.Get();
                        if (item != null)
                        {
                            item.ClearHeldItem();
                        }
                    }
                }
                this._bumper.GetComponent<UIPanel>().enabled = enable;
                UIPanel.Find(this._closeButton.transform).enabled = enable;
                if (this.RPOSOn)
                {
                    this.unlocker.On = true;
                }
                else
                {
                    this.unlocker.TryLock();
                }
                if (flag)
                {
                    this.CheckUseHoverTextEnabled();
                }
            }
            finally
            {
                this.rposModeLock = false;
            }
            ItemToolTip.SetToolTip(null, null);
        }
    }

    public static void Toggle()
    {
        if (g_RPOS != null)
        {
            g_RPOS.DoToggle();
        }
    }

    public static void ToggleOptions()
    {
    }

    public static void TossItem(byte slot)
    {
        if (g_RPOS != null)
        {
            g_RPOS.DoTossItem(slot);
        }
    }

    private void UIUpdate(UICamera camera)
    {
        this.UseHoverTextPostThink(camera.cachedCamera);
    }

    internal static void UnregisterWindow(RPOSWindow window)
    {
    Label_0000:
        if (window.zzz__index > -1)
        {
            bool flag;
            try
            {
                flag = g_windows.allWindows[window.zzz__index] == window;
            }
            catch (IndexOutOfRangeException)
            {
                flag = false;
            }
            if (!flag)
            {
                int index = g_windows.allWindows.IndexOf(window);
                Debug.LogWarning(string.Format("Some how list maintanance failed, stored index was {0} but index of returned {1}", window.zzz__index, index), window);
                window.zzz__index = index;
                goto Label_0000;
            }
            g_windows.allWindows.RemoveAt(window.zzz__index);
            int num2 = window.zzz__index;
            int count = g_windows.allWindows.Count;
            while (num2 < count)
            {
                g_windows.allWindows[num2].zzz__index = num2;
                num2++;
            }
            g_windows.orderChanged = true;
        }
    }

    private void Update()
    {
        HUDIndicator.Step();
        RPOSLimitFlags currentLimitFlags = this.currentLimitFlags;
        PlayerClient localPlayer = PlayerClient.GetLocalPlayer();
        if (localPlayer != null)
        {
            Controllable controllable2;
            Controllable controllable = localPlayer.controllable;
            if ((controllable != null) && ((controllable2 = controllable.masterControllable) != null))
            {
                this.currentLimitFlags = controllable2.rposLimitFlags;
            }
            else
            {
                this.currentLimitFlags = RPOSLimitFlags.HideSprites | RPOSLimitFlags.HideContext | RPOSLimitFlags.HideInventory | RPOSLimitFlags.KeepOff;
            }
        }
        else
        {
            this.currentLimitFlags = RPOSLimitFlags.HideSprites | RPOSLimitFlags.HideContext | RPOSLimitFlags.HideInventory | RPOSLimitFlags.KeepOff;
        }
        if (currentLimitFlags != this.currentLimitFlags)
        {
            RPOSLimitFlags flags2 = currentLimitFlags ^ this.currentLimitFlags;
            if ((flags2 & RPOSLimitFlags.HideContext) == RPOSLimitFlags.HideContext)
            {
                this.forceHideUseHoverTextCaseLimitFlags = (this.currentLimitFlags & RPOSLimitFlags.HideContext) == RPOSLimitFlags.HideContext;
                this.CheckUseHoverTextEnabled();
            }
            if ((flags2 & RPOSLimitFlags.HideSprites) == RPOSLimitFlags.HideSprites)
            {
                this.forceHideSprites = (this.currentLimitFlags & RPOSLimitFlags.HideSprites) == RPOSLimitFlags.HideSprites;
            }
            if ((flags2 & RPOSLimitFlags.HideInventory) == RPOSLimitFlags.HideInventory)
            {
                this.LimitInventory((this.currentLimitFlags & RPOSLimitFlags.HideInventory) == RPOSLimitFlags.HideInventory);
            }
            if ((flags2 & RPOSLimitFlags.KeepOff) == RPOSLimitFlags.KeepOff)
            {
                if ((this.currentLimitFlags & RPOSLimitFlags.KeepOff) == RPOSLimitFlags.KeepOff)
                {
                    if (this.RPOSOn)
                    {
                        this.SetRPOSMode(false);
                    }
                    this.forceOff = true;
                }
                else
                {
                    this.forceOff = false;
                }
            }
        }
        int width = Screen.width;
        int height = Screen.height;
        if ((g_windows.orderChanged || (height != this.lastScreenHeight)) || (width != this.lastScreenWidth))
        {
            g_windows.ProcessDepth(this.windowAnchor.transform);
            this.lastScreenHeight = height;
            this.lastScreenWidth = width;
        }
        if (g_RPOS.observedPlayer != null)
        {
            SetPlaqueActive("PlaqueWorkbench1", g_RPOS.observedPlayer.GetComponent<CraftingInventory>().AtWorkBench());
        }
    }

    [Obsolete("Use RPOS.HealthUpdate(amount)")]
    public void UpdateHealth(float amount)
    {
        this.healthLabel.text = amount.ToString("N0");
        this._healthProgress.sliderValue = Mathf.Clamp01(amount / 100f);
        UIFilledSprite component = this._healthProgress.foreground.GetComponent<UIFilledSprite>();
        if (amount > 75f)
        {
            component.color = Color.green;
        }
        else if (amount > 40f)
        {
            component.color = Color.yellow;
        }
        else
        {
            component.color = Color.red;
        }
    }

    private void UpdateUseHoverTextPlane()
    {
        this.pointUseHoverPlane = new Plane(-this._useHoverPanel.transform.forward, this._useHoverPanel.transform.position);
    }

    public static void UseHoverTextClear()
    {
        g_RPOS.useHoverText = string.Empty;
        g_RPOS.queuedUseHoverText = false;
        g_RPOS.lastUseHoverControllable = null;
        g_RPOS.lastUseHoverText = null;
        g_RPOS.lastUseHoverUpdatingText = null;
        g_RPOS.lastUseHoverPointText = null;
        g_RPOS.useHoverTextUpdatable = false;
        g_RPOS.useHoverTextPoint = false;
        g_RPOS.CheckUseHoverTextEnabled();
    }

    private void UseHoverTextInitialize()
    {
        if (this._useHoverPanel != null)
        {
            this.pointUseHoverOrigin = this._useHoverPanel.transform.localPosition;
            this.UpdateUseHoverTextPlane();
        }
        this.CheckUseHoverTextEnabled();
    }

    private void UseHoverTextMove(Camera sceneCamera, Vector3 worldPoint)
    {
        this.useHoverTextScreenPoint = new Vector3?(sceneCamera.WorldToScreenPoint(worldPoint));
    }

    private void UseHoverTextMoveRevert()
    {
        if (this._useHoverPanel != null)
        {
            this.useHoverTextScreenPoint = null;
            this._useHoverPanel.transform.localPosition = this.pointUseHoverOrigin;
        }
    }

    private void UseHoverTextPostThink(Camera panelCamera)
    {
        if (this._useHoverPanel != null)
        {
            this.UseHoverTextScreen(panelCamera);
        }
    }

    private void UseHoverTextScreen(Camera panelCamera)
    {
        if (this.useHoverTextScreenPoint.HasValue)
        {
            float num;
            Vector3 position = this.useHoverTextScreenPoint.Value;
            this.useHoverTextScreenPoint = null;
            Vector2 vector2 = this.useHoverLabelBounds.min + position;
            Vector2 vector3 = this.useHoverLabelBounds.max + position;
            if (vector2 != vector3)
            {
                if (vector2.x < 0f)
                {
                    if (vector3.x < Screen.width)
                    {
                        position.x -= vector2.x;
                    }
                }
                else if (vector3.x > Screen.width)
                {
                    position.x -= vector3.x - Screen.width;
                }
                if (vector2.y < 0f)
                {
                    if (vector3.y < Screen.height)
                    {
                        position.y -= vector2.y;
                    }
                }
                else if (vector3.y > Screen.height)
                {
                    position.y -= vector3.y - Screen.height;
                }
            }
            Ray ray = panelCamera.ScreenPointToRay(position);
            if (this.pointUseHoverPlane.Raycast(ray, out num))
            {
                this._useHoverPanel.transform.position = ray.GetPoint(num);
                this._useHoverPanel.ManualPanelUpdate();
            }
        }
    }

    public static void UseHoverTextSet(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            UseHoverTextClear();
        }
        else
        {
            g_RPOS.queuedUseHoverText = true;
            g_RPOS.useHoverText = text;
            g_RPOS.lastUseHoverText = null;
            g_RPOS.lastUseHoverControllable = null;
            g_RPOS.lastUseHoverUpdatingText = null;
            g_RPOS.lastUseHoverPointText = null;
            g_RPOS.useHoverTextUpdatable = false;
            g_RPOS.useHoverTextPoint = false;
            g_RPOS.UseHoverTextMoveRevert();
            g_RPOS.CheckUseHoverTextEnabled();
        }
    }

    public static void UseHoverTextSet(Controllable localPlayerControllable, IContextRequestableText text)
    {
        if (text == null)
        {
            UseHoverTextClear();
        }
        else if (g_RPOS.lastUseHoverText != text)
        {
            g_RPOS.lastUseHoverText = text;
            g_RPOS.lastUseHoverUpdatingText = text as IContextRequestableUpdatingText;
            g_RPOS.useHoverTextUpdatable = g_RPOS.lastUseHoverUpdatingText != null;
            g_RPOS.lastUseHoverPointText = text as IContextRequestablePointText;
            g_RPOS.useHoverTextPoint = g_RPOS.lastUseHoverPointText != null;
            if (!g_RPOS.useHoverTextPoint)
            {
                g_RPOS.UseHoverTextMoveRevert();
            }
            g_RPOS.lastUseHoverControllable = localPlayerControllable;
            g_RPOS.useHoverText = text.ContextText(localPlayerControllable);
            g_RPOS.queuedUseHoverText = true;
            g_RPOS.CheckUseHoverTextEnabled();
        }
    }

    private void UseHoverTextThink(Camera sceneCamera)
    {
        this.useHoverTextScreenPoint = null;
        if (!this.forceHideUseHoverText && this.queuedUseHoverText)
        {
            if (!(this.lastUseHoverText is MonoBehaviour))
            {
                this.lastUseHoverControllable = null;
            }
            if (this.lastUseHoverControllable != null)
            {
                string str;
                if (this._useHoverLabel == null)
                {
                    return;
                }
                if (this.useHoverTextUpdatable)
                {
                    string text1 = this.lastUseHoverUpdatingText.ContextTextUpdate(this.lastUseHoverControllable, this.useHoverText);
                    if (text1 != null)
                    {
                        str = text1;
                    }
                    else
                    {
                        str = string.Empty;
                    }
                }
                else
                {
                    str = this.lastUseHoverText.ContextText(this.lastUseHoverControllable);
                }
                if (str != this.useHoverText)
                {
                    this.useHoverText = str;
                    this.SetHoverTextState(true, this.useHoverText);
                }
            }
            else
            {
                this.useHoverTextPanelVisible = false;
                if (this.lastUseHoverText != null)
                {
                    UseHoverTextClear();
                }
            }
            if (this.useHoverTextPanelVisible)
            {
                if (this.useHoverTextPoint)
                {
                    Vector3 vector;
                    if (this.lastUseHoverPointText.ContextTextPoint(out vector))
                    {
                        this.UseHoverTextMove(sceneCamera, vector);
                    }
                    else
                    {
                        this.UseHoverTextMoveRevert();
                    }
                }
                this._useHoverPanel.ManualPanelUpdate();
            }
        }
    }

    public static TempList<RPOSWindow> AllClosedWindows
    {
        get
        {
            TempList<RPOSWindow> list = TempList<RPOSWindow>.New();
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if ((window != null) && window.closed)
                {
                    list.Add(window);
                }
            }
            return list;
        }
    }

    public static TempList<RPOSWindow> AllHidingWindows
    {
        get
        {
            TempList<RPOSWindow> list = TempList<RPOSWindow>.New();
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if ((window != null) && !window.showing)
                {
                    list.Add(window);
                }
            }
            return list;
        }
    }

    public static TempList<RPOSWindow> AllOpenWindows
    {
        get
        {
            TempList<RPOSWindow> list = TempList<RPOSWindow>.New();
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if ((window != null) && window.open)
                {
                    list.Add(window);
                }
            }
            return list;
        }
    }

    public static TempList<RPOSWindow> AllShowingWindows
    {
        get
        {
            TempList<RPOSWindow> list = TempList<RPOSWindow>.New();
            foreach (RPOSWindow window in g_windows.allWindows)
            {
                if ((window != null) && window.showing)
                {
                    list.Add(window);
                }
            }
            return list;
        }
    }

    public static TempList<RPOSWindow> AllWindows
    {
        get
        {
            return TempList<RPOSWindow>.New(g_windows.allWindows);
        }
    }

    public static bool Exists
    {
        get
        {
            return (bool) g_RPOS;
        }
    }

    private bool forceHideUseHoverText
    {
        get
        {
            return ((this.forceHideUseHoverTextCaseContextMenu || this.RPOSOn) || this.forceHideUseHoverTextCaseLimitFlags);
        }
    }

    public static bool hideSprites
    {
        get
        {
            return ((g_RPOS != null) && (g_RPOS.RPOSOn || g_RPOS.forceHideSprites));
        }
    }

    public static bool IsClosed
    {
        get
        {
            return !IsOpen;
        }
    }

    public static bool IsOpen
    {
        get
        {
            return (((g_RPOS != null) && g_RPOS.RPOSOn) && !g_RPOS.awaking);
        }
    }

    public static Controllable ObservedPlayer
    {
        get
        {
            return ((g_RPOS == null) ? null : g_RPOS.observedPlayer);
        }
        set
        {
            if (g_RPOS != null)
            {
                g_RPOS.SetObservedPlayer(value);
            }
        }
    }

    public static int WindowCount
    {
        get
        {
            return g_windows.allWindows.Count;
        }
    }

    private static class g_windows
    {
        public static List<RPOSWindow> allWindows = new List<RPOSWindow>();
        public static bool lastPropertySetSuccess = false;
        public static float lastZ;
        public static bool orderChanged = false;

        public static bool MoveDown(RPOSWindow window)
        {
            if (window == null)
            {
                throw new ArgumentNullException();
            }
            if (window.zzz__index == -1)
            {
                throw new InvalidOperationException("The window was not awake");
            }
            int count = allWindows.Count;
            if (count == 0)
            {
                throw new InvalidOperationException("There definitely should have been a window in the list unless you passed a prefab here or didnt ensure awake.");
            }
            if ((count == 1) || (allWindows[0] == window))
            {
                return false;
            }
            allWindows.Reverse(window.zzz__index - 1, 2);
            allWindows[window.zzz__index].zzz__index = window.zzz__index;
            window.zzz__index--;
            orderChanged = true;
            return true;
        }

        public static bool MoveUp(RPOSWindow window)
        {
            if (window == null)
            {
                throw new ArgumentNullException();
            }
            if (window.zzz__index == -1)
            {
                throw new InvalidOperationException("The window was not awake");
            }
            int count = allWindows.Count;
            if (count == 0)
            {
                throw new InvalidOperationException("There definitely should have been a window in the list unless you passed a prefab here or didnt ensure awake.");
            }
            if ((count == 1) || (allWindows[count - 1] == window))
            {
                return false;
            }
            allWindows.Reverse(window.zzz__index, 2);
            allWindows[window.zzz__index].zzz__index = window.zzz__index;
            window.zzz__index++;
            orderChanged = true;
            return true;
        }

        public static void ProcessDepth(Transform uiRoot)
        {
            orderChanged = false;
            lastZ = 0f;
            UIPanel[] panelArray = (RPOS.g_RPOS == null) ? null : RPOS.g_RPOS.keepBottom;
            if (panelArray != null)
            {
                for (int i = panelArray.Length - 1; i >= 0; i--)
                {
                    if (panelArray[i] != null)
                    {
                        ProcessTransform(panelArray[i].transform, ref lastZ);
                    }
                }
            }
            WindowRect[] rectArray = new WindowRect[allWindows.Count];
            WindowRect a = new WindowRect();
            Matrix4x4 worldToLocalMatrix = uiRoot.worldToLocalMatrix;
            int num2 = 0;
            foreach (RPOSWindow window in allWindows)
            {
                if (window != null)
                {
                    Bounds bounds;
                    ProcessTransform(ref worldToLocalMatrix, window, ref lastZ, out bounds);
                    WindowRect b = new WindowRect(bounds);
                    if (a.empty)
                    {
                        a = b;
                    }
                    else
                    {
                        a = new WindowRect(a, b);
                    }
                    rectArray[num2++] = b;
                }
                else
                {
                    WindowRect rect4 = new WindowRect();
                    rectArray[num2++] = rect4;
                }
            }
            panelArray = (RPOS.g_RPOS == null) ? null : RPOS.g_RPOS.keepTop;
            if (panelArray != null)
            {
                for (int j = 0; j < panelArray.Length; j++)
                {
                    if (panelArray[j] != null)
                    {
                        ProcessTransform(panelArray[j].transform, ref lastZ);
                    }
                }
            }
        }

        private static void ProcessTransform(Transform transform, ref float z)
        {
            AABBox box = NGUIMath.CalculateRelativeWidgetBounds(transform);
            Vector3 localPosition = transform.localPosition;
            localPosition.z = -(z + box.max.z);
            z += box.size.z;
            transform.localPosition = localPosition;
        }

        private static void ProcessTransform(ref Matrix4x4 toRoot, RPOSWindow window, ref float z, out Bounds bounds)
        {
            ProcessTransform(window.transform, ref z);
            Vector4 windowDimensions = window.windowDimensions;
            Matrix4x4 localToWorldMatrix = window.transform.localToWorldMatrix;
            bounds = new Bounds(toRoot.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(new Vector3(windowDimensions.x, windowDimensions.y, 0f))), Vector3.zero);
            bounds.Encapsulate(toRoot.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(new Vector3(windowDimensions.x, windowDimensions.y + windowDimensions.w, 0f))));
            bounds.Encapsulate(toRoot.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(new Vector3(windowDimensions.x + windowDimensions.z, windowDimensions.y, 0f))));
            bounds.Encapsulate(toRoot.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(new Vector3(windowDimensions.x + windowDimensions.z, windowDimensions.y + windowDimensions.w, 0f))));
        }

        public static RPOSWindow back
        {
            get
            {
                return ((allWindows.Count != 0) ? allWindows[0] : null);
            }
            set
            {
                lastPropertySetSuccess = false;
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (value.zzz__index == -1)
                {
                    throw new InvalidOperationException("The window was not awake");
                }
                int count = allWindows.Count;
                if (count == 0)
                {
                    throw new InvalidOperationException("There definitely should have been a window in the list unless you passed a prefab here or didnt ensure awake.");
                }
                if ((count != 1) && (allWindows[0] != value))
                {
                    for (int i = value.zzz__index; i > 0; i--)
                    {
                        RPOSWindow window = allWindows[i - 1];
                        allWindows[i] = window;
                        window.zzz__index = i;
                    }
                    allWindows[0] = value;
                    value.zzz__index = 0;
                    orderChanged = true;
                    lastPropertySetSuccess = true;
                }
            }
        }

        public static RPOSWindow front
        {
            get
            {
                int count = allWindows.Count;
                return ((count != 0) ? allWindows[count - 1] : null);
            }
            set
            {
                lastPropertySetSuccess = false;
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (value.zzz__index == -1)
                {
                    throw new InvalidOperationException("The window was not awake");
                }
                int count = allWindows.Count;
                if (count == 0)
                {
                    throw new InvalidOperationException("There definitely should have been a window in the list unless you passed a prefab here or didnt ensure awake.");
                }
                if ((count != 1) && (allWindows[count - 1] != value))
                {
                    for (int i = value.zzz__index; i < (count - 1); i++)
                    {
                        RPOSWindow window = allWindows[i + 1];
                        allWindows[i] = window;
                        window.zzz__index = i;
                    }
                    allWindows[count - 1] = value;
                    value.zzz__index = count - 1;
                    orderChanged = true;
                    lastPropertySetSuccess = true;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowRect
        {
            public int x;
            public int y;
            public ushort width;
            public ushort height;
            public WindowRect(RPOS.g_windows.WindowRect a, RPOS.g_windows.WindowRect b)
            {
                int num;
                if (a.x < b.x)
                {
                    this.x = a.x;
                    num = (b.x + b.width) - a.x;
                    if (num < a.width)
                    {
                        this.width = a.width;
                    }
                    else
                    {
                        this.width = (ushort) num;
                    }
                }
                else
                {
                    this.x = b.x;
                    num = (a.x + a.width) - b.x;
                    if (num < b.width)
                    {
                        this.width = b.width;
                    }
                    else
                    {
                        this.width = (ushort) num;
                    }
                }
                if (a.y < b.y)
                {
                    this.y = a.y;
                    num = (b.y + b.height) - a.y;
                    if (num < a.height)
                    {
                        this.height = a.height;
                    }
                    else
                    {
                        this.height = (ushort) num;
                    }
                }
                else
                {
                    this.y = b.y;
                    num = (a.y + a.height) - b.y;
                    if (num < b.height)
                    {
                        this.height = b.height;
                    }
                    else
                    {
                        this.height = (ushort) num;
                    }
                }
            }

            public WindowRect(int x, int y, int width, int height)
            {
                if (width < 0)
                {
                    this.x = x + width;
                    this.width = (ushort) -width;
                }
                else
                {
                    this.x = x;
                    this.width = (ushort) width;
                }
                if (height < 0)
                {
                    this.y = y + height;
                    this.height = (ushort) -height;
                }
                else
                {
                    this.y = y;
                    this.height = (ushort) height;
                }
            }

            public WindowRect(int x, int y, ushort width, ushort height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public WindowRect(Bounds bounds)
            {
                Vector2 center = bounds.center;
                Vector2 extents = bounds.extents;
                if (extents.x < 0f)
                {
                    this.x = Mathf.FloorToInt(center.x + extents.x);
                    this.width = (ushort) Mathf.CeilToInt((center.x - extents.x) - this.x);
                }
                else
                {
                    this.x = Mathf.FloorToInt(center.x - extents.x);
                    this.width = (ushort) Mathf.CeilToInt((center.x + extents.x) - this.x);
                }
                if (extents.y < 0f)
                {
                    this.y = Mathf.FloorToInt(center.y + extents.y);
                    this.height = (ushort) Mathf.CeilToInt((center.y - extents.y) - this.y);
                }
                else
                {
                    this.y = Mathf.FloorToInt(center.y - extents.y);
                    this.height = (ushort) Mathf.CeilToInt((center.y + extents.y) - this.y);
                }
            }

            public bool empty
            {
                get
                {
                    return ((this.width == 0) || (this.height == 0));
                }
            }
            public int left
            {
                get
                {
                    return this.x;
                }
            }
            public int right
            {
                get
                {
                    return (this.x + this.width);
                }
            }
            public int top
            {
                get
                {
                    return this.y;
                }
            }
            public int bottom
            {
                get
                {
                    return (this.y + this.height);
                }
            }
            public int center
            {
                get
                {
                    return (this.x + (this.width / 2));
                }
            }
            public int middle
            {
                get
                {
                    return (this.y + (this.height / 2));
                }
            }
            public bool Contains(RPOS.g_windows.WindowRect other)
            {
                return (((this.x >= other.x) ? ((this.x == other.x) && (other.width < this.width)) : (((other.x + other.width) - this.x) <= this.width)) && ((this.y >= other.y) ? ((this.y == other.y) && (other.height < this.height)) : (((other.y + other.height) - this.y) <= this.height)));
            }

            public bool ContainsOrEquals(RPOS.g_windows.WindowRect other)
            {
                return (((other.x != this.x) ? ((this.x < other.x) && (((other.x + other.width) - this.x) <= this.width)) : (other.width <= this.width)) && ((other.y != this.y) ? ((this.y < other.y) && (((other.y + other.height) - this.y) <= this.height)) : (other.height <= this.height)));
            }

            public bool Equals(RPOS.g_windows.WindowRect other)
            {
                return ((((this.width == other.width) && (this.x == other.x)) && (this.y == other.y)) && (this.height == other.height));
            }

            public bool Overlaps(RPOS.g_windows.WindowRect other)
            {
                return (((other.x >= this.x) ? (((this.x - other.x) + this.width) > 0) : ((other.x + other.width) > this.x)) && ((other.y >= this.y) ? (((this.y - other.y) + this.height) > 0) : ((other.y + other.height) > this.y)));
            }

            public bool OverlapsOrTouches(RPOS.g_windows.WindowRect other)
            {
                return (((other.x == this.x) || ((other.x >= this.x) ? (((this.x - other.x) + this.width) >= 0) : ((other.x + other.width) >= this.x))) && ((other.y == this.y) || ((other.y >= this.y) ? (((this.y - other.y) + this.height) >= 0) : ((other.y + other.height) >= this.y))));
            }

            public bool OverlapsOrOutside(RPOS.g_windows.WindowRect other)
            {
                return ((((other.x < this.x) || (other.y < this.y)) || (((this.x - other.x) + other.width) > this.width)) || (((this.y - other.y) + this.height) > this.height));
            }

            public bool OverlapsTouchesOrOutside(RPOS.g_windows.WindowRect other)
            {
                return ((((other.x <= this.x) || (other.y <= this.y)) || (((this.x - other.x) + other.width) >= this.width)) || (((this.y - other.y) + this.height) >= this.height));
            }

            public override string ToString()
            {
                object[] args = new object[] { this.x, this.y, this.width, this.height };
                return string.Format("{{x:{0},y:{1},width:{2},height:{3}}}", args);
            }
        }
    }
}

