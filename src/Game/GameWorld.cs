using System.Collections;
using System.Collections.Generic;
using Godot;

namespace MSG.Game
{
    public class GameWorldSettings
    {
        public int StateCount = 2;
    }

    public class GameWorld : Reference, IList<GameNation>
    {
        public GameDomain Domain { get; }
        public GameWorldSettings Settings { get; }

        public GameNation EmptyNation { get; }

        private List<GameNation> _nations = new List<GameNation>();

        public GameWorld(GameDomain domain, GameWorldSettings settings = default)
        {
            Domain = domain;
            Settings = settings ?? new GameWorldSettings();
            EmptyNation = new GameNation(this);
        }

        public void Initialize(Script.Game node)
        {
            for (var id = 0; id <= Settings.StateCount; id++)
                Add(new GameNation(this));
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
                if(index == Count)
                    Add(value);
                else if(!_nations.Contains(value))
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
            foreach(var nation in _nations)
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
            if(_nations.Contains(item)) return;
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