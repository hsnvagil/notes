using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cache {
    public class CacheController : ICache {
        private readonly Dictionary<string, Note> _cache;

        public CacheController() {
            _cache = new Dictionary<string, Note>();
        }

        public async Task Add(string key, string value) {
            await Task.Run(() => {
                var note = JsonConvert.DeserializeObject<Note>(value);
                if (_cache.ContainsKey(key))
                    _cache[key] = note;
                else
                    _cache.Add(key, note);
            });
        }

        public async Task<bool> Remove(string key) {
            return await Task.Run(() => {
                if (string.IsNullOrWhiteSpace(key) && !_cache.ContainsKey(key)) return false;
                _cache.Remove(key);
                return true;
            });
        }

        public async Task<string> Get(Func<Note, bool> predicate) {
            return await Task.Run(() => {
                var list = (IEnumerable<Note>) _cache.Values;
                list = list.Where(predicate).ToList();
                if (!list.Any()) return null;
                var json = JsonConvert.SerializeObject(list);
                return json;
            });
        }

        public async Task SetItems(string value) {
            await Task.Run(() => {
                var notes = JsonConvert.DeserializeObject<List<Note>>(value);
                foreach (var note in notes) {
                    var key = note.Id.ToString();
                    if (_cache.ContainsKey(key)) _cache.Remove(key);
                    _cache.Add(key, note);
                }
            });
        }
    }
}