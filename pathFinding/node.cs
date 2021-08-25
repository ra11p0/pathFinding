using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace pathFinding
{
    public class Node
    {
        public List<string> _nodes = new List<string>();
        public string _name { get; }
        public Button _representation = new Button();
        public Node(string name)
        {
            _name = name;
            _representation.Text = name;
        }
        public void AddNode(string symbol)
        {
            _nodes.Add(symbol);
        }
    }
    public static class PathManagement
    {
        public static void AddPath(ref List<Node> node, params string[] names)
        {
            foreach (string x in names)
            {
                if (node.Any(y => y._name == x))
                {
                    if (names.Length > names.ToList().FindIndex(z => z == x) + 1)
                        node.Find(y => y._name == x)._nodes.Add(names[names.ToList().FindIndex(z => z == x) + 1]);
                    if (0 <= names.ToList().FindIndex(z => z == x) - 1)
                        node.Find(y => y._name == x)._nodes.Add(names[names.ToList().FindIndex(z => z == x) - 1]);
                    node.Find(y => y._name == x)._nodes = node.Find(y => y._name == x)._nodes.Distinct().ToList();
                }
                else
                {
                    node.Add(new Node(x));
                    if (names.Length > names.ToList().FindIndex(z => z == x) + 1)
                        node.Find(y => y._name == x)._nodes.Add(names[names.ToList().FindIndex(z => z == x) + 1]);
                    if (0 <= names.ToList().FindIndex(z => z == x) - 1)
                        node.Find(y => y._name == x)._nodes.Add(names[names.ToList().FindIndex(z => z == x) - 1]);
                }
            }
        }
        static public class Path
        {
            public static List<string> GetPath(string first, string last, List<Node> nodes)
            {
                if (first == last) return new List<string>();
                List<List<String>> firstTree = BuildTree(first, last, nodes);
                if (firstTree.Count() == 0) return new List<string>();
                return FindPath(GetCommonNodesFromTrees(firstTree, BuildTree(last, first, nodes)), nodes);
            }
            private static List<List<String>> BuildTree(string first, string last, List<Node> nodes)
            {
                List<List<string>> tree = new List<List<string>>();
                List<String> visitedNodes = new List<string>();
                tree.Add(new List<string> {first});
                while(true)
                {
                    List<string> connectedNodes = new List<string>();
                    foreach (String currentNode in tree.Last())
                    {
                        List<string> nodesToRemove = new List<string>();
                        connectedNodes.AddRange(nodes.Find(x => x._name == currentNode)._nodes);
                        visitedNodes.Add(currentNode);
                        foreach (string connectedNode in connectedNodes) if(visitedNodes.Contains(connectedNode)) nodesToRemove.Add(connectedNode);
                        foreach (string nodeToRemove in nodesToRemove) connectedNodes.Remove(nodeToRemove);
                        visitedNodes = visitedNodes.Distinct().ToList();
                    }
                    tree.Add(connectedNodes.Distinct().ToList());
                    if (connectedNodes.Count == 0) return new List<List<string>>();
                    if (tree.Last().Contains(last)) break;
                }
                return tree;
            }
            private static List<List<String>> GetCommonNodesFromTrees(List<List<String>> firstTree, List<List<String>> secondTree)
            {
                if (firstTree.Count() != secondTree.Count())
                {
                    MessageBox.Show("Different size of trees.");
                    return new List<List<string>>();
                }
                List<List<String>> outputTree = new List<List<string>>();
                secondTree.Reverse();
                int counter = 0;
                foreach (List<string> currentLineFromFirstTree in firstTree)
                {
                    List<string> commonPart = new List<String>();
                    List<string> currentLineFromSecondTree = secondTree[counter];
                    counter++;
                    foreach(String currentNodeInFirstLine in currentLineFromFirstTree)
                    {
                        if (currentLineFromSecondTree.Contains(currentNodeInFirstLine)) commonPart.Add(currentNodeInFirstLine);
                    }
                    outputTree.Add(commonPart);
                }
                return outputTree;
            }
            private static List<string> FindPath(List<List<String>> tree, List<Node> nodes)
            {
                List<string> path = new List<string>();
                path.Add(tree[0][0]);
                while (true)
                {
                    List<String> connectedToNode = nodes.Find(x => x._name == path.Last())._nodes;
                    foreach (string node in connectedToNode)
                    {
                        if (tree[path.Count()].Contains(node))
                        {
                            path.Add(node);
                            break;
                        }
                    }
                    if (path.Contains(tree.Last().Last())) break;
                }
                return path;
            }
        }
    }
}
