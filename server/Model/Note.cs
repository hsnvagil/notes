using System;
using System.Collections.Generic;

namespace server.Model {
    public class Note : BaseEntity {
        public Note() { }

        public Note(string text, DateTime createdTime, List<Tag> tags) {
            Text = text;
            CreatedTime = createdTime;
            Tags = tags;
        }

        public string Text { get; set; }
        public DateTime CreatedTime { get; set; }
        public virtual List<Tag>? Tags { get; set; }
    }
}