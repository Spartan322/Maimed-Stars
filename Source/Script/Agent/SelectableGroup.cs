using System;
using Godot;
using MSG.Game.Unit;
using MSG.Global;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Common;
using SpartansLib.Extensions;

namespace MSG.Script.Agent
{
    public class SelectableGroup :
        Node2D,
        IScriptUnit<BaseUnitGroup<SelectableGroup>>,
        IComparable<SelectableGroup>,
        IComparableOverlap<SelectableGroup>,
        IEquatable<SelectableGroup>
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://Scenes/SelectableGroup.tscn");

        public UnitGroupAgent<SelectableGroup> Instance { get; private set; }
        public BaseUnitGroup<SelectableGroup> Unit { get; private set; }
        IUnit IScriptUnit.Unit => Unit;

        #region Script Exports

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
                    Unit.World.GetNation(_nationId).Add(Unit);
                    //Instance.State = universe.GetNation(_nationId);
                    //if (Instance.State == null)
                    //universe.Add(Instance);
                }
            }
        }

        [Export] public bool IgnoreMaxSpeed;

        private Vector2 minMaxSelectionAlpha = new Vector2(0.65f, 1);

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

        [Node("InfoPanel/InfoLabel")] public RichTextLabel InfoLabel;

        [Node] public Control InfoPanel;

        #endregion Script Exports


        #region Node Overrides

        public SelectableGroup()
        {
            // Instance is needed before _Ready is called @ SelectionMenu.OnAcceptButtonUp()
            // Instance = new UnitGroupAgent<SelectableGroup>(this);
            Unit = new BaseUnitGroup<SelectableGroup>();
        }

        public override void _Ready()
        {
            this.GetFirstAncestorOf<Game>().OnReady += _OnGameReady;
            //Instance.Create();
            //OnTopSelectionChange(Instance.IsTopSelection);
            //GroupInfoLabel = GetNode<RichTextLabel>(GroupInfoLabelPath);
            //GroupInfoPanel = GetNode<Control>(GroupInfoPanelPath);
            //Instance.OnSetAsTopSelection += (agent, menu, isTop) => OnTopSelectionChange(isTop);
            //Instance.OnUpdate += agnt => UpdateData();
        }

        public void _OnGameReady(Game game)
        {
            Unit.Initialize(game.Domain, this);
            NationId = NationId;
        }

        public override void _Process(float delta)
        {
            if (Instance.TryFree()) return;
            if (Visible) GlobalRotation = 0;
        }

        public override void _PhysicsProcess(float delta)
        {
            Instance.PhysicsProcess(delta);
        }

        public override void _ExitTree()
        {
            Instance.Destroy();
        }

        #endregion Node Overrides

        public void OnGroupInfoPanelGuiInput(InputEvent @event)
        {
            if (@event.LeftMouseIsJustPressed())
            {
                SelectionHandler.Select(Instance, !InputHandler.AddControlKeyPressed);
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
                InfoLabel.Text = Instance.Name + "\n" + Instance.Count;
        }

        public void SetSelected(bool value)
        {
            foreach (var i in Instance)
                i.OnSelected(value);
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