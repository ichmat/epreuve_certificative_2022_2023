using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Services.GeneralMessage.Args
{
    public class EPGetCourses : EndPointArgs
    {
        [JsonInclude]
        public int StartIndex;

        [JsonInclude]
        public int EndIndex;

        public EPGetCourses(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public override string Route() => APIRoute.GET_COURSES_ORDERED_BY_DATE_DESC;
    }
}
