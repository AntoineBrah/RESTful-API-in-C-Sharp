using System.Web;
using System.Web.Mvc;

namespace Consultation_Reservation__Service_web_
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
