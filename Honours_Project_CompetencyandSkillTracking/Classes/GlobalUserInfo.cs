using System.ComponentModel.DataAnnotations;
using System.Globalization;

    public class GlobalUserInfo
    {
        [Required]
        public int Id { get; set; }
        public void ApplyChanges(int aId)
        {
            this.Id = aId;

        }
    }
