using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using SpartansLib;
using SpartansLib.Extensions;

namespace MSG.Engine.Command
{
    [Flags]
    public enum ArgType
    {
        Bool = 0b1,
        Byte = 0b10,
        Char = 0b100,
        Short = 0b1000,
        Int = 0b10000,
        Long = 0b100000,
        Float = 0b1000000,
        Double = 0b10000000,
        Decimal = 0b100000000,
        String = 0b1000000000,
        DateTime = 0b10000000000,
    }

    public class ArgList : IReadOnlyList<string>
    {
        private string[] argList;

        public string FullExecution { get; } = "";

        public ArgList(int capacity)
        {
            argList = new string[capacity];
        }

        public ArgList(IList<string> array, string fullExecution = "") : this(array.Count)
        {
            array.CopyTo(argList, 0);
            FullExecution = fullExecution;
        }

        public ArgList(string commandStr)
        {
            //bool parsingAsString = false;
            //var commandList = new List<string>(commandStr.Split(" ").Length);
            //var saveString = new StringBuilder();
            //foreach(var c in commandStr)
            //{
            //    if (c == '"')
            //    {
            //        parsingAsString = !parsingAsString;
            //        if(!parsingAsString)
            //        {
            //            commandList.Add(saveString.ToString().Trim());
            //            saveString.Clear();
            //        }
            //    }
            //    else if (!parsingAsString && c == ' ')
            //    {
            //        commandList.Add(saveString.ToString().Trim());
            //        saveString.Clear();
            //    }
            //    else if (parsingAsString) saveString.Append(c);
            //}
            //commandList.Add(saveString.ToString().Trim());
            FullExecution = commandStr;
            argList = commandStr.ParseEscapableString().ToArray();
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) return null;
                return argList[index];
            }
        }

        public Vector2? GetVec2(int index)
        {
            var vec2 = new Vector2();
            if (this[index]?.AsVec2(out vec2) ?? false)
                return vec2;
            return null;
        }

        public Vector3? GetVec3(int index)
        {
            var vec3 = new Vector3();
            if (this[index]?.AsVec3(out vec3) ?? false)
                return vec3;
            return null;
        }

        public Rect2? GetRect2(int index)
        {
            var rect2 = new Rect2();
            if (this[index]?.AsRect2(out rect2) ?? false)
                return rect2;
            return null;
        }

        public T? GetAs<T>(int index)
            where T : struct, IConvertible
        {
            if (this[index] != null)
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.IsValid(this[index]))
                    return (T)converter.ConvertFrom(this[index]);
            }

            return null;
        }

        public int Count => argList.Length;

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var s in argList)
                yield return s;
        }

        IEnumerator IEnumerable.GetEnumerator() => argList.GetEnumerator();

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < argList.Length; i++)
            {
                var arg = argList[i];
                arg = arg.Replace("\"", "\\\"");
                if (arg.ContainsAny(" \n\t;"))
                    arg = $"\"{arg}\"";
                result += arg;
            }

            return result;
        }
    }
}