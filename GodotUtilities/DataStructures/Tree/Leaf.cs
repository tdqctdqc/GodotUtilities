// using Godot;
// using GodotUtilities.DataStructures.Picker;
// using GodotUtilities.DataStructures.Tree;
//
// namespace GodotUtilities.DataStructures.AggregateTree;
//
// public class Leaf<T> : IAggregate<Leaf<T>, T>
// {
//     IEnumerable<Leaf<T>> INeighbored<Leaf<T>>.Neighbors => Neighbors;
//     IEnumerable<T> IChildrened<T>.Children => Children;
//     
//     public HashSet<Leaf<T>> Neighbors { get; private set; }
//     public HashSet<T> Children { get; private set; }
//
//     public Leaf()
//     {
//         Neighbors = new HashSet<Leaf<T>>();
//         Children = new HashSet<T>();
//     }
//
//     public static List<Leaf<T>> MakeRandomishFirstElementSeed(
//         IEnumerable<T> elements,
//         int leafSize,
//         Func<T, IEnumerable<T>> getNeighbors,
//         Func<T, T, bool> canShare,
//         RandomNumberGenerator random)
//     {
//         var notTaken = elements.EnumerableToHashSet();
//         var getSeed = SeedFuncs.GetFirstRemainingSeed(notTaken);
//         var choose = 
//             PickerFuncs.ChooseRandom(notTaken, leafSize, getNeighbors, canShare, random);
//         return TreeAggregator.Aggregate<T, Leaf<T>>(
//             getSeed, 
//             notTaken, 
//             () => new Leaf<T>(), 
//             choose);
//     }
//     public void AddChild(T t)
//     {
//         Children.Add(t);
//     }
//     public void AddNeighbor(Leaf<T> t)
//     {
//         Neighbors.Add(t);
//     }
// }