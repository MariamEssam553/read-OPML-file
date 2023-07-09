using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HW4.Pages;

public class IndexModel : PageModel
{
    public async Task OnGetAsync()
    {
        await readOPML();
    }

    public async Task readOPML()
    {

    }
}
