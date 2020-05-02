using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using server.Model;

namespace server.EF {
    public static class Service {
        private static readonly NoteDbContext Context = new NoteDbContext();

        public static Note CreateNote(Note newNote) {
            Context.Entry(newNote).State = EntityState.Added;
            Context.SaveChanges();
            return newNote;
        }

        public static bool DeleteNote(int id) {
            try {
                var deletedNote = Context.Notes.FirstOrDefault(n => n.Id == id);
                Context.Entry(deletedNote).State = EntityState.Deleted;
                Context.SaveChanges();
                return true;
            } catch {
                // ignored
            }

            return false;
        }

        public static IEnumerable<Note> GetNotes() {
            var notes = Context.Notes.Include(n => n.Tags);
            return notes;
        }
    }
}