﻿using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerStunEffects : ServerEffectType
    {

        public new string effectType = "Stun";
        public new string[] effectTypeOptions = new string[] { "StunEffect" };

        // Use this for initialization
        public ServerStunEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight*1.5f;
            showTimeFields = true;
            showSkillModFields = false;
            return pos;
        }

        public override string EffectType
        {
            get
            {
                return effectType;
            }
        }

        public override string[] EffectTypeOptions
        {
            get
            {
                return effectTypeOptions;
            }
        }
    }
}