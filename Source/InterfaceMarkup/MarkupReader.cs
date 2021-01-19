using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Godot;
using SpartansLib.Common;

namespace MSG.InterfaceMarkup
{
    public class MarkupReader
    {
        //public Parser Parser;
        // public EventReader Eventor;

        public string ThemePath;
        public readonly List<string> namespaces = new List<string>();

        public MarkupReader(string input, bool uri = false)
        {
            // Eventor = new EventReader(input, uri);
            // Eventor.OnReadElement += OnElement;
            // Eventor.OnReadEndElement += OnEndElement;
            // Eventor.OnReadProcessingInstructions += OnProcessorInstruction;
            // Eventor.OnReadText += OnText;
            //GD.Print(c);
        }

        // private void OnProcessorInstruction(EventReader reader)
        // {
        //     switch (reader.Name.ToLower())
        //     {
        //         case "namespace":
        //             namespaces.AddRange(reader.Value.SplitWhitespace());
        //             break;
        //         case "reset":
        //             namespaces.Clear();
        //             break;
        //         case "theme":
        //             ThemePath = reader.Value;
        //             break;
        //     }
        // }

        private Node CurrentNode = new Node();

        private readonly Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // private void OnElement(EventReader reader, IDictionary<string, string> attributes)
        // {
        //     if (reader.Depth == 0 && reader.Name == "scene") return;
        //     foreach (var assm in assemblies)
        //     foreach (var type in assm.GetExportedTypes())
        //         if (namespaces.Contains(type.Namespace) && reader.Name == type.Name &&
        //             typeof(Control).IsAssignableFrom(type))
        //         {
        //             var node = CurrentNode.AddChildAndReturn((Node) Activator.CreateInstance(type));
        //             if (!reader.IsEmptyElement) CurrentNode = node;
        //             return;
        //         }
        //
        //     throw new XmlException($"{reader.Name} type not found, perhaps missing a namespace in the xml.");
        // }
        //
        // void OnEndElement(EventReader reader)
        // {
        //     if (reader.Depth > 0) CurrentNode = CurrentNode.GetParent();
        // }
        //
        // void OnText(EventReader reader)
        // {
        //     if (CurrentNode.HasMethod("is_using_bbcode") && (bool) CurrentNode.Call("is_using_bbcode"))
        //         CurrentNode.Call("append_bbcode", "\n" + reader.Value);
        //     else if (CurrentNode.HasMethod("set_text"))
        //         CurrentNode.Call("set_text", CurrentNode.Call("get_text") + "\n" + reader.Value);
        // }
        //
        //
        // public Node GenerateTree()
        // {
        //     Eventor.Parse();
        //     return CurrentNode;
        // }
    }
}