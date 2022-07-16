
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public abstract class BaseDto<TKey>
    {
        [Display(Name = "Key")]
        public TKey? Id { get; set; }
    }
}