using System;
using System.Collections.Generic;
using System.Linq;
using server.EF;
using server.Model;

namespace server.Controller {
    public class Controller {
        public IEnumerable<Note> GetNotes(Func<Note, bool> predicate = null) {
            var query = Service.GetNotes();
            if (predicate != null) query = query.Where(predicate);

            return query;
        }

        public Note CreateNote(Note newNote) {
            return Service.CreateNote(newNote);
        }

        public bool DeleteNote(int id) {
            return Service.DeleteNote(id);
        }
    }
}