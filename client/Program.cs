using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace client {
    internal class Program {
        private static void Main(string[] args) {
            var client = new HttpClient();
            while (true) {
                Console.Clear();

                Console.WriteLine("1.Get notes\n2.Add new note\n3.Delete note");
                var input = Console.ReadLine();
                string json = null;
                HttpResponseMessage response = null;
                var url = "http://localhost:45678/";
                switch (input) {
                    case "1":
                        var temp = url;
                        Console.Write("\nenter created from (dd MM yyyy):: ");
                        var createdFrom = Console.ReadLine();

                        Console.Write("\nenter created to(dd MM yyyy):: ");
                        var createdTo = Console.ReadLine();

                        Console.Write("\nenter tag:: ");
                        var tag = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(createdFrom)) {
                            var d = createdFrom.Split(' ').Select(int.Parse).ToArray();
                            var dateTime = new DateTime(d[2], d[1], d[0]);
                            url += $"?createdFrom={dateTime.ToShortDateString()}";
                        }

                        if (!string.IsNullOrWhiteSpace(createdTo)) {
                            var d = createdTo.Split(' ').Select(int.Parse).ToArray();
                            var dateTime = new DateTime(d[2], d[1], d[0]);
                            var ch = url != temp ? '&' : '?';
                            url += $"{ch}createdTo={dateTime.ToShortDateString()}";
                        }

                        if (!string.IsNullOrWhiteSpace(tag)) {
                            var ch = url != temp ? '&' : '?';
                            url += $"{ch}tag={tag}";
                        }

                        response = client.GetAsync(url).Result;
                        json = response.Content.ReadAsStringAsync().Result;
                        var notes = JsonConvert.DeserializeObject<IEnumerable<Note>>(json);
                        if (notes != null && notes.Any()) {
                            Console.WriteLine();
                            foreach (var note in notes) {
                                Console.WriteLine($"text:{note.Text}\ncreated time:{note.CreatedTime}");
                                if (note.Tags.Count > 0)
                                    foreach (var noteTag in note.Tags)
                                        Console.WriteLine($"tags:{noteTag.TagText}");
                                else
                                    Console.WriteLine("tags: ");

                                Console.WriteLine();
                            }
                        } else {
                            Console.WriteLine("\nNotes not found");
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadLine();
                        break;
                    case "2":
                        Console.Write("Enter notes::");
                        var noteText = Console.ReadLine();
                        Console.WriteLine("Want to add tags to note? y/n");
                        var c = Console.ReadKey();
                        var tags = new List<Tag>();
                        if (c.Key == ConsoleKey.Y) {
                            Console.WriteLine("\nEnter tags (write '/n' at the end to finish)");
                            do {
                                var tagText = Console.ReadLine();
                                var length = tagText.Length;
                                if (length >= 3 && tagText[length - 1] == 'n' && tagText[length - 2] == '/') {
                                    tagText = tagText.Substring(0, length - 3);
                                    tags.Add(new Tag(tagText));
                                    break;
                                }

                                tags.Add(new Tag(tagText));
                            } while (true);
                        }

                        var newNote = new Note(noteText, DateTime.Now, tags);
                        json = JsonConvert.SerializeObject(newNote);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = client.PostAsync("http://localhost:45678", content).Result;

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadLine();
                        break;

                    case "3":
                        Console.Write("Enter id::");
                        var id = int.Parse(Console.ReadLine());
                        if (!string.IsNullOrWhiteSpace(id.ToString())) url += $"deletedNote?id={id}";

                        response = client.DeleteAsync(new Uri(url)).Result;
                        Console.WriteLine(response.StatusCode);
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}