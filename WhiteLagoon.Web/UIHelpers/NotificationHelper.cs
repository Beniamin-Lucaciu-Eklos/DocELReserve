using Microsoft.AspNetCore.Mvc;

namespace WhiteLagoon.Web.UIHelpers
{
    public static class NotificationHelper
    {
        public static void ShowSuccess(this Controller controller, string message)
        {
            controller.TempData["SuccesMessage"] = message;
        }

        public static void ShowError(this Controller controller, string message)
        {
            controller.TempData["ErrorMessage"] = message;
        }
    }
}
