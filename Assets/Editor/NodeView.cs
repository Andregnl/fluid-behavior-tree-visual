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
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            if (outputPort != null)
            {
                outputPort.portName = "";
                outputContainer.Add(outputPort);
            }
        }
        else if (node is TaskParentBase)
        {
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

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
