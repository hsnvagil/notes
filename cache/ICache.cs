using System;
using System.Threading.Tasks;

namespace cache {
    public interface ICache {
        Task Add(string key, string value);
        Task<bool> Remove(string key);
        Task<string> Get(Func<Note, bool> predicate);
        Task SetItems(string value);
    }
}