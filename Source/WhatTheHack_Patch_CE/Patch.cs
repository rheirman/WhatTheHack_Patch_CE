﻿using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace WhatTheHack_Patch_CE
{
    [StaticConstructorOnStartup]
    public static class Patch
    {
        static Patch()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("WhatTheHack_Patch_CE");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
