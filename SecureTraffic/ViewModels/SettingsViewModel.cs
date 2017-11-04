using SecureTraffic.Models;
using Xamarin.Forms;

namespace SecureTraffic
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
        }

        /// <summary>
        /// Funcion que devuelve la configuracion de alertas si no hay devuelve todas a true
        /// </summary>
        /// <returns>Configuracion alertas</returns>
        protected SettingsModel retrieveSettings()
        {
            if (Application.Current.Properties.ContainsKey("sonido") && Application.Current.Properties.ContainsKey("imagen") && Application.Current.Properties.ContainsKey("color"))
            {
                bool sonido = (bool)Application.Current.Properties["sonido"];
                bool imagen = (bool)Application.Current.Properties["imagen"];
                bool color = (bool)Application.Current.Properties["color"];

                return new SettingsModel(sonido, imagen, color);
            }
            return new SettingsModel();
        }

        protected void SaveSettings(SettingsModel settings)
        {
            Application.Current.Properties.Add("sonido", settings.sonido);
            Application.Current.Properties.Add("imagen", settings.imagen);
            Application.Current.Properties.Add("color", settings.color);

            Application.Current.SavePropertiesAsync();
        }
    }
}
