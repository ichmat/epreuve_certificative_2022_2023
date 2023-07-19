using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public static class APIRoute
    {
        #region CONNEXION_CONTROLLER
        public const string ESTABLISH_CONNECTION = "/connect/establish";
        public const string SIGN_KEY = "/connect/sign";
        public const string RESET_TIME_OUT = "/connect/resetTimeout";
        public const string ATTEMPT_CONNECTION = "/connect/attempt";
        #endregion
        #region UTILISATEUR_CONTROLLER
        public const string CREATE_USER = "/user/create";
        public const string UPDATE_USER = "/user/update";
        public const string GET_USER_BY_TOKEN = "/user/getUserByToken";
        #endregion
        #region STAT_CONTROLLER
        public const string SAVE_STAT = "/stat/saveStat";
        public const string GET_STAT_BY_USER_ID = "/stat/getStatByUserId";
        public const string GENERATE_ACTIVITY_ENGINE = "/stat/generateActivityEngine";
        #endregion
        #region VILLAGE
        public const string GET_ENTIRE_VILLAGE = "/village/getTheEntireUserVillage";
        public const string GET_NECESSARY_DATA_VILLAGE = "/village/getNecessaryDataVillage";
        public const string CREATE_USER_VILLAGE = "/village/createUserVillage";
        #endregion
        #region BUILDING
        public const string CREATE_BUILDING = "/building/createBuilding";
        public const string PLACE_BUILDING = "/building/placeBuilding";
        public const string UNPLACE_BUILDING = "/building/unplaceBuilding";
        #endregion
        #region COURSES
        public const string GET_COURSES_ORDERED_BY_DATE_DESC = "/courses/getCoursesOrderedByDateDesc";
        #endregion
    }
}
