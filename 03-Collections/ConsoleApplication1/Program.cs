using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections.Tasks;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        //class MockNode<T> : ITreeNode<T>
        //{
        //    public T Data { set; get; }
        //    public IEnumerable<ITreeNode<T>> Children { set; get; }
        //}

        //class IntNode : MockNode<int> { }

        //private static IntNode CreateWideTree()
        //{
        //    IntNode root8 = new IntNode() { Data = 8 };
        //    IntNode root7 = new IntNode() { Data = 7 };
        //    IntNode root6 = new IntNode() { Data = 6, Children = new[] { root8 } };
        //    IntNode root5 = new IntNode() { Data = 5 };
        //    IntNode root4 = new IntNode() { Data = 4, Children = new[] { root7 } };
        //    IntNode root3 = new IntNode() { Data = 3 };
        //    IntNode root2 = new IntNode() { Data = 2, Children = new[] { root5, root6 } };
        //    IntNode root1 = new IntNode() { Data = 1, Children = new[] { root2, root3, root4 } };
        //    return root1;
        //}

        static void Main(string[] args)
        {
            //var list = Collections.Tasks.Task.WidthTraversalTree(CreateWideTree());
            foreach (var array in Collections.Tasks.Task.GenerateAllPermutations(new int[] {1,2,3,4,5,6,7,8,9,10,11,12 }, 0))
            {
                Console.WriteLine(string.Join(",", array));
            }
        }
    }
}