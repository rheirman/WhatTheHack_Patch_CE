using CombatExtended;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using WhatTheHack;

namespace WhatTheHack_Patch_CE
{
    [HarmonyPatch(typeof(JobGiver_TakeAndEquip), "TryGiveJob")]
    class CE_JobGiver_TakeAndEquip_TryGiveJob
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instruction.operand == typeof(Pawn).GetMethod("get_RaceProps"))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CE_JobGiver_TakeAndEquip_TryGiveJob), "ShouldReload", new Type[] { typeof(Pawn) }));//Injected code     
                }
                else if (instruction.operand == AccessTools.Method(typeof(RaceProperties), "get_Humanlike"))
                {
                    //Ommit this instruction
                }
                else
                {
                    yield return instruction;
                }

            }
        }
        static public bool ShouldReload(Pawn p)
        {
            //For mechanoids replace the check of is p.RaceProps.HumanLike by custom logic
            if (p.RaceProps.IsMechanoid && p.IsHacked())
            {
                //return true when a mechanoid is hacked and does not have much ammo. 
                CompInventory inventory = p.TryGetComp<CompInventory>();
                ThingWithComps primaryEq = p.equipment.Primary;
                if (inventory != null && primaryEq != null)
                {
                    CompAmmoUser ammoUser = primaryEq.TryGetComp<CompAmmoUser>();
                    if(ammoUser != null)
                    {
                        int ammoCount = inventory.AmmoCountOfDef(ammoUser.CurrentAmmo);
                        int minAmmo = ammoUser.Props.magazineSize == 0 ? 10 : ammoUser.Props.magazineSize; //No magic numbers?
                        if (ammoCount < minAmmo)
                        {
                            return true;
                        }
                    }
                }
            }
            return p.RaceProps.Humanlike;
        }
    }
}
