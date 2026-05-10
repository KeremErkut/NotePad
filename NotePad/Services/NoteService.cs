using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NotePad.Models;

namespace NotePad.Services
{
    public class NoteService
    {
        // Path to the storage file in the application directory
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notes.json");

        public void SaveNotes(List<Note> notes)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(notes, options);
            File.WriteAllText(_filePath, jsonString);
        }

        public List<Note> LoadNotes()
        {
            if (!File.Exists(_filePath)) return new List<Note>();

            try
            {
                string jsonString = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Note>>(jsonString) ?? new List<Note>();
            }
            catch
            {
                return new List<Note>(); // Return empty list if reading fails
            }
        }
    }
}