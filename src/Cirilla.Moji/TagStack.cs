using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirilla.Moji
{
    public class TagStack
    {
        public TagStack()
        {
            tags = new List<IMojiTag>();
        }

        public TagStack(TagStack other)
        {
            tags = other.tags.ToList();
        }

        public bool IsEmpty => tags.Count == 0;
        public IReadOnlyList<IMojiTag> Tags => tags;

        private readonly List<IMojiTag> tags;

        public void Push(IMojiTag tag)
        {
            tags.Add(tag);
        }

        public bool Pop(MojiType type)
        {
            var x = tags.LastOrDefault(x => x.Type == type);
            if (x != default)
                return tags.Remove(x);
            return false;
        }

        public bool Pop(string typeString)
        {
            if (Enum.TryParse<MojiType>(typeString, out var type))
                return Pop(type);
            return false;
        }

        public T? Get<T>() where T : class, IMojiTag
        {
            return tags.Reverse<IMojiTag>().OfType<T>().FirstOrDefault();
        }
    }
}
