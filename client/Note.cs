using System;
using System.Collections.Generic;

namespace client {
    public class Note {
        public Note(string text, DateTime createdTime, ICollection<Tag> tags) {
            Text = text;
            CreatedTime = createdTime;
            Tags = tags;
        }

        public string Text { get; private set; }
        public DateTime CreatedTime { get; private set; }

        public virtual ICollection<Tag> Tags { get; protected set; }
    }
}