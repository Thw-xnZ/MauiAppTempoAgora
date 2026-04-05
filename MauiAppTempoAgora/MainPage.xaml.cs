using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

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
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // O try-catch aqui é usando para lidar com o problema de conexão com a internet, caso  o usuário esteja sem acesso
            // ou se a conexão estiver lenta, dentre outros problemas de conectividade que possam ocorrer, mostrando uma mensagem de alerta.
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisão(txt_cidade.Text);

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
    }
}
