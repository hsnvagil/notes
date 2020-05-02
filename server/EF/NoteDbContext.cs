using System.Data.Entity;
using server.Model;

namespace server.EF {
    internal class NoteDbContext : DbContext {
        public NoteDbContext() : base("NoteDb") { }

        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            var note = modelBuilder.Entity<Note>();
            note.HasKey(n => n.Id);
            note.Property(n => n.Text).HasMaxLength(250).IsRequired();
            note.Property(n => n.CreatedTime).HasColumnType("datetime").IsRequired();
            note.HasMany(n => n.Tags).WithOptional(t => t.Note).HasForeignKey(t => t.NoteId);

            var tag = modelBuilder.Entity<Tag>();
            tag.HasKey(t => t.Id);
            tag.Property(t => t.TagText).HasMaxLength(250).IsRequired();
        }
    }
}