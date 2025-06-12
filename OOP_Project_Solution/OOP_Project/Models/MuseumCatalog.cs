using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Linq;

namespace OOP_Project.Models {
    public class MuseumCatalog {
        private string name = "main";
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();

        public MuseumCatalog() { }

        public MuseumCatalog(string name) {
            this.name = name;
        }

        public string GetDataFilePath() {
            return $"data_{name}.json";
        }

        public void LoadData() {
            string path = GetDataFilePath();
            Debug.WriteLine($"Attempting to load data from: {path}");
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    Debug.WriteLine($"Read JSON: {json.Substring(0, Math.Min(json.Length, 100))}...");
                    var settings = new JsonSerializerSettings {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Auto
                    };
                    var catalog = JsonConvert.DeserializeObject<MuseumCatalog>(json, settings);

                    if (catalog != null) {
                        Artworks = catalog.Artworks ?? new List<Artwork>();
                        Artists = catalog.Artists ?? new List<Artist>();
                        Exhibitions = catalog.Exhibitions ?? new List<Exhibition>();

                        // Rebuild Artist references
                        foreach (var artwork in Artworks) {
                            artwork.Artist = Artists.FirstOrDefault(a => a.Name == artwork.ArtistName);
                            if (artwork.Artist != null && !artwork.Artist.Artworks.Contains(artwork))
                                artwork.Artist.Artworks.Add(artwork);
                            Debug.WriteLine($"Rebuilt reference for artwork: {artwork?.Title}, Artist: {artwork?.Artist?.Name}");
                        }

                        // Ensure Exhibitions have valid Artwork references
                        foreach (var exhibition in Exhibitions) {
                            var validArtworks = exhibition.Artworks.Where(a => Artworks.Contains(a)).ToList();
                            exhibition.Artworks = validArtworks;
                            Debug.WriteLine($"Validated exhibition: {exhibition?.Title}, Artworks: {exhibition.Artworks.Count}");
                        }
                        Debug.WriteLine($"Data loaded: {Artists.Count} artists, {Artworks.Count} artworks, {Exhibitions.Count} exhibitions");
                    } else {
                        Debug.WriteLine("Deserialized catalog is null");
                        MessageBox.Show("Failed to load data: Invalid JSON format", "Error");
                    }
                } else {
                    Debug.WriteLine($"Data file not found: {path}");
                }
            } catch (Exception ex) {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"Error loading data: {ex.Message}", "Error");
                Artworks = new List<Artwork>();
                Artists = new List<Artist>();
                Exhibitions = new List<Exhibition>();
            }
        }

        public void SaveData() {
            Debug.WriteLine("Attempt to save data");
            string path = GetDataFilePath();
            try {
                var settings = new JsonSerializerSettings {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                string json = JsonConvert.SerializeObject(this, settings);
                File.WriteAllText(path, json);
                Debug.WriteLine($"Data saved to: {path}");
            } catch (Exception ex) {
                Debug.WriteLine($"Error saving data: {ex.Message}");
                MessageBox.Show($"Error saving data: {ex.Message}", "Error");
            }
        }
    }

}