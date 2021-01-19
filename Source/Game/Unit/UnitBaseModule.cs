using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace MSG.Game.Unit
{
    public enum ModuleIndex {
        Debug,
        Nation,
        Select,
        Group,
        Aim,
        Move,
        Count
    }

    public class UnitModuleStorage : IReadOnlyList<UnitBaseModule>
    {

        public UnitModuleStorage(IUnitController controller)
        {
            _modules = new UnitBaseModule[]
            {
                new UnitDebugModule(controller),
                new UnitNationModule(controller),
                new UnitSelectModule(controller),
                new UnitGroupModule(controller),
                new UnitAimModule(controller),
                new UnitMoveModule(controller)
            };
        }

        private UnitBaseModule[] _modules;

        public UnitBaseModule this[int index] =>_modules[index];
        public UnitBaseModule this[ModuleIndex index] =>_modules[(int)index];

        public int Count => _modules.Length;

        public bool IsReadOnly => true;

        public IEnumerator<UnitBaseModule> GetEnumerator()
        {
            foreach(var module in _modules)
                yield return module;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => _modules.GetEnumerator();

        public PrimT GetStruct<PrimT>(string name) where PrimT : struct
        {
            foreach (var module in _modules)
            {
                var stru = module.GetStruct<PrimT>(name);
                if (stru != null) return stru.Value;
            }
            throw new InvalidOperationException();
        }

        public ClassT GetClass<ClassT>(string name) where ClassT : class
        {
            foreach (var module in _modules)
            {
                var stru = module.GetClass<ClassT>(name);
                if (stru != null) return stru;
            }
            throw new InvalidOperationException();
        }
    }

    public class UnitBaseModule
    {
        public readonly IUnitController UnitController;
        protected UnitBaseModule(IUnitController unitController)
        {
            UnitController = unitController;
        }
        public virtual void Ready() {}
        public virtual void EnterTree() {}
        public virtual void ExitTree() {}
        public virtual void Input(InputEvent @event) {}
        public virtual void UnhandledInput(InputEvent @event) {}
        public virtual void Process(float delta) {}
        public virtual void PhysicsProcess(float delta) {}
        public virtual void Draw() {}

        public virtual PrimT? GetStruct<PrimT>(string name) where PrimT : struct
            => default;

        public virtual ClassT GetClass<ClassT>(string name) where ClassT : class
            => default;
    }
}