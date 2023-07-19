using AppCore.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPPublishCourse : EndPointArgs
    {
        public override string Route() => APIRoute.PUBLISH_COURSE;

        [JsonInclude]
        public DifficulteCourse Difficulty;

        [JsonInclude]
        public double DistanceKm;

        [JsonInclude]
        public double TotalActivitySec;

        [JsonInclude]
        public double TotalPauseSec;

        [JsonInclude]
        public double MeanSpeedKmH;

        [JsonInclude]
        public DateTime DateStartActivity;

        public EPPublishCourse(DifficulteCourse difficulty, double distanceKm, double totalActivitySec, double totalPauseSec, double meanSpeedKmH, DateTime dateStartActivity)
        {
            Difficulty = difficulty;
            DistanceKm = distanceKm;
            TotalActivitySec = totalActivitySec;
            TotalPauseSec = totalPauseSec;
            MeanSpeedKmH = meanSpeedKmH;
            DateStartActivity = dateStartActivity;
        }
    }
}
