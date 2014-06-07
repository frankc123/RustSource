using System;
using System.Runtime.InteropServices;

public class actor : ConsoleSystem
{
    public static bool forceThirdPerson;
    private static float last3rdPersonDistance = 2f;
    private static float last3rdPersonHeight = -0.5f;
    private static float last3rdPersonOffset;
    private static float last3rdPersonYaw;

    private static bool GetCharacterStuff(ref ConsoleSystem.Arg args, out Character character, out CameraMount camera, out ItemRepresentation itemRep, out ArmorModelRenderer armor)
    {
        character = null;
        itemRep = null;
        armor = null;
        camera = CameraMount.current;
        if (camera == null)
        {
            args.ReplyWith("Theres no active camera mount.");
            return false;
        }
        character = IDBase.GetMain(camera) as Character;
        if (character == null)
        {
            args.ReplyWith("theres no character for the current mounted camera");
            return false;
        }
        armor = character.GetLocal<ArmorModelRenderer>();
        InventoryHolder local = character.GetLocal<InventoryHolder>();
        if (local != null)
        {
            itemRep = local.itemRepresentation;
        }
        return true;
    }
}

