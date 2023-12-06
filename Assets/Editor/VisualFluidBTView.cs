using System;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.TaskParents;
using CleverCrow.Fluid.BTs.TaskParents.Composites;
using CleverCrow.Fluid.BTs.Decorators;


public class VisualFluidBTView : GraphView
{
    public new class UxmlFactory : UxmlFactory<VisualFluidBTView, GraphView.UxmlTraits> {}

    BehaviorTree tree;
    public Action<NodeView> OnNodeSelected;

    public VisualFluidBTView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/VisualFluidBT.uss");
        styleSheets.Add(styleSheet);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // base.BuildContextualMenu(evt);

        // var type = typeof(TestStructure);
        // evt.menu.AppendAction("Create node", (a) => CreateNode(type));

        AddTypesToMenu(evt, TypeCache.GetTypesDerivedFrom<ActionBase>().ToList());
        AddTypesToMenu(evt, TypeCache.GetTypesDerivedFrom<CompositeBase>().ToList());
        AddTypesToMenu(evt, TypeCache.GetTypesDerivedFrom<DecoratorBase>().ToList());
        AddTypesToMenu(evt, TypeCache.GetTypesDerivedFrom<ConditionBase>().ToList());

        evt.menu.AppendAction("Root Node", (a) => { tree.CreateRootNode(); CreateNodeView(tree.Root); PopulateView(tree); });
    }

    private void AddTypesToMenu(ContextualMenuPopulateEvent evt, List<Type> types)
    {
        foreach (var type in types)
        {
            evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (a) => CreateNode(type));
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
                                    endPort.direction != startPort.direction &&
                                    endPort.node != startPort.node).ToList();
    }

    void CreateNode(System.Type type)
    {
        ITask node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    NodeView FindNodeView(ITask node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    public void ResetTree(BehaviorTree tree)
    {
        tree.ResetActiveTasks();

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        tree.allNodes.ForEach(n => {
            n.time = 0.0f;
            n.NeedsToResetHasBeenActive = false;
            n.HasBeenActive = false;
            CreateNodeView(n);
        });

        tree.allNodes.ForEach(n => {
            var children = tree.GetChildren(n);

            if (children != null)
            {
                children.ForEach(c => {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.outputPort.ConnectTo(childView.inputPort);
                    AddElement(edge);
                });
            }
        });

    }

	public void UpdateNodeViews(BehaviorTree tree)
	{
		tree.allNodes.ForEach(n => {
            if (n.NeedsToResetHasBeenActive)
                n.time += Time.deltaTime;

            if (n.time > 0.1f)
            {
                n.time = 0.0f;
                n.NeedsToResetHasBeenActive = false;
                n.HasBeenActive = false;
            }
			FindNodeView(n).UpdateNodeView();
        });
	}
    public void PopulateView(BehaviorTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        tree.allNodes.ForEach(n => {
            if (n.NeedsToResetHasBeenActive)
                n.time += Time.deltaTime;

            if (n.time > 0.1f)
            {
                n.time = 0.0f;
                n.NeedsToResetHasBeenActive = false;
                n.HasBeenActive = false;
            }
            CreateNodeView(n);
        });

        tree.allNodes.ForEach(n => {
            var children = tree.GetChildren(n);

            if (children != null)
            {
                children.ForEach(c => {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.outputPort.ConnectTo(childView.inputPort);
                    AddElement(edge);
                });
            }
        });
    }

    GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem => {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                Debug.Log(childView);
                tree.AddChild(parentView.node, childView.node);
            });
        }

        return graphViewChange;
    }

    void CreateNodeView(ITask node)
    {
        Debug.Log("Create Node View");
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void SaveTree()
    {
        Debug.Log("Saved Tree");
        tree.SaveTree();
    }
}
