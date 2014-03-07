namespace POSH.Socrata.Entity.Models
{
    public class CityAllData
    {
        public Location location { get; set; }

        public dynamic latitude { get; set; }

        public dynamic longitude { get; set; }

        public dynamic phone { get; set; }

        public dynamic name { get; set; }

        public dynamic Event { get; set; }

        public dynamic architect { get; set; }

        public dynamic address { get; set; }

        public dynamic human_address { get; set; }

        public dynamic street_address { get; set; }

        public dynamic station_name { get; set; }

        public dynamic city { get; set; }

        public dynamic zip { get; set; }

        public dynamic state { get; set; }

        public dynamic commentCount { get; set; }

        public dynamic image { get; set; }

        public dynamic sub_category { get; set; }

        public dynamic building_code { get; set; }

        public dynamic fuel_type_code { get; set; }
    }

    public class Location
    {
        public dynamic needs_recoding { get; set; }

        public dynamic latitude { get; set; }

        public dynamic longitude { get; set; }

        public dynamic human_address { get; set; }
    }
}