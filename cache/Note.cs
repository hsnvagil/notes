using System;
using System.Collections.Generic;

namespace cache {
    public class Note {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<Tag> Tags { get; set; }
    }
}