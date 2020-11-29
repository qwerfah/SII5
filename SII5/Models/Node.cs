using SII5.Helpers;
using System;
using System.Collections.Generic;

namespace SII5.Models
{
    public class Node : Observable
    {
        private string _name = null;
        private Node _parent = null;
        private List<Node> _child = null;
        private NodeParams _params = null;

        public double Distance { get; set; }
        public int Points { get; set; }

        /// <summary> Уникальное имя узла (идентификатор). </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        /// <summary> Родительский элемент узла. </summary>
        public Node Parent
        {
            get => _parent;
            set => Set(ref _parent, value);
        }

        /// <summary> Список потомков данного узла. </summary>
        public List<Node> Child
        {
            get => _child;
            set => Set(ref _child, value);
        }

        /// <summary> Параметры узла дерева. </summary>
        public NodeParams Params
        {
            get => _params;
            set => Set(ref _params, value);
        }

        public Node()
        {
            Name = "node";
            Child = new List<Node>();
            Params = new NodeParams();
        }

        public Node(string name, List<Node> child = null, Node parent = null, NodeParams nodeParams = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Child = (child == null) ? new List<Node>() : child;
            Parent = parent;
            Params = (nodeParams == null) ? new NodeParams() : nodeParams;
        }

        public override string ToString()
        {
            return $"{Name} (Число очков: {Points})";
        }
    }
}
