namespace Level2_ASPNetCore_MVC_TaskManager.Models
{
    public class ContactViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;

        public bool IsSubmitted { get; set; } = false;
    }
}
