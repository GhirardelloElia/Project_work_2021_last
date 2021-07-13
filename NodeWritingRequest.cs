using System;
namespace Progetto_Main
{
    public class NodeWritingRequest<T> where T : IConvertible
    {
        public string NodeId { get; }

        public T Value { get; }

        public NodeWritingRequest(
            string nodeId,
            T value)
        {
            NodeId = nodeId;
            Value = value;
        }
    }
}
