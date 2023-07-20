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
        public const double MULT_AWARD = 1.2;

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
        public int AwardScrapMetal { get; private set; }
        [JsonInclude]
        public double AwardCommonObject { get; private set; }
        [JsonInclude]
        public double AwardRareObject { get; private set; }
        [JsonInclude]
        public double AwardEpicObject { get; private set; }
        [JsonInclude]
        public double AwardLegendaryObject { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusWood { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusScrapMetal { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusCommonObject { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusRareObject { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusEpicObject { get; private set; }
        [JsonInclude]
        public SuccessBonus BonusLegendaryObject { get; private set; }

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
        public ActivityEngine(Stat currentStat, int awardWood, int awardScrapMetal, double awardCommonObject, double awardRareObject, double awardEpicObject, double awardLegendaryObject, SuccessBonus bonusWood, SuccessBonus bonusScrapMetal, SuccessBonus bonusCommonObject, SuccessBonus bonusRareObject, SuccessBonus bonusEpicObject, SuccessBonus bonusLegendaryObject)
        {
            CurrentStat = currentStat;
            AwardWood = awardWood;
            AwardScrapMetal = awardScrapMetal;
            AwardCommonObject = awardCommonObject;
            AwardRareObject = awardRareObject;
            AwardEpicObject = awardEpicObject;
            AwardLegendaryObject = awardLegendaryObject;
            BonusWood = bonusWood;
            BonusScrapMetal = bonusScrapMetal;
            BonusCommonObject = bonusCommonObject;
            BonusRareObject = bonusRareObject;
            BonusEpicObject = bonusEpicObject;
            BonusLegendaryObject = bonusLegendaryObject;
        }

        private void RegenerateAward()
        {
            Random r = new Random(GetRandSeedToday());
            AwardWood = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_WOOD));
            AwardScrapMetal = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_SCRAP_METAL));
            AwardCommonObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_COMMON_OBJECT));
            AwardRareObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_RARE_OBJECT));
            AwardEpicObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_EPIC_OBJECT));
            AwardLegendaryObject = AddRand(r, MultByTotalKm(CurrentStat.TotalDistanceKm, INITAL_AWARD_LEGENDARY_OBJECT));

            BonusWood = SuccessBonus.GenerateSBWood();
            BonusScrapMetal = SuccessBonus.GenerateSBScrapMetal();
            BonusCommonObject = SuccessBonus.GenerateSBCommonObj(r);
            BonusRareObject = SuccessBonus.GenerateSBRareObj(r);
            BonusEpicObject = SuccessBonus.GenerateSBEpicObj(r);
            BonusLegendaryObject = SuccessBonus.GenerateSBLegendaryObj(r);
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

        /// <summary>
        /// Génère la graine d'aléatoire par rapport à la date d'aujourd'hui
        /// </summary>
        /// <returns>Graine d'aléatoire</returns>
        private static int GetRandSeedToday()
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

        #region GET_DEPENDING_ON_THE_DIFFICULTY

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
    
        public int GetAwardWood(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => (int)Math.Round(AwardWood / MULT_AWARD),
                DifficulteCourse.Difficile => (int)Math.Round(AwardWood * MULT_AWARD),
                DifficulteCourse.Epuisement => (int)Math.Round(AwardWood * (MULT_AWARD * 2)),
                _ => AwardWood
            };

        public int GetAwardScrapMetal(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => (int)Math.Round(AwardScrapMetal / MULT_AWARD),
                DifficulteCourse.Difficile => (int)Math.Round(AwardScrapMetal * MULT_AWARD),
                DifficulteCourse.Epuisement => (int)Math.Round(AwardScrapMetal * (MULT_AWARD * 2)),
                _ => AwardScrapMetal
            };

        public double GetAwardCommonObj(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => AwardCommonObject / MULT_AWARD,
                DifficulteCourse.Difficile => AwardCommonObject * MULT_AWARD,
                DifficulteCourse.Epuisement => AwardCommonObject * (MULT_AWARD * 2),
                _ => AwardCommonObject
            };

        public double GetAwardRareObj(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => AwardRareObject / MULT_AWARD,
                DifficulteCourse.Difficile => AwardRareObject * MULT_AWARD,
                DifficulteCourse.Epuisement => AwardRareObject * (MULT_AWARD * 2),
                _ => AwardRareObject
            };

        public double GetAwardEpicObj(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => AwardEpicObject / MULT_AWARD,
                DifficulteCourse.Difficile => AwardEpicObject * MULT_AWARD,
                DifficulteCourse.Epuisement => AwardEpicObject * (MULT_AWARD * 2),
                _ => AwardEpicObject
            };

        public double GetAwardLegendaryObj(DifficulteCourse level = DifficulteCourse.Normal) =>
            level switch
            {
                DifficulteCourse.Facile => AwardLegendaryObject / MULT_AWARD,
                DifficulteCourse.Difficile => AwardLegendaryObject * MULT_AWARD,
                DifficulteCourse.Epuisement => AwardLegendaryObject * (MULT_AWARD * 2),
                _ => AwardLegendaryObject
            };

        #endregion

        #region GENERATE_AWARD

        public ResultAward GenerateAward(Courses courses)
        {
            return GenerateAward(courses.NiveauDifficulte, courses.DistanceKm, courses.TempsSec, courses.TempsSecPause, courses.VitesseMoyenKmH);
        }

        public ResultAward GenerateAward(DifficulteCourse difficulty, double distanceKm, double totalSec, double totalSecPause, double meanSpeedKmH)
        {
            int awardWood, awardScrapMetal;
            double awardCommonObject, awardRareObject, awardEpicObject, awardLegendaryObject;

            int numberOfObjectivesReached = ObtainTheNumberOfObjectivesReached(difficulty, distanceKm, totalSec, totalSecPause, meanSpeedKmH);
            
            if (numberOfObjectivesReached > 0)
            {
                // un ou plusieurs objectifs rempli(s), application des bonus
                GetAwardsWithBonus(difficulty, numberOfObjectivesReached,
                    out awardWood, out awardScrapMetal, out awardCommonObject, out awardRareObject, out awardEpicObject, out awardLegendaryObject);
            }
            else
            {
                // aucun objectif rempli, calcule du pourcentage de complétion
                GetAwardsWithoutBonus(difficulty, distanceKm / GetDistanceKm(difficulty),
                    out awardWood, out awardScrapMetal, out awardCommonObject, out awardRareObject, out awardEpicObject, out awardLegendaryObject);
            }

            // génération des gains d'objets
            Dictionary<Objet, int> awardObj = new Dictionary<Objet, int>();
            RandomObjAward(ref awardObj, awardCommonObject, TypeRarete.COMMUN);
            RandomObjAward(ref awardObj, awardRareObject, TypeRarete.RARE);
            RandomObjAward(ref awardObj, awardEpicObject, TypeRarete.EPIC);
            RandomObjAward(ref awardObj, awardLegendaryObject, TypeRarete.LEGENDAIRE);

            return new ResultAward(
                awardWood, 
                awardScrapMetal,
                awardObj
                );
        }

        private int ObtainTheNumberOfObjectivesReached(DifficulteCourse difficulty, double distanceKm, double totalSec, double totalSecPause, double meanSpeedKmH)
        {
            int numberOfObjectivesReached = 0;
            // il est obligatoire d'effectuer l'objectif de distance
            if (GetDistanceKm(difficulty) <= distanceKm)
            {
                numberOfObjectivesReached++;

                if(GetTotalSecActivity(difficulty) >= totalSec)
                {
                    numberOfObjectivesReached++;
                }
                if (GetTotalSecPauseActivity(difficulty) >= totalSecPause)
                {
                    numberOfObjectivesReached++;
                }
                if (GetMeanSpeedKmH(difficulty) <= meanSpeedKmH)
                {
                    numberOfObjectivesReached++;
                }
            }

            return numberOfObjectivesReached;
        }

        private void GetAwardsWithoutBonus(DifficulteCourse difficulty, double percentAward, out int awardWood, out int awardScrapMetal, 
            out double awardCommonObject, out double awardRareObject, out double awardEpicObject, out double awardLegendaryObject)
        {
            awardWood = (int)Math.Round(GetAwardWood(difficulty) * percentAward);
            awardScrapMetal = (int)Math.Round(GetAwardScrapMetal(difficulty) * percentAward);
            awardCommonObject = GetAwardCommonObj(difficulty) * percentAward;
            awardRareObject = GetAwardRareObj(difficulty) * percentAward;
            awardEpicObject = GetAwardEpicObj(difficulty) * percentAward;
            awardLegendaryObject = GetAwardLegendaryObj(difficulty) * percentAward;
        }

        private void GetAwardsWithBonus(DifficulteCourse difficulty, int numberOfObjectivesReached, out int awardWood, out int awardScrapMetal,
            out double awardCommonObject, out double awardRareObject, out double awardEpicObject, out double awardLegendaryObject)
        {
            awardWood = ApplyBonus(GetAwardWood(difficulty), BonusWood, numberOfObjectivesReached);
            awardScrapMetal = ApplyBonus(GetAwardScrapMetal(difficulty), BonusScrapMetal, numberOfObjectivesReached);
            awardCommonObject = ApplyBonus(GetAwardCommonObj(difficulty), BonusCommonObject, numberOfObjectivesReached);
            awardRareObject = ApplyBonus(GetAwardRareObj(difficulty), BonusRareObject, numberOfObjectivesReached);
            awardEpicObject = ApplyBonus(GetAwardEpicObj(difficulty), BonusEpicObject, numberOfObjectivesReached);
            awardLegendaryObject = ApplyBonus(GetAwardLegendaryObj(difficulty), BonusLegendaryObject, numberOfObjectivesReached);
        }

        private int ApplyBonus(int initial, SuccessBonus bonus, int numberObjectivesReached)
        {
            if(bonus.Type == TypeSuccessBonus.AllObjective && numberObjectivesReached == 4)
            {
                return (int)Math.Round(initial * bonus.Multiply);
            }
            else if(bonus.Type == TypeSuccessBonus.PerObjective && numberObjectivesReached > 0)
            {
                return (int)Math.Round(initial * (bonus.Multiply * numberObjectivesReached));
            }
            else
            {
                return initial;
            }
        }

        private double ApplyBonus(double initial, SuccessBonus bonus, int numberObjectivesReached)
        {
            if (bonus.Type == TypeSuccessBonus.AllObjective && numberObjectivesReached == 4)
            {
                return initial * bonus.Multiply;
            }
            else if (bonus.Type == TypeSuccessBonus.PerObjective && numberObjectivesReached > 0)
            {
                return initial * (bonus.Multiply * numberObjectivesReached);
            }
            else
            {
                return initial;
            }
        }

        private void RandomObjAward(ref Dictionary<Objet, int> awardObj, double percentChance, TypeRarete typeRarete)
        {
            // récupération des objets de cette rareté
            Objet[] arr_objs = NecessaryData.objets.Where(x => x.Rarete == typeRarete).ToArray();

            while(percentChance != 0)
            {
                if(percentChance > 1)
                {
                    int numberOfDraw = (int)Math.Floor(percentChance);

                    RandomDraw(ref awardObj, arr_objs, numberOfDraw);

                    percentChance -= numberOfDraw;
                }
                else if(percentChance < 1 && percentChance > 0)
                {
                    if(LuckyRoll(percentChance))
                    {
                        RandomDraw(ref awardObj, arr_objs, 1);
                    }
                    percentChance = 0;
                }
            }
        }

        private void RandomDraw(ref Dictionary<Objet, int> awardObj, Objet[] arr_objs, in int numberOfTime)
        {
            Random r = new Random();
            int max = arr_objs.Length - 1;
            Objet obj;
            for (int i = 0; i < numberOfTime; ++i)
            {
                // tirage
                obj = arr_objs[r.Next(max)];

                if(!awardObj.ContainsKey(obj))
                {
                    awardObj.Add(obj, 0);
                }
                awardObj[obj]++; ;
            }
        }

        private bool LuckyRoll(double percentChance)
        {
            Random r = new Random();
            return r.NextDouble() <= percentChance;
        }

        #endregion
    }

    public class ResultAward
    {
        [JsonInclude]
        public int RealAwardWood;
        [JsonInclude]
        public int RealAwardScrapMetal;
        [JsonInclude]
        public Dictionary<int, int> RealAwardObjets;

        public ResultAward(int realAwardWood, int realAwardScrapMetal, Dictionary<Objet, int> realAwardObjets)
        {
            RealAwardWood = realAwardWood;
            RealAwardScrapMetal = realAwardScrapMetal;
            RealAwardObjets = realAwardObjets.ToDictionary(x => x.Key.ObjetId, x => x.Value);
        }

        [JsonConstructor]
        public ResultAward(int realAwardWood, int realAwardScrapMetal, Dictionary<int, int> realAwardObjets)
        {
            RealAwardWood = realAwardWood;
            RealAwardScrapMetal = realAwardScrapMetal;
            RealAwardObjets = realAwardObjets;
        }

        public int CountNumberRarity(TypeRarete rarete)
        {
            int count = 0;
            foreach(var obj in RealAwardObjets.Where(x => NecessaryData.GetObjetById(x.Key).Rarete == rarete))
            {
                count += obj.Value;
            }
            return count;
        }
    }


    public struct SuccessBonus
    {
        private const double MULTIPLY_WOOD = 1.3;
        private const double MULTIPLY_SCRAP_METAL = 1.2;

        private const double COMMON_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE = 0.9;
        private const double MULTIPLY_COMMON_OBJ_PER_OBJECTIVE = 1.1;
        private const double MULTIPLY_COMMON_OBJ_ALL_OBJECTIVE = 1.6;

        private const double RARE_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE = 0.4;
        private const double MULTIPLY_RARE_OBJ_PER_OBJECTIVE = 1.05;
        private const double MULTIPLY_RARE_OBJ_ALL_OBJECTIVE = 1.5;

        private const double EPIC_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE = 0.05;
        private const double MULTIPLY_EDIC_OBJ_PER_OBJECTIVE = 1.025;
        private const double MULTIPLY_EPIC_OBJ_ALL_OBJECTIVE = 1.3;

        private const double MULTIPLY_LEGENDARY_OBJ_ALL_OBJECTIVE = 1.1;

        [JsonInclude]
        public double Multiply;
        [JsonInclude]
        public TypeSuccessBonus Type;

        public SuccessBonus(double multiply, TypeSuccessBonus type)
        {
            Multiply = multiply;
            Type = type;
        }

        public static SuccessBonus GenerateSBWood()
        {
            return new SuccessBonus(MULTIPLY_WOOD, TypeSuccessBonus.PerObjective);
        }

        public static SuccessBonus GenerateSBScrapMetal()
        {
            return new SuccessBonus(MULTIPLY_SCRAP_METAL, TypeSuccessBonus.PerObjective);
        }

        public static SuccessBonus GenerateSBCommonObj(Random r)
        {
            if (COMMON_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE >= r.NextDouble())
            {
                return new SuccessBonus(MULTIPLY_COMMON_OBJ_PER_OBJECTIVE, TypeSuccessBonus.PerObjective);
            }
            else
            {
                return new SuccessBonus(MULTIPLY_COMMON_OBJ_ALL_OBJECTIVE, TypeSuccessBonus.AllObjective);
            }
        }

        public static SuccessBonus GenerateSBRareObj(Random r)
        {
            if (RARE_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE >= r.NextDouble())
            {
                return new SuccessBonus(MULTIPLY_RARE_OBJ_PER_OBJECTIVE, TypeSuccessBonus.PerObjective);
            }
            else
            {
                return new SuccessBonus(MULTIPLY_RARE_OBJ_ALL_OBJECTIVE, TypeSuccessBonus.AllObjective);
            }
        }

        public static SuccessBonus GenerateSBEpicObj(Random r)
        {
            if (EPIC_OBJ_PERCENT_CHANCE_TO_BE_PER_OBJECTIVE >= r.NextDouble())
            {
                return new SuccessBonus(MULTIPLY_EDIC_OBJ_PER_OBJECTIVE, TypeSuccessBonus.PerObjective);
            }
            else
            {
                return new SuccessBonus(MULTIPLY_EPIC_OBJ_ALL_OBJECTIVE, TypeSuccessBonus.AllObjective);
            }
        }

        public static SuccessBonus GenerateSBLegendaryObj(Random r)
        {
            return new SuccessBonus(MULTIPLY_LEGENDARY_OBJ_ALL_OBJECTIVE, TypeSuccessBonus.AllObjective);
        }
    }

    public enum TypeSuccessBonus
    {
        PerObjective = 0,
        AllObjective = 1,
    }
}
