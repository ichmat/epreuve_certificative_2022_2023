using AppCore.Models;
using AppCore.Property;
using AppCore.Services.GeneralMessage.Args;
using System.Data;
using System.Text.Json.Serialization;

namespace AppCore.Services
{
    public class ActivityEngine
    {
        public const double MULT_KM = 1.15;
        public const double MULT_SEC_ACTIVITY = 1.1;
        public const double MULT_SEC_PAUSE = 1.5;

        public const double DIVISOR_TOTAL_KM = 100;

        private const int INITAL_AWARD_WOOD = 20;
        private const int INITAL_AWARD_SCRAP_METAL = 5;
        private const double INITAL_AWARD_COMMON_OBJECT = 1;
        private const double INITAL_AWARD_RARE_OBJECT = 0.1;
        private const double INITAL_AWARD_EPIC_OBJECT = 0.02;
        private const double INITAL_AWARD_LEGENDARY_OBJECT = 0.005;

        [JsonInclude]
        public Stat CurrentStat { get; private set; }

        [JsonInclude]
        public int AwardWood { get; private set; }
        [JsonInclude]
        public int AwardScapMetal { get; private set; }
        [JsonInclude]
        public double AwardCommonObject { get; private set; }
        [JsonInclude]
        public double AwardRareObject { get; private set; }
        [JsonInclude]
        public double AwardEpicObject { get; private set; }
        [JsonInclude]
        public double AwardLegendaryObject { get; private set; }

        public ActivityEngine(Stat stat) 
        {
            CurrentStat = stat;
            if (stat.ObjectifDistanceKm == null ||
               stat.ObjectifPauseSecMax == null ||
               stat.ObjectifTempsSecMax == null)
                throw new NoNullAllowedException("ObjectifDistanceKm, ObjectifPauseSecMax et ObjectifTempsSecMax doivent posséder des valeurs.");
            RegenerateAward();
        }

        [JsonConstructor]
        public ActivityEngine(Stat currentStat, int awardWood, int awardScapMetal, double awardCommonObject, double awardRareObject, double awardEpicObject, double awardLegendaryObject) : this(currentStat)
        {
            AwardWood = awardWood;
            AwardScapMetal = awardScapMetal;
            AwardCommonObject = awardCommonObject;
            AwardRareObject = awardRareObject;
            AwardEpicObject = awardEpicObject;
            AwardLegendaryObject = awardLegendaryObject;
        }

        private void RegenerateAward()
        {
            Random r = new Random(GetSeedToday());
            AwardWood = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_WOOD));
            AwardScapMetal = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_SCRAP_METAL));
            AwardCommonObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_COMMON_OBJECT));
            AwardRareObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_RARE_OBJECT));
            AwardEpicObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_EPIC_OBJECT));
            AwardLegendaryObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_LEGENDARY_OBJECT));
        }

        private static int AddRand(Random r, int i)
        {
            return i + r.Next(0, i);
        }

        private static double AddRand(Random r, double d)
        {
            return d  + (d * r.NextDouble());
        }

        private static int MultByTotalKm(double? totalKm, int initial)
        {
            if(totalKm == null) return initial;

            return initial + (int)Math.Round(initial * (totalKm.Value / DIVISOR_TOTAL_KM));
        }

        private static double MultByTotalKm(double? totalKm, double initial)
        {
            if (totalKm == null) return initial;

            return initial + initial * (totalKm.Value / DIVISOR_TOTAL_KM);
        }

        private static int GetSeedToday()
        {
            return DateTime.Today.Day + (100 * DateTime.Today.Month) + (10000 * DateTime.Today.Year);
        }

        #region STATIC

        public static Stat GenerateStatWithActivityCalibration(Guid UtilisateurId, double distanceKm, double totalSecActivity, double totalSecPause, int fatigueLevel)
        {
            if (fatigueLevel < 1 || fatigueLevel > 5) throw new ArgumentOutOfRangeException("fatiguelevel doit se trouver entre 1 et 5");
            switch (fatigueLevel)
            {
                case 1:
                    distanceKm = Math.Round(distanceKm * (MULT_KM*2), 2);
                    totalSecActivity = Math.Round(totalSecActivity * (MULT_SEC_ACTIVITY * 2));
                    totalSecPause = Math.Round(totalSecPause / (MULT_SEC_PAUSE * 2));
                    break;
                case 2:
                    distanceKm = Math.Round(distanceKm * MULT_KM, 2);
                    totalSecActivity = Math.Round(totalSecActivity * MULT_SEC_ACTIVITY);
                    totalSecPause = Math.Round(totalSecPause / MULT_SEC_PAUSE);
                    break;
                case 3:
                    distanceKm = Math.Round(distanceKm, 2);
                    totalSecActivity = Math.Round(totalSecActivity);
                    totalSecPause = Math.Round(totalSecPause);
                    break;
                case 4:
                    distanceKm = Math.Round(distanceKm / MULT_KM, 2);
                    totalSecActivity = Math.Round(totalSecActivity / MULT_SEC_ACTIVITY);
                    totalSecPause = Math.Round(totalSecPause * MULT_SEC_PAUSE);
                    break;
                case 5:
                    distanceKm = Math.Round(distanceKm / (MULT_KM * 2), 2);
                    totalSecActivity = Math.Round(totalSecActivity / (MULT_SEC_ACTIVITY * 2));
                    totalSecPause = Math.Round(totalSecPause * MULT_SEC_PAUSE);
                    break;
            }

            Stat stat = new Stat();
            stat.UtilisateurId = UtilisateurId;
            stat.ObjectifDistanceKm = distanceKm;
            stat.ObjectifPauseSecMax = totalSecPause;
            stat.ObjectifTempsSecMax = totalSecActivity;
            stat.ObjectifVitesseMoyenneKmH = CalculateMeanSpeedKmHObjective(in totalSecActivity, in totalSecPause, in distanceKm); ;
            return stat;
        }

        public static double CalculateMeanSpeedKmHObjective(in double secActivity, in double secPause, in double km)
        {
            return km / TimeSpan.FromSeconds(secActivity + secPause * 1.05).TotalHours;
        }

        #endregion

        public double GetDistanceKm(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => CurrentStat.ObjectifDistanceKm.Value / MULT_KM,
                DifficulteCourse.Difficile => CurrentStat.ObjectifDistanceKm.Value * MULT_KM,
                DifficulteCourse.Epuisement => CurrentStat.ObjectifDistanceKm.Value * (MULT_KM * 2),
                _ => CurrentStat.ObjectifDistanceKm.Value
            };

        public double GetTotalSecActivity(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => CurrentStat.ObjectifTempsSecMax.Value / MULT_SEC_ACTIVITY,
                DifficulteCourse.Difficile => CurrentStat.ObjectifTempsSecMax.Value * MULT_SEC_ACTIVITY,
                DifficulteCourse.Epuisement => CurrentStat.ObjectifTempsSecMax.Value * (MULT_SEC_ACTIVITY * 2),
                _ => CurrentStat.ObjectifTempsSecMax.Value
            };

        public double GetTotalSecPauseActivity(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => CurrentStat.ObjectifPauseSecMax.Value * MULT_SEC_PAUSE,
                DifficulteCourse.Difficile => CurrentStat.ObjectifPauseSecMax.Value / MULT_SEC_PAUSE,
                DifficulteCourse.Epuisement => CurrentStat.ObjectifPauseSecMax.Value / (MULT_SEC_PAUSE * 2),
                _ => CurrentStat.ObjectifPauseSecMax.Value
            };

        public double GetMeanSpeedKmH(DifficulteCourse level = DifficulteCourse.Normal)
        {
            if (level == DifficulteCourse.Normal && CurrentStat.ObjectifVitesseMoyenneKmH != null)
                return CurrentStat.ObjectifVitesseMoyenneKmH.Value;

            return CalculateMeanSpeedKmHObjective(
                GetTotalSecActivity(level),
                GetTotalSecPauseActivity(level),
                GetDistanceKm(level)
                );
        }
    }
}
