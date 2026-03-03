using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Pathfinding.BehaviorTrees
{

    public interface IPolicy 
    {
        bool ShouldReturn(Node.Status status);
    }

    public static class Policies 
    {
        public static readonly IPolicy RunForever = new RunForeverPolicy();
        public static readonly IPolicy RunUntilSuccess = new RunUntilSuccessPolicy();
        public static readonly IPolicy RunUntilFailure = new RunUntilFailurePolicy();
        
        class RunForeverPolicy : IPolicy 
        {
            public bool ShouldReturn(Node.Status status) => false;
        }
        
        class RunUntilSuccessPolicy : IPolicy 
        {
            public bool ShouldReturn(Node.Status status) => status == Node.Status.Success;
        }
        
        class RunUntilFailurePolicy : IPolicy 
        {
            public bool ShouldReturn(Node.Status status) => status == Node.Status.Failure;
        }
    }

    public class Inverter : Node 
    {
        public Inverter(string name) : base(name) { }
        
        public override Status Process() 
        {
            MarkProcessing();
            switch (Children[0].Process()) 
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Failure;
            }
        }
    }

    public class RandomSelector : PrioritySelector 
    {
        //Fisher-Yates_shuffle을 사용. 자세한 원리는 ListShuffle.cs를 참조
        protected override List<Node> SortChildren() => Children.Shuffle().ToList();
        
        public RandomSelector(string name, int priority = 0) : base(name, priority) { }
    }
    
    public class PrioritySelector : Selector
    {
        List<Node> _sortedChildren;

        //각 Child들의 priority를 검사하여 하향식으로 검사 후 정렬. 필요시 OrderByAscending(상향식)으로 수정가능
        List<Node> SortedChildren => _sortedChildren ??= SortChildren();
        protected virtual List<Node> SortChildren() => Children.OrderByDescending(children => children.priority).ToList();

        public PrioritySelector(string name, int priority) : base(name, priority) {}

        public override void Reset()
        {
            base.Reset();
            _sortedChildren = null;
        }

        public override Status Process()
        {
            MarkProcessing();
            foreach(var child in SortedChildren)
            {
                var status = child.Process();
                if (status == Status.Running)
                {
                    return Status.Running;
                }
                if (status == Status.Success)
                {
                    return Status.Success;
                }
            }
            return Status.Failure;
        }
    }

    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name,priority) {}

        // 순서대로 Child를 실행하며 중간에 Failure 노드를 만나도(default) 인덱스를 증가시켜서 다음 노드 검사
        public override Status Process()
        {
            MarkProcessing();
            if(currChild < Children.Count)
            {
                switch(Children[currChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        return Status.Success;
                    default:
                        currChild++;
                        return Status.Running;
                }
            }
            Reset();
            return Status.Failure;
        }
    }
    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name,priority) {}
        
        //중간에 Failure를 반환한 Node를 한번이라도 만나면 Reset(). 끝까지 갔을 시 Success 반환
        public override Status Process()
        {
            MarkProcessing();
            if(currChild < Children.Count)
            {
                switch(Children[currChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currChild++;
                        return currChild == Children.Count ? Status.Success : Status.Running;
                }
            }
            Reset();
            return Status.Success;
        }
    }

    //Node의 Process 정의. 사실상의 Root 노드. 외부에서 BT 호출은 여기서부터 시작.
    // Root 밑의 List로 담겨져 있는 Child 노드들에게 Process를 호출.
    // 각 자식들 역시도 자신의 Child 노드들의 Root가 됨
    public class BehaviorTree : Node
    {
        readonly IPolicy policy;
        
        public BehaviorTree(string name, IPolicy policy = null) : base(name) 
        {
            this.policy = policy ?? Policies.RunForever;
        }
        public override Status Process()
        {
            MarkProcessing();
            Status status = Children[currChild].Process();
            if (policy.ShouldReturn(status)) 
            {
                return status;
            }
            
            currChild = (currChild + 1) % Children.Count;
            return Status.Running;

        }


        //Tree의 구조를 메세지에 호출
        public void PrintTree() 
        {
            StringBuilder sb = new StringBuilder();
            PrintNode(this, 0, sb);
            Debug.Log(sb.ToString());
        }

        static void PrintNode(Node node, int indentLevel, StringBuilder sb) 
        {
            sb.Append(' ', indentLevel * 2).AppendLine(node.name);
            foreach (Node child in node.Children) 
            {
                PrintNode(child, indentLevel + 1, sb);
            }
        }

    }


    //말단 노드
    
    public class Leaf : Node
    {
        readonly INodeStrategy action;

        public Leaf(string name,INodeStrategy action, int priority = 0) : base(name, priority)
        {
            this.action = action;
        }

        public override Status Process()
        {
            MarkProcessing();
            var status = action.Process();
            return status;
        }

        public override void Reset() => action.Reset();
    }


    public class Node
    {
        public enum Status { Success, Failure, Running }

        public static Node LastProcessingNode { get; private set; }

        public readonly string name;
        public readonly int priority;

        public List<Node> Children = new();
        protected int currChild;

        public Node(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }


        public void AddChild(Node child) => Children.Add(child);

        protected void MarkProcessing()
        {
            LastProcessingNode = this;
        }

        public virtual Status Process()
        {
            MarkProcessing();
            return Children[currChild].Process();
        }
        public virtual void Reset()
        {
            currChild = 0;

            foreach(var child in Children)
            {
                child.Reset();
            }

        }

    }



}
