using System.Collections;
using System.Collections.Generic;
using Godot;

namespace MSG.Script.Game.World
{

    public class GameWorld : Node, IList<GameNation>
    {
        public class SettingsClass
        {
        }

        public GameDomain Domain { get; private set; }
        public SettingsClass Settings { get; private set; }

        public GameNation EmptyNation { get; private set; }

        private List<GameNation> _nations = new List<GameNation>();

        public override void _Ready()
        {
            Domain = GetParent<GameDomain>();
            EmptyNation = new GameNation(this);
        }

        public override void _ExitTree()
        {
            EmptyNation.QueueFree();
            EmptyNation = null;
        }

        private void _onNationSet(GameNation nation)
        {
            nation.World = this;
        }

        private void _onNationRemoved(GameNation nation, GameWorld newWorld)
        {
            nation.World = newWorld;
        }

        public void Add(GameNation gameNation)
        {
            if (Contains(gameNation)) return;
            _nations.Add(gameNation);
            _onNationSet(gameNation);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<GameNation> GetEnumerator() => _nations.GetEnumerator();

        public int Count => _nations.Count;

        public bool IsReadOnly => false;

        public GameNation this[int index]
        {
            get => _nations[index];
            set
            {
                if (index == Count)
                    Add(value);
                else if (!_nations.Contains(value))
                {
                    _nations[index] = value;
                    _onNationSet(value);
                }
            }
        }

        public bool Remove(GameNation nation)
        {
            _onNationRemoved(nation, null);
            return _nations.Remove(nation);
        }

        public void Clear()
        {
            foreach (var nation in _nations)
                _onNationRemoved(nation, null);
            _nations.Clear();
        }

        public bool Contains(GameNation item)
            => _nations.Contains(item);

        public void CopyTo(GameNation[] array, int arrayIndex)
            => _nations.CopyTo(array, arrayIndex);

        public int IndexOf(GameNation item)
            => _nations.IndexOf(item);

        public void Insert(int index, GameNation item)
        {
            if (_nations.Contains(item)) return;
            _nations.Insert(index, item);
            _onNationSet(item);
        }

        public void RemoveAt(int index)
        {
            _onNationRemoved(_nations[index], null);
            _nations.RemoveAt(index);
        }
    }
}
