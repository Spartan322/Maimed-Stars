using Godot;
using SpartansLib.Structure;

namespace MSG.Script.Unit.Weapon
{
    [Tool]
    public class Arrow : Node2D
    {
        public bool editorOnly = false;
        public Color color = Colors.Black;
        public float width = 5;
        public float length = 3;
        public float pointWidthModifier = 1.2f;
        public bool antialiased = false;

        [Export]
        public bool EditorOnly
        {
            get => editorOnly;
            set
            {
                editorOnly = value;
                Update();
            }
        }

        [Export]
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                Update();
            }
        }

        [Export]
        public float Width
        {
            get => width;
            set
            {
                width = value;
                Update();
            }
        }

        [Export]
        public float Length
        {
            get => length;
            set
            {
                length = value;
                Update();
            }
        }

        [Export]
        public float PointerWidthModifier
        {
            get => pointWidthModifier;
            set
            {
                pointWidthModifier = value;
                Update();
            }
        }

        [Export]
        public bool Antialiased
        {
            get => antialiased;
            set
            {
                antialiased = value;
                Update();
            }
        }

        public override void _Draw()
        {
            //DrawLine(new Vector2(0, 0), new Vector2(Length * Mathf.Cos(Rotation), Length * Mathf.Sin(Rotation)), Color, Width, false);
            //DrawLine(new Vector2(0, 0), new Angle(0).ForwardVector()*Length, Colors.Blue, Width, false);
            if (!EditorOnly || (EditorOnly && Engine.EditorHint))
            {
                var arrowEndLength = new Angle().ForwardVector() * Length;
                DrawLine(new Vector2(), arrowEndLength, Color, Width, Antialiased);
                var points = new Vector2[]
                {
                    arrowEndLength + new Vector2(Width, 0),
                    arrowEndLength + new Vector2(0,
                        0.7071067811865475244008443621048490f * Width * PointerWidthModifier),
                    arrowEndLength + new Vector2(0, -0.7071067811865475244008443621048490f * Width * pointWidthModifier)
                };

                /*DrawLine(arrowEndLength.AddX(Width/2), arrowEndLength + new Angle(180-45).ForwardVector() * Length*0.6f, Color, Width, Antialiased);
                DrawLine(arrowEndLength, arrowEndLength + new Angle(-180+45).ForwardVector() * Length*0.6f, Color, Width, Antialiased);
                DrawRect(new Rect2(arrowEndLength.AddX(Width/2), new Vector2(1,1)), Colors.Blue, true);*/
                DrawPrimitive(points, new[] { Color, Color, Color }, new Vector2[0]);
            }
        }
    }
}
