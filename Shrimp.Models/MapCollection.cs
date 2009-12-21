using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public class MapCollection : Model
    {
        private class Node
        {
            public Node(int id, Map map, bool isExpanded)
            {
                this.Id = id;
                this.Map = map;
                this.IsExpanded = isExpanded;
                this.Children = new List<Node>();
            }

            public int Id { get; private set; }
            public virtual Map Map { get; private set; }
            public bool IsExpanded { get; set; }
            public Node Parent { get; set; }
            public List<Node> Children { get; private set; }

            public virtual JToken ToJson()
            {
                return new JObject(
                    new JProperty("Id", this.Id),
                    new JProperty("IsExpanded", this.IsExpanded),
                    new JProperty("Map", this.Map.ToJson()),
                    new JProperty("Children",
                        new JArray(this.Children.Select(n => n.ToJson()))));
            }

            public virtual string Name
            {
                get { return this.Map.Name; }
            }
        }

        private abstract class RootNode : Node
        {
            public RootNode(int id, string name, bool isExpanded)
                : base(id, null, isExpanded)
            {
                this.name = name;
            }

            public override JToken ToJson()
            {
                return new JObject(
                    new JProperty("Id", this.Id),
                    new JProperty("IsExpanded", this.IsExpanded),
                    new JProperty("Children",
                        new JArray(this.Children.Select(n => n.ToJson()))));
            }

            public override Map Map
            {
                get { return null; }
            }

            public override string Name
            {
                get { return this.name; }
            }
            private string name;
        }

        private class ProjectNode : RootNode
        {
            public ProjectNode(bool isExpanded) : base(0, "Project", isExpanded) { }
        }

        private class TrashNode : RootNode
        {
            public TrashNode(bool isExpanded) : base(-1, "Trash", isExpanded) { }
        }

        public MapCollection(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ProjectNodeInstance = new ProjectNode(false);
            this.TrashNodeInstance = new TrashNode(false);
            this.Clear();
        }

        public ViewModel ViewModel { get; private set; }

        private Node ProjectNodeInstance;
        private Node TrashNodeInstance;

        private IEnumerable<Node> Nodes
        {
            get
            {
                foreach (Node node in this.RootNodes)
                {
                    foreach (Node node2 in this.Traverse(node))
                    {
                        yield return node2;
                    }
                }
            }
        }

        private IEnumerable<Node> Traverse(Node node)
        {
            yield return node;
            foreach (Node child in node.Children)
            {
                foreach (Node child2 in this.Traverse(child))
                {
                    yield return child2;
                }
            }
        }

        public IEnumerable<int> NodeIds
        {
            get { return this.Nodes.Select(n => n.Id); }
        }

        public int[] Roots
        {
            get { return this.RootNodes.Select(n => n.Id).ToArray(); }
        }

        private Node[] RootNodes
        {
            get { return new[] { this.ProjectNodeInstance, this.TrashNodeInstance }; }
        }

        public int ProjectNodeId
        {
            get { return this.ProjectNodeInstance.Id; }
        }

        public int TrashNodeId
        {
            get { return this.TrashNodeInstance.Id; }
        }

        private Node GetNode(int id)
        {
            Node node = this.Nodes.FirstOrDefault(n => n.Id == id);
            if (node == null)
            {
                throw new ArgumentException("Node not found", "id");
            }
            return node;
        }

        private bool TryGetNode(int id, out Node node)
        {
            node = this.Nodes.FirstOrDefault(n => n.Id == id);
            return node != null;
        }

        public Map GetMap(int id)
        {
            return this.GetNode(id).Map;
        }

        public string GetName(int id)
        {
            return this.GetNode(id).Name;
        }

        public bool TryGetMap(int id, out Map map)
        {
            map = null;
            Node node;
            if (this.TryGetNode(id, out node) && node.Parent != null)
            {
                map = node.Map;
            }
            return map != null;
        }

        public bool IsExpanded(int id)
        {
            return this.GetNode(id).IsExpanded;
        }

        public int GetParent(int id)
        {
            return this.GetNode(id).Parent.Id;
        }

        public int GetRoot(int id)
        {
            Node node = this.GetNode(id);
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node.Id;
        }

        public int[] GetChildren(int id)
        {
            return this.GetNode(id).Children.Select(n => n.Id).ToArray();
        }

        public int Add(int parentId)
        {
            var ids = this.Nodes.Select(n => n.Id);
            if (!ids.Contains(parentId))
            {
                throw new ArgumentException("Invalid id", "parentId");
            }
            int id = Util.GetNewId(this.Nodes.Select(n => n.Id));
            Node node = new Node(id, new Map(this, id), false);
            node.Parent = this.GetNode(parentId);
            node.Parent.Children.Add(node);
            this.OnNodeAdded(new NodeEventArgs(id));
            return id;
        }

        public void Remove(int id)
        {
            if (this.Roots.Contains(id))
            {
                throw new ArgumentException("Couldn't remove the root", "id");
            }
            Node node = this.GetNode(id);
            Node parentNode = node.Parent;
            Debug.Assert(parentNode != null);
            Debug.Assert(parentNode.Children.Contains(node));
            parentNode.Children.Remove(node);
            this.OnNodeRemoved(new NodeEventArgs(id));
        }

        public void Move(int id, int parentId)
        {
            if (this.Roots.Contains(id))
            {
                throw new ArgumentException("Couldn't move the root", "id");
            }
            Node node = this.GetNode(id);
            Node newParentNode = this.GetNode(parentId);
            Node oldParentNode = node.Parent;
            Debug.Assert(oldParentNode != null);
            Debug.Assert(oldParentNode.Children.Contains(node));
            oldParentNode.Children.Remove(node);
            newParentNode.Children.Add(node);
            node.Parent = newParentNode;
            this.OnNodeMoved(new NodeEventArgs(id));
        }

        public void ExpandNode(int id)
        {
            Node node = this.GetNode(id);
            if (node.IsExpanded != true)
            {
                node.IsExpanded = true;
                this.OnUpdated(new UpdatedEventArgs(null));
            }
        }

        public void CollapseNode(int id)
        {
            Node node = this.GetNode(id);
            if (node.IsExpanded != false)
            {
                node.IsExpanded = false;
                this.OnUpdated(new UpdatedEventArgs(null));
            }
        }

        public override JToken ToJson()
        {
            return new JObject(
                new JProperty("Project", this.ProjectNodeInstance.ToJson()),
                new JProperty("Trash", this.TrashNodeInstance.ToJson()));
        }

        public override void LoadJson(JToken json)
        {
            this.Clear();
            JObject projectJson = json["Project"] as JObject;
            JObject trashJson = json["Trash"] as JObject;
            this.ProjectNodeInstance.IsExpanded = projectJson.Value<bool>("IsExpanded");
            this.TrashNodeInstance.IsExpanded = projectJson.Value<bool>("IsExpanded");
            foreach (JObject childJson in projectJson["Children"])
            {
                this.AddNodeFromJson(this.ProjectNodeInstance, childJson);
            }
            foreach (JObject childJson in trashJson["Children"])
            {
                this.AddNodeFromJson(this.TrashNodeInstance, childJson);
            }
        }

        private void AddNodeFromJson(Node parentNode, JObject json)
        {
            int id = json.Value<int>("Id");
            Map map = new Map(this, id);
            map.LoadJson(json["Map"]);
            Node node = new Node(id, map, json.Value<bool>("IsExpanded"));
            parentNode.Children.Add(node);
            node.Parent = parentNode;
            foreach (JObject childJson in json["Children"])
            {
                this.AddNodeFromJson(node, childJson);
            }
        }

        public override void Clear()
        {
            this.ProjectNodeInstance.Children.Clear();
            this.TrashNodeInstance.Children.Clear();
        }

        public event EventHandler<NodeEventArgs> NodeAdded;
        protected virtual void OnNodeAdded(NodeEventArgs e)
        {
            if (this.NodeAdded != null) { this.NodeAdded(this, e); }
            this.OnUpdated(new UpdatedEventArgs(null));
        }

        public event EventHandler<NodeEventArgs> NodeRemoved;
        protected virtual void OnNodeRemoved(NodeEventArgs e)
        {
            if (this.NodeRemoved != null) { this.NodeRemoved(this, e); }
            this.OnUpdated(new UpdatedEventArgs(null));
        }

        public event EventHandler<NodeEventArgs> NodeMoved;
        protected virtual void OnNodeMoved(NodeEventArgs e)
        {
            if (this.NodeMoved != null) { this.NodeMoved(this, e); }
            this.OnUpdated(new UpdatedEventArgs(null));
        }
    }

    public class NodeEventArgs : EventArgs
    {
        public NodeEventArgs(int nodeId)
        {
            this.NodeId = nodeId;
        }

        public int NodeId { get; private set; }
    }
}
