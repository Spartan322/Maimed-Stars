using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using MSG.Script.UI.Game;
using MSG.Script.World;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Script.Unit
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
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://asset/godot-scene/Unit/SelectableGroup.tscn");

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

        private static SelectionMenu _selectionMenu;

        private bool _isReady;
        public override void _Ready()
        {
            base._Ready();
            _isReady = true;
            UpdateData();
        }

        public override void _Process(float delta)
        {
            GlobalRotation = 0;
        }

        protected override GameNation FindNation()
            => GetParent<GameUnit>().Nation;

        [Connect("gui_input", "InfoPanel")]
        public void OnGroupInfoPanelGuiInput(InputEvent @event)
        {
            if (_selectionMenu == null) _selectionMenu = NodeRegistry.Get<SelectionMenu>();
            if (@event.LeftMouseIsJustPressed())
            {
                if (!InputHandler.AddControlKeyPressed) _selectionMenu.Clear();
                _selectionMenu.Add(this);
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
            if (!_isReady) return;
            InfoLabel.Text = UnitName + "\n" + Count;
        }

        public override void SelectUpdate(InternalSelectList list)
        {
            foreach (var unit in this)
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
