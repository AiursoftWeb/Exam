using Aiursoft.Exam.Entities;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Exam.Models.UsersViewModels;

public class UserWithRolesViewModel
{
    public required User User { get; set; }
    public required IList<string> Roles { get; set; }
}

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Users";
    }

    public required List<UserWithRolesViewModel> Users { get; set; }
}
