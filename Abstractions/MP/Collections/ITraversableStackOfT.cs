namespace MP.Collections
{
    /// <summary>
    /// Specialization of the <see cref="IStack{T}"/> interface for collections that can be traversed by an index value.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this stack will hold.</typeparam>
    public interface ITraversableStack<T> : IStack<T>, ITraversableCollection<T>
    {

    }
}
