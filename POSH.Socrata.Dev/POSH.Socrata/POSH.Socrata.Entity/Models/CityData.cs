using System.ComponentModel;

namespace POSH.Socrata.Entity.Models
{
    public class CityData : INotifyPropertyChanged
    {
        private Altitude _coordinate;

        public Altitude Coordinate
        {
            get
            {
                return _coordinate;
            }
            set
            {
                _coordinate = value;
                //NotifyPropertyChanged("Coordinate");
            }
        }

        private string _phone;
                    
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                //NotifyPropertyChanged("Phone");
            }
        }

        private string _address;

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                //NotifyPropertyChanged("Address");
            }
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                //NotifyPropertyChanged("Name");
            }
        }

        private string _subTitle;

        public string SubTitle
        {
            get
            {
                return _subTitle;
            }
            set
            {
                _subTitle = value;
                //NotifyPropertyChanged("SubTitle");
            }
        }

        private string _category;

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                //NotifyPropertyChanged("Category");
            }
        }

        private string _comments;

        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
                //NotifyPropertyChanged("Comments");
            }
        }

        public int _indexNo;

        public int IndexNo
        {
            get
            {
                return _indexNo;
            }
            set
            {
                _indexNo = value;
                //NotifyPropertyChanged("IndexNo");
            }
        }

        private string _distance;

        public string Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                //NotifyPropertyChanged("Distance");
            }
        }

        private string _backgroundImage;

        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                //NotifyPropertyChanged("BackgroundImage");
            }
        }

        private string _compositeKey;

        public string CompositeKey
        {
            get { return _compositeKey; }
            set
            {
                _compositeKey = value;
                //NotifyPropertyChanged("CompositeKey");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class Altitude
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}