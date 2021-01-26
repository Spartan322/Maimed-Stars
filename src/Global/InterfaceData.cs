using System;
using System.Collections.Generic;
using Godot;

namespace MSG.Global
{
    public static class InterfaceData
    {
        private static readonly Dictionary<Type, Control> interfaces = new Dictionary<Type, Control>();

        public static T Get<T>() where T : Control => (T) interfaces[typeof(T)];

        internal static T Set<T>(T obj)
            where T : Control
        {
            interfaces[typeof(T)] = obj;
            return obj;
        }
    }
}