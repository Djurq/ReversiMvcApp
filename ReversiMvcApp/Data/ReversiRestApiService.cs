using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Data
{
    public class ReversiRestApiService
    {
        private readonly HttpClient _httpClient;

        public ReversiRestApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001/");
        }

        public List<Spel> GetAllOpen()
        {
            List<Spel> openSpellen = new List<Spel>();

            var resultaat = _httpClient.GetAsync("api/spel/").Result;
            if (resultaat.IsSuccessStatusCode)
            {
                var content = resultaat.Content.ReadAsStringAsync().Result;
                openSpellen = JsonConvert.DeserializeObject<List<Spel>>(content);
            }

            return openSpellen;
        }

        public List<Spel> GetAll(string spelerToken)
        {
            List<Spel> openSpellen = new List<Spel>();

            var resultaat = _httpClient.GetAsync($"api/spel/{spelerToken}").Result;
            if (resultaat.IsSuccessStatusCode)
            {
                var content = resultaat.Content.ReadAsStringAsync().Result;
                openSpellen = JsonConvert.DeserializeObject<List<Spel>>(content);
            }

            return openSpellen;
        }

        public Spel Get(string id)
        {
            Spel spel = null;

            var resultaat = _httpClient.GetAsync($"api/spel/{id}").Result;
            if (resultaat.IsSuccessStatusCode)
            {
                var content = resultaat.Content.ReadAsStringAsync().Result;
                spel = JsonConvert.DeserializeObject<Spel>(content);
            }

            return spel;
        }

        public bool Delete(string id, string spelerToken)
        {
            var resultaat = _httpClient.DeleteAsync($"/api/spel/{id}/?token={spelerToken}").Result;

            return resultaat.IsSuccessStatusCode;
        }

        public Spel NewSpel(string spelerToken, string omschrijving)
        {
            Spel nieuwSpel = null;

            var spelercontent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("spelerToken", spelerToken),
                new KeyValuePair<string, string>("omschrijving", omschrijving)
            });
            
            HttpResponseMessage resultaat = _httpClient.PostAsync("/api/spel/", spelercontent).Result;
            
            if (resultaat.IsSuccessStatusCode) {
                var content = resultaat.Content.ReadAsStringAsync().Result;
                nieuwSpel = JsonConvert.DeserializeObject<Spel>(content);
            }
            
            return nieuwSpel;
        }

        public Spel JoinSpel(string id, string spelerToken)
        {
            Spel joinedSpel = null;
            
            HttpResponseMessage resultaat = _httpClient.PutAsync($"/api/spel/{id}/join/?token={spelerToken}", new StringContent("")
            ).Result;

            if (resultaat.IsSuccessStatusCode)
            {
                var content = resultaat.Content.ReadAsStringAsync().Result;
                joinedSpel = JsonConvert.DeserializeObject<Spel>(content);
            }

            return joinedSpel;
        }

        public Spel DoeZet(string id, string spelerToken, int rij, int kolom)
        {
            Spel gezetSpel = null;
                HttpResponseMessage resultaat = _httpClient.PutAsync($"/api/spel/{id}/zet?token={spelerToken}&rij={rij}&kolom={kolom}",
                new StringContent("")).Result;

                if (resultaat.IsSuccessStatusCode)
                {
                    var content = resultaat.Content.ReadAsStringAsync().Result;
                    gezetSpel = JsonConvert.DeserializeObject<Spel>(content);
                }

                return gezetSpel;
        }
    }
}