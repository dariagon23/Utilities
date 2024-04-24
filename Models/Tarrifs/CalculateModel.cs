using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utilities.Database.Models;

namespace Utilities.MVC.Models.Tarrifs
{
    public class CalculateModel
    {
        public int UtilId { get; set; }
        public int RegionId { get; set; }
       
        [Description("Показатели за прошлый месяц")]
        [Range(0, 99999.99, ErrorMessage = "Введите корректное число")]
        [Required(ErrorMessage = "Обязательно введите это число.")]
        public string PrevValue { get; set; }
        [Description("Показатели за текущий месяц")]
        [Range(0, 99999.99, ErrorMessage = "Введите корректное число")]
        [Required(ErrorMessage = "Обязательно введите это число.")]
        public string CurrentValue { get; set; }
        public List<SelectListItem> Regions { get; set; }
    }
}
