﻿using System.Collections;
using Godot;
using Godot.Collections;
using MSG.Global;
using MSG.Script.UI.Game;
using MSG.Settings;
using SpartansLib.Extensions;

namespace MSG.Script
{
    public class Camera : Camera2D
    {
        public static Camera CurrentCamera { get; private set; }

        [Export] public float PanSpeed = 20;
        [Export] public float ZoomSpeed = 25;
        [Export] public Vector2 ZoomLimits = new Vector2(0.5f, 3);
        [Export] public Vector2 EdgePanMargin = new Vector2(40, 40);

        [Export]
        // <summary>
        ///
        /// </summary>
        public int ZoomStep = 100;

        [Export] public float MinSelectThreshold = 20;

        public float ZoomFactor = 1;

        public Rect2 SelectGlobal { get; set; }
        public Rect2 SelectLocal { get; set; }
        public bool IsSeleting { get; private set; }

        public enum InputPanDirection
        {
            Left,
            Up,
            Right,
            Down,
            Max
        }

        public readonly Dictionary<InputPanDirection, string> InputPan = new Dictionary<InputPanDirection, string>
        {
            {InputPanDirection.Left, "game_cam_move_left"},
            {InputPanDirection.Up, "game_cam_move_up"},
            {InputPanDirection.Right, "game_cam_move_right"},
            {InputPanDirection.Down, "game_cam_move_down"}
        };

        public readonly BitArray PanDirection = new BitArray((int) InputPanDirection.Max);

        public enum InputZoomDirection
        {
            Inward,
            Outward,
            Max
        }

        public readonly Dictionary<InputZoomDirection, string> InputZoom = new Dictionary<InputZoomDirection, string>
        {
            {InputZoomDirection.Inward, "game_cam_zoom_in"},
            {InputZoomDirection.Outward, "game_cam_zoom_out"}
        };

        public readonly BitArray ZoomDirection = new BitArray((int) InputZoomDirection.Max);

        public override void _PhysicsProcess(float delta)
        {
            HandleCameraPhysicsProcess(delta);
        }

        public void Reset()
        {
            PanDirection.SetAll(false);
            ZoomDirection.SetAll(false);
        }

        private void HandleCameraPhysicsProcess(float delta)
        {
            var input = new Vector2(
                PanDirection.GetFloat(InputPanDirection.Right) - PanDirection.GetFloat(InputPanDirection.Left),
                PanDirection.GetFloat(InputPanDirection.Down) - PanDirection.GetFloat(InputPanDirection.Up)
            );
            var vpSize = GetViewportRect().Size;
            if (ControlSettings.EdgeScroll)
            {
                var uscroll = MouseWatcher.MouseOriginLocal - EdgePanMargin;
                var lscroll = MouseWatcher.MouseOriginLocal - (vpSize - EdgePanMargin);
                input += ((uscroll - uscroll.Abs()) / 2 / EdgePanMargin)
                         + ((lscroll + lscroll.Abs()) / 2 / EdgePanMargin);
            }

            Position = Position.LinearInterpolate(Position + input * PanSpeed * Zoom,
                PanSpeed * delta * ControlSettings.CameraMovementMultiplier);

            var nzoom = Zoom.LinearInterpolate(Vector2.One * ZoomFactor, ZoomSpeed * delta);
            if (nzoom < Zoom)
                Position = Position + (-0.5f * vpSize + MouseWatcher.MouseOriginLocal) * (Zoom - nzoom);
            Zoom = nzoom;
        }

        private void HandlePanInput()
        {
            foreach (var pair in InputPan)
                PanDirection.Set(pair.Key, Input.IsActionPressed(pair.Value));
        }

        private void HandleZoomInput()
        {
            foreach (var pair in InputZoom)
                ZoomDirection.Set(pair.Key, Input.IsActionPressed(pair.Value));
            var zoomDelta = ZoomDirection.GetFloat(InputZoomDirection.Outward) -
                            ZoomDirection.GetFloat(InputZoomDirection.Inward);
            if (Mathf.Abs(zoomDelta) >= 1)
                ZoomFactor = Mathf.Clamp(ZoomFactor + zoomDelta / ZoomStep * ZoomSpeed, ZoomLimits.x, ZoomLimits.y);
        }

        private void HandleDragPanInput(InputEvent @event)
        {
            if (InputHandler.MiddleMousePressed)
                Position += (MouseWatcher.PrevMouseOriginLocal - MouseWatcher.MouseOriginLocal) * Zoom;
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("game_option_scroll_edge"))
                ControlSettings.EdgeScroll = !ControlSettings.EdgeScroll;
            // prevents edit focus from overriding camera panning
            if (!PauseMenu.FocusOwnerExist)
                HandlePanInput();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            HandleZoomInput();
            HandleDragPanInput(@event);
        }

        public override void _Notification(int what)
        {
            switch (what)
            {
                case NotificationInternalProcess:
                case NotificationInternalPhysicsProcess:
                case NotificationTransformChanged:
                case NotificationEnterTree:
                    if (CurrentCamera != this && Current) CurrentCamera = this;
                    break;
            }
        }
    }
}