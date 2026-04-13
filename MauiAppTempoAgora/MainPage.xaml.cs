using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Globalization;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        // Botão para buscar a previsão do tempo com tratamento de erros
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            // O try-catch aqui é usando para lidar com o problema de conexão com a internet, caso  o usuário esteja sem acesso
            // ou se a conexão estiver lenta, dentre outros problemas de conectividade que possam ocorrer, mostrando uma mensagem de alerta.
            try
            {
                if (!string.IsNullOrEmpty(txt_city.Text))
                {
                    Tempo? t = await DataService.GetPrevisão(txt_city.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Tempo Máx: {t.temp_max} \n" +
                                         $"Tempo Min: {t.temp_min} \n" +
                                         $"Clima atual:  {t.description} \n" +
                                         $"Velocidade: {t.speed} \n" +
                                         $"Visibilidade: {t.visibility} \n";

                        lbl_resultado.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",",".")}";

                        wv_mapa.Source = mapa;
                    }
                    else
                    {
                        lbl_resultado.Text = "Cidade não encontrada, certeza que está certo?";
                    }
                }
                else
                {
                    lbl_resultado.Text = "Digite o nome de uma cidade.";
                }
            }
            // Os catchs abaixo são para lidar com os erros de conexão, mostrando mensagens específicas para cada tipo de erro,
            // como falta de internet ou conexão lenta, além de um catch genérico para outros tipos de erros que possam ocorrer.
            catch (HttpRequestException)
            {
                await DisplayAlert("Sem internet", "Verifique sua conexão e tente novamente.", "OK");
            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Conexão lenta", "A requisição demorou muito.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)
                );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude} \n";
                    lbl_cood.Text = local_disp;

                    // O método GetCidade pega o nome da cidade que está nas coodenadas adquiridas
                    GetCidade(local.Latitude, local.Longitude);
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível obter a localização: {ex.Message}", "OK");
            }
        }

        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                Placemark? place = places.FirstOrDefault();
                if (place != null)
                {
                   txt_city.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }
        }
    }
}
