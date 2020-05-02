using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using cache;
using Newtonsoft.Json;
using server.EF;
using Note = server.Model.Note;

namespace server {
    public static class HttpListenerContextExtensions {
        public static async Task<T> ReadAsync<T>(this HttpListenerRequest request) {
            using var reader = new StreamReader(request.InputStream);
            var str = await reader.ReadToEndAsync();
            var obj = JsonConvert.DeserializeObject<T>(str);
            return obj;
        }

        public static int? GetIntParam(this HttpListenerRequest request, string paramName) {
            var param = request.QueryString[paramName];
            if (param == null) return null;
            return int.Parse(param);
        }

        public static string GetStringParam(this HttpListenerRequest request, string paramName) {
            return request.QueryString[paramName];
        }

        public static DateTime? GetDateTimeParam(this HttpListenerRequest request, string paramName) {
            var param = request.QueryString[paramName];
            if (param == null) return null;
            return DateTime.Parse(param);
        }

        public static async Task WriteAsync<T>(this HttpListenerResponse response, T data, int statusCode) {
            using var sw = new StreamWriter(response.OutputStream);
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            var json = JsonConvert.SerializeObject(data);
            await sw.WriteLineAsync(json);
        }
    }

    internal class Program {
        private static readonly Controller.Controller Controller = new Controller.Controller();
        private static readonly CacheController CacheController = new CacheController();
        private static HttpListener _listener;

        private static void Main(string[] args) {
            using var db = new NoteDbContext();
            db.Database.CreateIfNotExists();
            StartServer().GetAwaiter().GetResult();
        }

        private static async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            switch (request.HttpMethod) {
                case "GET":
                    IEnumerable<Note> notes = null;
                    var createdFrom = request.GetDateTimeParam("createdFrom");
                    var createdTo = request.GetDateTimeParam("createdTo");
                    var tag = request.GetStringParam("tag") ?? null;

                    bool CachePredicate(cache.Note n) {
                        return (tag == null || n.Tags != null && n.Tags.Exists(t => t.TagText == tag)) &&
                               (createdFrom == null || n.CreatedTime >= createdFrom) &&
                               (createdTo == null || n.CreatedTime <= createdTo);
                    }

                    var json = await CacheController.Get(CachePredicate);
                    if (json == null) {
                        bool ServerPredicate(Note n) {
                            return (tag == null || n.Tags != null && n.Tags.Exists(t => t.TagText == tag)) &&
                                   (createdFrom == null || n.CreatedTime >= createdFrom) &&
                                   (createdTo == null || n.CreatedTime <= createdTo);
                        }

                        notes = Controller.GetNotes(ServerPredicate);

                        json = JsonConvert.SerializeObject(notes);
                        await CacheController.SetItems(json);
                    } else {
                        notes = JsonConvert.DeserializeObject<IEnumerable<Note>>(json);
                    }

                    await response.WriteAsync(notes, 200);
                    break;
                case "POST":
                    var note = await request.ReadAsync<Note>();
                    try {
                        var result = Controller.CreateNote(note);
                        var key = note.Id.ToString();
                        json = JsonConvert.SerializeObject(result);
                        await CacheController.Add(key, json);
                        await response.WriteAsync(result.Id, 201);
                    } catch (Exception e) {
                        await response.WriteAsync(e.Message, 400);
                    }

                    break;
                case "DELETE":
                    var id = request.GetIntParam("id");
                    if (id != null) {
                        response.StatusCode = Controller.DeleteNote(id.Value) ? 204 : 404;
                        if (response.StatusCode == 204) await CacheController.Remove(id.ToString());
                    }

                    break;
            }
        }

        private static async Task StartServer() {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:45678/");
            _listener.Prefixes.Add("http://127.0.0.1:45678/");
            _listener.Start();
            while (true) {
                var context = await _listener.GetContextAsync();

                await HandleRequest(context.Request, context.Response);

                context.Response.Close();
            }
        }
    }
}