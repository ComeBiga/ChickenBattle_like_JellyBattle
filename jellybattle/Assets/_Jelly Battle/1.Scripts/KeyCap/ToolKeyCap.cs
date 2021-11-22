using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolKeyCap : KeyCap
{
    public KeyCap tool;

    public ToolKeyCap(KeyCap _tool)
    {
        tag = "FalseTool";
        tool = _tool;
        type = KeyCapType.FalseTool;
    }

    public override void Use(Player player, Player[] enemies)
    {
        isActive = false;
        tool.UseAsTool(player, enemies);
    }
}
