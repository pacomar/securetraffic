using SecureTraffic.Messages;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SecureTraffic
{
	public partial class SlowVehicleView : ContentPage
	{
		public SlowVehicleView(Vehicle veh)
		{
            //var attributesWindow = new WindowManagerLayoutParams();

			InitializeComponent();

			this.Title = "Emitiendo posición: " + veh;

            var message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(message, "StartLongRunningTaskMessage");

            BindingContext = new SlowVehicleViewModel(veh, MyMap);
		}
        protected override bool OnBackButtonPressed()
        {
            var message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(message, "StopLongRunningTaskMessage");
            return base.OnBackButtonPressed();
        }

        void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<TickedMessage>(this, "TickedMessage", message => {
                Device.BeginInvokeOnMainThread(() => {
                    //ticker.Text = message.Message;
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, "CancelledMessage", message => {
                Device.BeginInvokeOnMainThread(() => {
                    //ticker.Text = "Cancelado";
                });
            });
        }
    }
}
