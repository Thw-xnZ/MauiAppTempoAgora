using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    // Aqui temos a classe DataService, que é responsável por fazer a requisição à API do OpenWeatherMap e processar os
    // dados recebidos para criar um objeto do tipo Tempo, que contém as informações relevantes sobre a previsão do tempo para
    // a cidade especificada.
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisão(string cidade)
        {
            Tempo? t = null;

            // Aqui temos outro Try-catch, usado para lidar com possiveis erros de conexão que possam ocorrer durante a requisição da API.
            try
            {
                // A chave de acesso à API do OpenWeatherMap, que é necessária para autenticar a requisição e obter os dados de previsão.
                string chave = "6135072afe7f6cec1537d5cb08a5a1a2";

                // A URL da API do OpenWeatherMap, que inclui a {} cidade especificada, a chave de acesso,
                // as unidades de medida (métricas) e a escolha do idioma (pt_br).
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&appid={chave}&units=metric&lang=pt_br";

                // O Using é usado para criar uma instância de HttpClient, que é responsável por fazer a requisição HTTP para a API
                // O bloco Using garante que os recursos serão liberados corretamente após o uso.
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();

                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString(),
                            sunset = sunset.ToString(),
                            speed = (double)rascunho["wind"]["speed"]
                        }; // Fecha o objeto Tempo
                    } // Fecha o if se o status do servidor foi um sucesso
                } // Fecha o using do HttpClient
            }
            catch (HttpRequestException)
            {
                throw new Exception("Sem conexão com a internet.");
            }
                return t;
        }
    }
}
