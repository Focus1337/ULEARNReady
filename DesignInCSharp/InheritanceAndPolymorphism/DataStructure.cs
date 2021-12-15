using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        public string Product { get; }
        public MessageType Type { get; }
        public MessageTopic Topic { get; }
        
        public Category(string pProduct, MessageType pType, MessageTopic pTopic)
        {
            Product = pProduct;
            Type = pType;
            Topic = pTopic;
        }
        
        public override string ToString() => $"{Product}.{Type}.{Topic}";
        
        public override bool Equals(object obj)
        {
            if (!(obj is Category categoryObj) || categoryObj.GetHashCode() != GetHashCode()) return false;

            return Product == categoryObj.Product && Type == categoryObj.Type && Topic == categoryObj.Topic;
        }

        public override int GetHashCode()
        {
            if (Product == null) return base.GetHashCode();
            var hash = 13;
            hash = (hash * 7) + Product.GetHashCode();
            hash = (hash * 7) + Topic.GetHashCode();
            hash = (hash * 7) + Type.GetHashCode();
            return hash;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Category categoryObj)) return 1;

            var productCompare =
                string.Compare(Product, categoryObj.Product, StringComparison.InvariantCulture);

            var typeCompare = Type.CompareTo(categoryObj.Type);

            var topicCompare = Topic.CompareTo(categoryObj.Topic);

            if (productCompare != 0) return productCompare;
            if (typeCompare != 0) return typeCompare;
            if (topicCompare != 0) return topicCompare;

            return 0;
        }
        
        public static bool operator <(Category a, Category b) => a.CompareTo(b) < 0;
        public static bool operator <=(Category a, Category b) => a.CompareTo(b) <= 0;
        public static bool operator >(Category a, Category b) => a.CompareTo(b) > 0;
        public static bool operator >=(Category a, Category b) => a.CompareTo(b) >= 0;
    }
}