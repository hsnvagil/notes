using Newtonsoft.Json;

namespace server.Model {
    public class Tag : BaseEntity {
        public Tag() { }

        public Tag(string tagText) {
            TagText = tagText;
        }

        public string TagText { get; set; }
        public int? NoteId { get; set; }

        [JsonIgnore] public virtual Note Note { get; set; }
    }
}