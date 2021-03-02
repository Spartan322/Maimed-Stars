using System;
using System.Collections.Generic;
using Godot;
using MSG.Engine;
using MSG.Game.Rts.Unit;
using MSG.Script.Game.World;
using MSG.Script.Gui.Game.Select;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.Game.Unit
{
    public partial class GroupUnit :
        BaseUnit,
        IFormationHolder,
        IList<BaseUnit>,
        IReadOnlyList<BaseUnit>,
        IComparable<GroupUnit>,
        //IComparableOverlap<SelectableGroup>,
        IEquatable<GroupUnit>
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://asset/godot-scene/Game/Unit/GroupUnit.tscn");

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

        private static SelectionDisplay _selectionDisplay;

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
            => GetParent<BaseUnit>().Nation;

        [Connect("gui_input", "InfoPanel")]
        public void OnGroupInfoPanelGuiInput(InputEvent @event)
        {
            if (_selectionDisplay == null) _selectionDisplay = NodeRegistry.Get<SelectionDisplay>();
            if (@event.LeftMouseIsJustPressed())
            {
                if (!InputManager.AddControlKeyPressed) _selectionDisplay.Clear();
                _selectionDisplay.Add(this);
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

        public int CompareOverlap(GroupUnit other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(GroupUnit other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(GroupUnit other)
        {
            throw new NotImplementedException();
        }
    }
}
