using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.TreeTraversal
{
    public class Traversal<TTree, TValue>
    {
        Func<TTree, IEnumerable<TValue>> nodeHandler;
        Func<TTree, IEnumerable<TTree>> traveler;
        Func<TTree, bool> isValid;

        public Traversal(Func<TTree, IEnumerable<TTree>> traveler,
            Func<TTree, IEnumerable<TValue>> nodeHandler,
            Func<TTree, bool> isValid)
        {
            this.traveler = traveler;
            this.nodeHandler = nodeHandler;
            this.isValid = isValid;
        }

        public void Traverse(TTree tree, List<TValue> result)
        {
            if (isValid(tree)) result.AddRange(nodeHandler(tree));
            var untreated = traveler(tree);
            foreach (var node in untreated)
                Traverse(node, result);
        }
    }

    public static class Traversal
    {
        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
            Traversal<ProductCategory, Product> traversal = new Traversal<ProductCategory, Product>(
                productCategory => productCategory.Categories,
                productCategory => productCategory.Products,
                productCategory => true);
            List<Product> result = new List<Product>();
            traversal.Traverse(root, result);
            return result;
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            Traversal<Job, Job> traversal = new Traversal<Job, Job>(
                job => job.Subjobs,
                job => new List<Job> {job},
                job => job.Subjobs == null || job.Subjobs.Count == 0);
            List<Job> result = new List<Job>();
            traversal.Traverse(root, result);
            return result;
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            Traversal<BinaryTree<T>, T> traversal = new Traversal<BinaryTree<T>, T>(
                (tree) =>
                {
                    var nodes = new List<BinaryTree<T>>();
                    if ((tree.Left != null && tree.Left.Right != null))
                    {
                        nodes.Add(tree.Left);
                    }

                    if ((tree.Right != null && tree.Right.Left != null))
                    {
                        nodes.Add(tree.Right);
                    }

                    return nodes;
                },
                tree => new List<T> {tree.Value}, tree => true);
            List<T> result = new List<T>();
            traversal.Traverse(root, result);


            Traversal<BinaryTree<T>, T> traversal2 = new Traversal<BinaryTree<T>, T>(
                (tree) =>
                {
                    var nodes = new List<BinaryTree<T>>();
                    if ((tree.Left != null))
                    {
                        nodes.Add(tree.Left);
                    }

                    if ((tree.Right != null))
                    {
                        nodes.Add(tree.Right);
                    }

                    return nodes;
                },

                tree => new List<T> {tree.Value}, tree => true);
            List<T> result2 = new List<T>();

            traversal2.Traverse(root, result2);

            for (int i = 0; i < result.Count; i++)
            {
                result2.Remove(result[i]);
            }

            return result2;
        }

    }
}