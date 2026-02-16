using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pathfinding.BehaviorTrees
{
    public interface INodeStrategy
    {
        Node.Status Process();
        void Reset() {}
    }

    public class NodeAction : INodeStrategy
    {
        readonly Action doingSomething;

        public NodeAction(Action doingSomething)
        {
            //Debug.Log("Node Action is Working");
            this.doingSomething = doingSomething;
        }

        public Node.Status Process()
        {
            doingSomething();
            return Node.Status.Success;
        }
    }

    public class ConditionNode : INodeStrategy
    {
        readonly Func<bool> predicate;

        public ConditionNode(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
    }
    public class PatrolNodeStrategy : INodeStrategy
    {
        Transform entity;
        NavMeshAgent agent;
        List<Transform> WayPoints;
        float normalSpeed;
        int currIndex;
        bool isPathCalculated;

        public PatrolNodeStrategy(Transform entity,NavMeshAgent agent,
                                List<Transform> WayPoints,float normalSpeed = 3f)
        {
            this.entity = entity;
            this.agent = agent;
            this.WayPoints = WayPoints;
            this.normalSpeed = normalSpeed;
        }

        public Node.Status Process()
        {
            if(currIndex == WayPoints.Count) return Node.Status.Success;

            var target = WayPoints[currIndex];
            agent.SetDestination(target.position);
            entity.LookAt(target);

            if(isPathCalculated && agent.remainingDistance < 0.1f)
            {
                currIndex++;
                isPathCalculated = false;
            }

            if (agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;

        }

        public void Reset() => currIndex = 0;
    }
}

