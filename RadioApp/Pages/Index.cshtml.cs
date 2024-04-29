using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RadioApp.Pages
{
    public class IndexModel : PageModel
    {
        /*
        public void OnGet()
        {
        
        }
        */

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPost(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                // �������������� �� �������� ���� � ���������� username
                return RedirectToPage("/Chat", new { username });
            }
            else
            {
                // ���� ��� ������������ ������, �������� �� �������� �����
                return Page();
            }
        }

    }
}
