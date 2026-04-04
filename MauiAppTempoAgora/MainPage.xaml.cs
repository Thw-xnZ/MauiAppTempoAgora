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

        private async void Button_Clicked(object sender, EventArgs e)
        {
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
                                         $"Visibilidade: {t.visibility} \n";

                        lbl_resultado.Text = dados_previsao;
                    }
                    else
                    {
                        lbl_resultado.Text = "Cidade não encontrada.";
                    }
                }
                else
                {
                    lbl_resultado.Text = "Digite o nome de uma cidade.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Putzz..", ex.Message, "OK");
            }
        }
    }
}
