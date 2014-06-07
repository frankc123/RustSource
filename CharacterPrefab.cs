using System;
using System.Collections.Generic;

public class CharacterPrefab : NetMainPrefab
{
    public CharacterPrefab() : this(typeof(Character), false, null, false)
    {
    }

    protected CharacterPrefab(Type characterType) : this(characterType, true, null, false)
    {
    }

    protected CharacterPrefab(Type characterType, params Type[] requiredIDLocalComponents) : this(characterType, true, requiredIDLocalComponents, (requiredIDLocalComponents != null) && (requiredIDLocalComponents.Length > 0))
    {
    }

    private CharacterPrefab(Type characterType, bool typeCheck, Type[] requiredIDLocalComponents, bool anyRequiredIDLocalComponents) : base(characterType)
    {
        if (typeCheck && !typeof(Character).IsAssignableFrom(characterType))
        {
            throw new ArgumentOutOfRangeException("type", "type must be assignable to Character");
        }
    }

    protected static Type[] TypeArrayAppend(Type[] mustInclude, Type[] given)
    {
        if ((mustInclude == null) || (mustInclude.Length == 0))
        {
            return given;
        }
        if ((given == null) || (given.Length == 0))
        {
            return mustInclude;
        }
        List<Type> list = new List<Type>(given);
        for (int i = 0; i < mustInclude.Length; i++)
        {
            bool flag = false;
            for (int j = 0; j < given.Length; j++)
            {
                if (mustInclude[i].IsAssignableFrom(given[j]))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                list.Add(mustInclude[i]);
            }
        }
        return list.ToArray();
    }
}

