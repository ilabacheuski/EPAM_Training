using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Collections.Tasks {

    /// <summary>
    ///  Tree node item 
    /// </summary>
    /// <typeparam name="T">the type of tree node data</typeparam>
    public interface ITreeNode<T> {
        T Data { get; set; }                             // Custom data
        IEnumerable<ITreeNode<T>> Children { get; set; } // List of childrens
    }

    public class Task {

        /// <summary> Generate the Fibonacci sequence f(x) = f(x-1)+f(x-2) </summary>
        /// <param name="count">the size of a required sequence</param>
        /// <returns>
        ///   Returns the Fibonacci sequence of required count
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0</exception>
        /// <example>
        ///   0 => { }  
        ///   1 => { 1 }    
        ///   2 => { 1, 1 }
        ///   12 => { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144 }
        /// </example>
        public static IEnumerable<int> GetFibonacciSequence(int count) {
            // TODO : Implement Fibonacci sequence generator
            if (count < 0) throw new ArgumentException(); // Is it right description of exception in conditions?
            IList<int> List = new List<int>();

            int a = 0, b = 1, tmp;
            for (int i = 0; i < count; ++i)
            {
                tmp = a;
                a = b;
                b = tmp + b;
                List.Add(a);
            }
            return List;
            //throw new NotImplementedException();
        }

        /// <summary>
        ///    Parses the input string sequence into words
        /// </summary>
        /// <param name="reader">input string sequence</param>
        /// <returns>
        ///   The enumerable of all words from input string sequence. 
        /// </returns>
        /// <exception cref="System.ArgumentNullException">reader is null</exception>
        /// <example>
        ///  "TextReader is the abstract base class of StreamReader and StringReader, which ..." => 
        ///   {"TextReader","is","the","abstract","base","class","of","StreamReader","and","StringReader","which",...}
        /// </example>
        public static IEnumerable<string> Tokenize(TextReader reader) {
            char[] delimeters = new[] { ',', ' ', '.', '\t', '\n' };
            // TODO : Implement the tokenizer
            if (reader == null) throw new ArgumentNullException("reader");

            List<string> List = new List<string>();

            StringBuilder str = new StringBuilder();

            while(true)
            {
                int i = reader.Read();

                if (i == -1) break;

                char c = (char)i;

                if (Array.IndexOf(delimeters, c) == -1)
                {
                    str.Append(c);
                    continue;
                }
                if (str.Length > 0)
                {
                    List.Add(str.ToString());
                    str.Clear();
                }
            }

            if (str.Length > 0)
                List.Add(str.ToString());

            return List;
            //throw new NotImplementedException();
        }



        /// <summary>
        ///   Traverses a tree using the depth-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in depth-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  6  7
        ///                  / \     \
        ///                 3   4     8
        ///                     |
        ///                     5   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> DepthTraversalTree<T>(ITreeNode<T> root) {
            // TODO : Implement the tree depth traversal algorithm
            if (root == null) throw new ArgumentNullException("root");

            List<T> List = new List<T>();
            Stack<ITreeNode<T>> stack = new Stack<ITreeNode<T>>();

            stack.Push(root);

            while (stack.Count != 0)
            {
                ITreeNode<T> currNode = stack.Pop();
                List.Add(currNode.Data);

                if (currNode.Children == null) continue;
                foreach (var child in currNode.Children.Reverse()) // Not really good solution, bcs of buffering. Better implement Reverse iterator
                {
                    stack.Push(child);
                }
            }
            return List;
        }

        /// <summary>
        ///   Traverses a tree using the width-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in width-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  3  4
        ///                  / \     \
        ///                 5   6     7
        ///                     |
        ///                     8   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> WidthTraversalTree<T>(ITreeNode<T> root) {
            // TODO : Implement the tree width traversal algorithm

            if (root == null) throw new ArgumentNullException("root");

            List<T> List = new List<T>();
            Queue<ITreeNode<T>> queue = new Queue<ITreeNode<T>>();
            queue.Enqueue(root);

            while (queue.Count != 0)
            {
                var currNode = queue.Dequeue();
                List.Add(currNode.Data);

                if (currNode.Children == null) continue;

                foreach( var child in currNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return List;
        }

        /// <summary>
        ///   Generates all permutations of specified length from source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source array</param>
        /// <param name="count">permutation length</param>
        /// <returns>
        ///    All permuations of specified length
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0 or greater then the source length</exception>
        /// <example>
        ///   source = { 1,2,3,4 }, count=1 => {{1},{2},{3},{4}}
        ///   source = { 1,2,3,4 }, count=2 => {{1,2},{1,3},{1,4},{2,3},{2,4},{3,4}}
        ///   source = { 1,2,3,4 }, count=3 => {{1,2,3},{1,2,4},{1,3,4},{2,3,4}}
        ///   source = { 1,2,3,4 }, count=4 => {{1,2,3,4}}
        ///   source = { 1,2,3,4 }, count=5 => ArgumentOutOfRangeException
        /// </example>
        public static IEnumerable<T[]> GenerateAllPermutations<T>(T[] source, int count) {
            /// Example with {1,2,3,4,5} would be better. It gives more variants of answers. Like count=3:
            ///   source = { 1,2,3,4,5 }, count=1 => {{1},{2},{3},{4},{5}}
            ///   source = { 1,2,3,4,5 }, count=2 => {{1,2},{1,3},{1,4},{1,5},{2,3},{2,4},{2,5},{3,4},{3,5},{4,5}}
            ///   source = { 1,2,3,4,5 }, count=3 => {{1,2,3},{1,2,4},{1,2,5},{1,3,4},{1,3,5},{2,3,4},{2,3,5},{2,4,5},{3,4,5}}
            ///   source = { 1,2,3,4,5,6,7 }, count=5 => {{1,2,3,4,5}, {1,2,3,4,6}, {1,2,3,4,7}, {1,2,3,5,6}, {1,2,3,5,7}, {1,2,3,6,7}, {1,2,4,5,6}, {1,2,4,5,7}, {1,2,4,6,7}, {1,2,4,5,6,7}
            ///                                           {1,3,4,5,6,7},}
            ///                                           {1,4,5,6,7,8} ... {1,4,5,8,9,12} {1,4,5,8,10,12} {1,4,5,8,9,12} {1,4,5,8,9,12}
            ///   source = { 1,2,3,4,5,6,7 }, count=4 => {{1,2,3,4}, ..., {1,2,3,7}, {1,2,4,5}}
            ///   source = { 1,2,3,4,5 }, count=5 => {{1,2,3,4,5}}

            // TODO : Implement GenerateAllPermutations method

            if (count < 0 || count > source.Length) throw new ArgumentOutOfRangeException("count");

            List<T[]> ResultList = new List<T[]>();

            if (count==0)
            {
                return ResultList;
            }

            LinkedList<T> LinkedList = new LinkedList<T>(source);

            //if (LinkedList.Count == 0) throw new ArgumentOutOfRangeException("LinkedList");

            LinkedListNode<T>[] NodesArray = new LinkedListNode<T>[count];
            var LengthNodesArray = NodesArray.Length;

            //Fill initital list
            var FillNode = LinkedList.First;
            for (int i = 0; i < count; i++)
            {
                NodesArray[i] = FillNode;
                FillNode = FillNode.Next;
            }

            int index = LengthNodesArray - 1;

            LinkedListNode<T> CurrNode = NodesArray.Last();
            LinkedListNode<T> NextNode = null;

            LinkedListNode<T> ChkNode = LinkedList.Last;
            for (int i = 1; i < count; i++)
            {
                ChkNode = ChkNode.Previous;
            }

            while (true)
            {
                ResultList.Add(NodesArray.Select(x => x.Value).ToArray());

                //yield return NodesArray.Select(x => x.Value).ToArray();


                if (index == 0 && CurrNode == ChkNode)
                    break;

                //Get index we must shift to next;
                if (NodesArray[index].Next == NextNode)
                {
                    //Find Index
                    while (index != -1 && NodesArray[index].Next == NextNode)
                    {
                        NextNode = NodesArray[index];
                        --index;
                    }

                    //stop if the end
                    if (index == -1)
                        break;

                    CurrNode = NodesArray[index].Next;
                    NodesArray[index] = CurrNode;

                    //Shift all till the end
                    for (var k=index+1; k < LengthNodesArray; ++k)
                    {
                        CurrNode = CurrNode.Next;
                        NodesArray[k] = CurrNode;
                        ++index;

                    }
                    NextNode = null;
                    continue;
                }

                CurrNode = CurrNode.Next;
                NodesArray[index] = CurrNode;
            }

            return ResultList;
        }

    }

    public static class DictionaryExtentions {
        
        /// <summary>
        ///    Gets a value from the dictionary cache or build new value
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dictionary">source dictionary</param>
        /// <param name="key">key</param>
        /// <param name="builder">builder function to build new value if key does not exist</param>
        /// <returns>
        ///   Returns a value assosiated with the specified key from the dictionary cache. 
        ///   If key does not exist than builds a new value using specifyed builder, puts the result into the cache 
        ///   and returns the result.
        /// </returns>
        /// <example>
        ///   IDictionary<int, Person> cache = new SortedDictionary<int, Person>();
        ///   Person value = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should return a loaded Person and put it into the cache
        ///   Person cached = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should get a Person from the cache
        /// </example>
        public static TValue GetOrBuildValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> builder) {
            // TODO : Implement GetOrBuildValue method for cache
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            var val_ = builder();

            dictionary.Add(key, val_);

            return val_;

        }
    }
}
