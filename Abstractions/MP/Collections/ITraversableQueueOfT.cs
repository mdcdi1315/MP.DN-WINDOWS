namespace MP.Collections
{
    /// <summary>
    /// Specialization of the <see cref="IQueue{T}"/> interface for collections that can be traversed by an index value.
    /// </summary>
    /// <typeparam name="T">The type of the elements that this queue will hold.</typeparam>
    public interface ITraversableQueue<T> : IQueue<T>, ITraversableCollection<T>
    {
        
    }
}
