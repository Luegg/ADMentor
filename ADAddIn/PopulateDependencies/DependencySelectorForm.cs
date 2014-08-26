﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using EAAddInFramework;
using AdAddIn.DataAccess;

namespace AdAddIn.PopulateDependencies
{
    public partial class DependencySelectorForm : Form, IDependencySelector
    {
        private readonly ElementRepository Repo;

        public DependencySelectorForm(ElementRepository repo)
        {
            Repo = repo;

            InitializeComponent();

            dependencyTreeView.CheckBoxes = true;
            dependencyTreeView.AfterCheck += dependencyTreeView_AfterCheck;
        }

        public Option<LabeledTree<SolutionInstantiation, EA.Connector>> GetSelectedDependencies(LabeledTree<SolutionInstantiation, EA.Connector> availableDependencies)
        {
            dependencyTreeView.Nodes.Clear();
            dependencyTreeView.Nodes.Add(ToTreeNode(availableDependencies));
            dependencyTreeView.ExpandAll();

            var result = ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return Options.Some(MarkSelectedNodes(availableDependencies, dependencyTreeView.Nodes[0]));
            }
            else
            {
                return Options.None<LabeledTree<SolutionInstantiation, EA.Connector>>();
            }
        }

        private LabeledTree<SolutionInstantiation, EA.Connector> MarkSelectedNodes(LabeledTree<SolutionInstantiation, EA.Connector> problemSpace, TreeNode treeNode)
        {
            var edges = from pair in problemSpace.Edges.Zip(treeNode.Nodes.Cast<TreeNode>())
                        select LabeledTree.Edge(pair.Item1.Label, MarkSelectedNodes(pair.Item1.Target, pair.Item2));

            if (!problemSpace.Label.Instance.IsDefined && treeNode.Checked)
            {
                return LabeledTree.Node(new SolutionInstantiation(problemSpace.Label.Element, selected: true), edges);
            }
            else
            {
                return LabeledTree.Node(problemSpace.Label, edges);
            }
        }

        private TreeNode ToTreeNode(LabeledTree<SolutionInstantiation, EA.Connector> dependencyNode)
        {
            var node = new TreeNode(dependencyNode.Label.Element.Name, ToTreeNodes(dependencyNode.Edges));
            node.Tag = dependencyNode;
            if (dependencyNode.Label.Instance.IsDefined)
                node.ForeColor = System.Drawing.SystemColors.GrayText;
            return node;
        }

        private TreeNode[] ToTreeNodes(IEnumerable<LabeledTree<SolutionInstantiation, EA.Connector>.Edge> edges)
        {
            return edges.Select(edge =>
            {
                var stype = Repo.GetStereotype(edge.Label).Value;
                var label = String.Format("{0}: {1}", stype.DisplayName, edge.Target.Label.Element.Name);
                var node = new TreeNode(label, ToTreeNodes(edge.Target.Edges));
                node.Tag = edge.Target;
                if (edge.Target.Label.Instance.IsDefined)
                    node.ForeColor = System.Drawing.SystemColors.GrayText;
                return node;
            }).ToArray();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
                UpdateAll(dependencyTreeView.Nodes, true);
        }

        private void buttonDeselectAll_Click(object sender, EventArgs e)
        {
            UpdateAll(dependencyTreeView.Nodes, false);
        }

        private void UpdateAll(TreeNodeCollection treeNodeCollection, bool check)
        {
            treeNodeCollection.Cast<TreeNode>().ForEach(node =>
            {
                node.Checked = check;
                UpdateAll(node.Nodes, check);
            });
        }

        private void dependencyTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                e.Node.Parent.AsOption().Do(parent =>
                {
                    parent.Checked = true;
                });
            }
            else
            {
                e.Node.Nodes.Cast<TreeNode>().ForEach(child =>
                {
                    child.Checked = false;
                });
            }
        }
    }
}
