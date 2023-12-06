using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using CleverCrow.Fluid.BTs.TaskParents;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Decorators;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public ITask node;
    public Port inputPort;
    public Port outputPort;

    public NodeView(ITask node) : base("Assets/Editor/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        CreatStyles();
    }

	public void UpdateNodeView()
	{
		if (node.HasBeenActive)
        {
            AddBorder(Color.green, 3, 10);
            Debug.Log("Ativei nó:" + node.Name);
        }
        else
        {
            RemoveBorder();
            Debug.Log("Desativei nó:" + node.Name);
        }
	}

    void RemoveStyles()
    {
        if (node is TaskRoot)
        {
            RemoveFromClassList("root");
        }
        else if (node is TaskBase)
        {
            RemoveFromClassList("task");
        }
        else if (node is DecoratorBase)
        {
            RemoveFromClassList("decorator");
        }
        else if (node is TaskParentBase)
        {
            RemoveFromClassList("taskparent");
        }
    }
    void CreatStyles()
    {
        if (node is TaskRoot)
        {
            AddToClassList("root");
        }
        else if (node is TaskBase)
        {
            AddToClassList("task");
        }
        else if (node is DecoratorBase)
        {
            AddToClassList("decorator");
        }
        else if (node is TaskParentBase)
        {
            AddToClassList("taskparent");
        }
    }

    void AddBorder(Color color, float width, float roundness)
    {
        style.borderBottomColor = color;
        style.borderBottomWidth = width;
        style.borderBottomLeftRadius = roundness;
        style.borderBottomRightRadius = roundness;

        style.borderTopColor = color;
        style.borderTopWidth = width;
        style.borderTopLeftRadius = roundness;
        style.borderTopRightRadius = roundness;

        style.borderRightColor = color;
        style.borderRightWidth = width;

        style.borderLeftColor = color;
        style.borderLeftWidth = width;
    }

    void RemoveBorder()
    {
        AddBorder(Color.clear, 0, 0);
    }

    void CreateInputPorts()
    {
        if (node is TaskRoot) return;

        inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

        if (inputPort != null)
        {
            inputPort.portName = "";
            //inputPort.style.FlexDirection = FlexDirection.Column;
            inputContainer.Add(inputPort);
        }
    }

    void CreateOutputPorts()
    {
        if (node is TaskBase)
        {
            return;
        }
        else if (node is DecoratorBase || node is TaskRoot)
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            if (outputPort != null)
            {
                outputPort.portName = "";
                outputContainer.Add(outputPort);
            }
        }
        else if (node is TaskParentBase)
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            if (outputPort != null)
            {
                outputPort.portName = "";
                outputContainer.Add(outputPort);
            }
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Vector2 pos = new Vector2(newPos.xMin, newPos.yMin);
        node.SetPosition(pos);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null) {
            OnNodeSelected.Invoke(this);
        }
    }
}
