using System.ComponentModel;

namespace POSH.Socrata.Entity.Models
{
    public class ErrorMessage : INotifyPropertyChanged
    {
        public string MessageText { get; set; }

        public string MessageTitle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}