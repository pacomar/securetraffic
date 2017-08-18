using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using SecureTraffic.Messages;
using System;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;

namespace SecureTraffic
{
    public class TaskCounter
    {
        public async Task RunCounter(CancellationToken token)
        {
            await Task.Run(async () => {

                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(5000);
                    var message = new TickedMessage
                    {
                        Message = i.ToString()
                    };

                    AvisarSoyLento(Vehicle.Otro);

                    Device.BeginInvokeOnMainThread(() => {
                        MessagingCenter.Send<TickedMessage>(message, "TickedMessage");
                    });
                }
            }, token);
        }

        public async void AvisarSoyLento(Vehicle vehiculo)
        {
            VehiclesService _vehServ = new VehiclesService();
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

            //TODO map e to My position
            MyPosition aux = new MyPosition()
            {
                Coordinate = new Coordinate(position.Latitude, position.Longitude),
                Speed = position.Speed,
                Vehicle = vehiculo,
                Time = Helper.ConvertToTimestamp(DateTime.Now).ToString()
            };
            
            await _vehServ.SetPositionVehicle(aux);
        }
    }
}
