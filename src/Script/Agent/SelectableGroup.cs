﻿using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Script.Agent
{
    public partial class SelectableGroup :
        GameUnit,
        IFormationHolder,
        IList<GameUnit>,
        IReadOnlyList<GameUnit>,
        IComparable<SelectableGroup>,
        IComparableOverlap<SelectableGroup>,
        IEquatable<SelectableGroup>
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://asset/godot-scene/SelectableGroup.tscn");

        private FormationBase _formation;
        public FormationBase Formation
        {
            get => _formation;
            set
            {
                _formation = value;
                _formation.Holder = this;
            }
        }

        private Vector2 minMaxSelectionAlpha = new Vector2(0.65f, 1);

        #region Exports
        private int _nationId;
        [Export]
        public int NationId
        {
            get => _nationId;
            set
            {
                _nationId = value;
                if (!Engine.EditorHint)
                {
                    Manager.RegisterUnit(this);
                    //Instance.State = universe.GetNation(_nationId);
                    //if (Instance.State == null)
                    //universe.Add(Instance);
                }
            }
        }

        [Export] public bool IgnoreMaxSpeed;

        [Export]
        public Vector2 MinMaxSelectionAlpha
        {
            get => minMaxSelectionAlpha;
            set
            {
                minMaxSelectionAlpha = value;
                Update();
            }
        }
        #endregion

        #region Nodes
        [Node("InfoPanel/InfoLabel")] public RichTextLabel InfoLabel;
        [Node] public Control InfoPanel;
        #endregion

        public void OnGroupInfoPanelGuiInput(InputEvent @event)
        {
            if (@event.LeftMouseIsJustPressed())
            {
                // TODO: replace SelectionHandler with SelectionMenu UnitSelectList
                //SelectionHandler.Select(Instance, !InputHandler.AddControlKeyPressed);
                InfoPanel.AcceptEvent();
            }
        }

        public void OnTopSelectionChange(bool isTop)
        {
            if (isTop)
                Modulate = Modulate.A(MinMaxSelectionAlpha.y);
            else
                Modulate = Modulate.A(MinMaxSelectionAlpha.x);
        }

        public void UpdateData()
        {
            if (InfoLabel != null)
                InfoLabel.Text = UnitName + "\n" + Count;
        }

        public override void SelectUpdate(InternalUnitSelectList list)
        {
            foreach(var unit in this)
                unit.SelectUpdate(list);
        }

        public int CompareOverlap(SelectableGroup other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(SelectableGroup other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(SelectableGroup other)
        {
            throw new NotImplementedException();
        }
    }
}