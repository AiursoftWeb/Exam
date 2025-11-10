using System.ComponentModel.DataAnnotations;
using Aiursoft.Exam.Authorization;
using Aiursoft.Exam.Entities;
using Aiursoft.Exam.Services;
using Aiursoft.UiStack.Layout;
using Aiursoft.UiStack.Navigation;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Exam.Controllers;

public class QuestionBankViewModel : UiStackLayoutViewModel
{
    public QuestionBankViewModel()
    {
        PageTitle = "All Questions";
    }

    public required List<Question> Questions { get; set; } = new();
}

public class CreateQuestionViewModel : UiStackLayoutViewModel
{
    public CreateQuestionViewModel()
    {
        PageTitle = "Create New Question";
    }
}

public class DeleteQuestionViewModel : UiStackLayoutViewModel
{
    public DeleteQuestionViewModel()
    {
        PageTitle = "Delete Question";
    }

    public required Question Question { get; init; }
}

public class ChoiceViewModel : UiStackLayoutViewModel
{
    public ChoiceViewModel()
    {
        PageTitle = "Choice Option";
    }

    public required Guid Id { get; init; }

    [Required(ErrorMessage = "Option content is required.")]
    [StringLength(2048, ErrorMessage = "Option content cannot exceed 2048 characters.")]
    public required string Content { get; set; } = string.Empty;
    public required bool IsCorrect { get; set; }
}

public class EditChoiceViewModel : UiStackLayoutViewModel
{
    public EditChoiceViewModel()
    {
        PageTitle = "Edit Choice Question";
    }

    public required Guid Id { get; set; }
    [Required(ErrorMessage = "Question content is required.")]
    [StringLength(4096, ErrorMessage = "Question content cannot exceed 4096 characters.")]
    public required string Content { get; set; } = string.Empty;
    public List<ChoiceViewModel> Choices { get; set; } = [];
}

public class EditFillViewModel : UiStackLayoutViewModel
{
    public EditFillViewModel()
    {
        PageTitle = "Edit Fill-in-the-Blank Question";
    }

    public required Guid Id { get; set; }
    [Required(ErrorMessage = "Question text is required.")]
    [StringLength(8192, ErrorMessage = "Question text cannot exceed 8192 characters.")]
    public required string TextWithBlanks { get; set; } = string.Empty;

    [Required(ErrorMessage = "The correct answer is required.")]
    [StringLength(1024, ErrorMessage = "The answer cannot exceed 1024 characters.")]
    public required string Answer { get; set; } = string.Empty;
}

[LimitPerMin]
[Authorize(Policy = AppPermissionNames.CanAdminQuestionsBank)]
public class QuestionsController(TemplateDbContext context) : Controller
{
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "Exam",
        CascadedLinksIcon = "database",
        CascadedLinksOrder = 1,
        LinkText = "Question Bank",
        LinkOrder = 1)]
    public async Task<IActionResult> Index()
    {
        var questions = await context.Questions
            .OrderByDescending(q => q.CreationTime)
            .ToListAsync();

        var model = new QuestionBankViewModel { Questions = questions };
        return this.StackView(model);
    }

    // --- 创建 (Create) 流程 ---

    // 第 1 步：显示类型选择
    // GET: /AdminQuestions/Create
    public IActionResult Create()
    {
        // 此视图 (Create.cshtml) 非常简单，只有两个链接：
        // 1. <a asp-action="CreateChoice">创建选择题</a>
        // 2. <a asp-action="CreateFill">创建填空题</a>
        return this.StackView(new CreateQuestionViewModel());
    }

    // 第 2 步 (分支 A)：显示“创建选择题”的表单
    // GET: /AdminQuestions/CreateChoice
    public IActionResult CreateChoice()
    {
        var model = new EditChoiceViewModel
        {
            Id = Guid.NewGuid(),
            Content = string.Empty,
            // 预先添加 4 个空选项，优化体验
            Choices =
            [
                new ChoiceViewModel
                {
                    Id = Guid.NewGuid(),
                    Content = string.Empty,
                    IsCorrect = false
                },

                new ChoiceViewModel
                {
                    Id = Guid.NewGuid(),
                    Content = string.Empty,
                    IsCorrect = false
                },

                new ChoiceViewModel
                {
                    Id = Guid.NewGuid(),
                    Content = string.Empty,
                    IsCorrect = false
                },

                new ChoiceViewModel
                {
                    Id = Guid.NewGuid(),
                    Content = string.Empty,
                    IsCorrect = false
                }

            ]
        };
        // 复用 EditChoice.cshtml 视图
        return this.StackView(viewName: nameof(EditChoice), model: model);
    }

    // 第 3 步 (分支 A)：处理“创建选择题”的 POST
    // POST: /AdminQuestions/CreateChoice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateChoice(EditChoiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.StackView(viewName: nameof(EditChoice), model: model);
        }

        // 映射 ViewModel 到 Entity
        var newQuestion = new ChoiceQuestion
        {
            Id = Guid.NewGuid(),
            Discriminator = nameof(ChoiceQuestion), // 严格遵循你的实体设计
            Content = model.Content,
            Choices = model.Choices.Select(c => new Choice
            {
                Id = Guid.NewGuid(),
                Content = c.Content,
                IsCorrect = c.IsCorrect,
                QuestionId = null // 你的设计允许 QuestionId 为 null
            }).ToList()
        };

        context.ChoiceQuestions.Add(newQuestion);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // 第 2 步 (分支 B)：显示“创建填空题”的表单
    // GET: /AdminQuestions/CreateFill
    public IActionResult CreateFill()
    {
        var model = new EditFillViewModel
        {
            Id = Guid.NewGuid(),
            Answer = string.Empty,
            TextWithBlanks = string.Empty,
        };
        // 复用 EditFill.cshtml 视图
        return this.StackView(viewName: nameof(EditFill), model: model);
    }

    // 第 3 步 (分支 B)：处理“创建填空题”的 POST
    // POST: /AdminQuestions/CreateFill
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFill(EditFillViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.StackView(viewName: nameof(EditFill), model: model);
        }

        var newQuestion = new FillInBlankQuestion
        {
            Id = Guid.NewGuid(),
            Discriminator = nameof(FillInBlankQuestion),
            TextWithBlanks = model.TextWithBlanks,
            Answer = model.Answer
        };

        context.FillInBlankQuestion.Add(newQuestion);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // --- 编辑 (Edit) 流程 ---

    // 关键动作：这是一个“路由”动作 (Router Action)
    // 它不等同于 MarkToHtml 的 Edit()，因为它不返回视图，
    // 而是根据问题类型，重定向到 *正确* 的编辑动作。
    // GET: /AdminQuestions/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        // 检查类型并重定向
        if (question is ChoiceQuestion)
        {
            return RedirectToAction(nameof(EditChoice), new { id });
        }

        if (question is FillInBlankQuestion)
        {
            return RedirectToAction(nameof(EditFill), new { id });
        }

        return NotFound("Unsupported question type.");
    }

    // 实际的“编辑选择题”表单
    // GET: /AdminQuestions/EditChoice/{id}
    public async Task<IActionResult> EditChoice(Guid id)
    {
        var question = await context.ChoiceQuestions
            .Include(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null) return NotFound();

        // 映射 Entity 到 ViewModel
        var model = new EditChoiceViewModel
        {
            Id = question.Id,
            Content = question.Content,
            Choices = question.Choices.Select(c => new ChoiceViewModel
            {
                Id = c.Id,
                Content = c.Content,
                IsCorrect = c.IsCorrect
            }).ToList()
        };

        return this.StackView(model); // 渲染 EditChoice.cshtml
    }

    // 处理“编辑选择题”的 POST
    // POST: /AdminQuestions/EditChoice/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditChoice(Guid id, EditChoiceViewModel model)
    {
        if (id != model.Id) return BadRequest();

        // 此时，model.Choices 已经通过 IValidatableObject
        // 确保了“至少一个正确答案”的规则
        if (!ModelState.IsValid)
        {
            return this.StackView(model);
        }

        var questionInDb = await context.ChoiceQuestions
            .Include(q => q.Choices) // 必须 Include(Choices)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questionInDb == null) return NotFound();

        // 1. 更新题干
        questionInDb.Content = model.Content;

        // [FIXED] --- 复杂但必要的同步逻辑 开始 ---

        var modelChoices = model.Choices;
        var dbChoices = questionInDb.Choices.ToList();

        // 2. 删除 (Delete): 找出数据库中有、但新提交的 model 中没有的选项
        var choicesToRemove = dbChoices
            .Where(dbChoice => !modelChoices.Any(mChoice => mChoice.Id == dbChoice.Id))
            .ToList();

        context.Choices.RemoveRange(choicesToRemove);

        // 3. 更新 (Update) 和 添加 (Add)
        foreach (var modelChoice in modelChoices)
        {
            // 忽略内容为空的行
            if (string.IsNullOrWhiteSpace(modelChoice.Content))
            {
                // 如果这个空行在数据库中存在，删除它
                var emptyDbChoice = dbChoices.FirstOrDefault(c => c.Id == modelChoice.Id);
                if (emptyDbChoice != null)
                {
                    context.Choices.Remove(emptyDbChoice);
                }
                continue;
            }

            var dbChoice = dbChoices.FirstOrDefault(c => c.Id == modelChoice.Id);

            if (dbChoice != null)
            {
                // 3a. 更新 (Update) 现有的选项
                dbChoice.Content = modelChoice.Content;
                dbChoice.IsCorrect = modelChoice.IsCorrect;
            }
            else
            {
                // 3b. 添加 (Add) 新的选项
                var newChoice = new Choice
                {
                    Id = Guid.NewGuid(),
                    Content = modelChoice.Content,
                    IsCorrect = modelChoice.IsCorrect,
                    QuestionId = questionInDb.Id // 关联到这个题目
                };
                context.Choices.Add(newChoice);
            }
        }
        // --- 复杂但必要的同步逻辑 结束 ---

        context.ChoiceQuestions.Update(questionInDb);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // 实际的“编辑填空题”表单
    // GET: /AdminQuestions/EditFill/{id}
    public async Task<IActionResult> EditFill(Guid id)
    {
        var question = await context.FillInBlankQuestion.FindAsync(id);
        if (question == null) return NotFound();

        var model = new EditFillViewModel
        {
            Id = question.Id,
            TextWithBlanks = question.TextWithBlanks,
            Answer = question.Answer
        };

        return this.StackView(model); // 渲染 EditFill.cshtml
    }

    // 处理“编辑填空题”的 POST
    // POST: /AdminQuestions/EditFill/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFill(Guid id, EditFillViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            return this.StackView(model);
        }

        // 填空题的更新很简单，直接映射
        var questionInDb = await context.FillInBlankQuestion.FindAsync(id);
        if (questionInDb == null) return NotFound();

        questionInDb.TextWithBlanks = model.TextWithBlanks;
        questionInDb.Answer = model.Answer;

        context.FillInBlankQuestion.Update(questionInDb);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // --- 删除 (Delete) 流程 ---

    // GET: /AdminQuestions/Delete/{id} (等同于 MarkToHtml)
    public async Task<IActionResult> Delete(Guid id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        var model = new DeleteQuestionViewModel { Question = question };
        return this.StackView(model); // 渲染 Delete.cshtml
    }

    // POST: /AdminQuestions/Delete/{id} (等同于 MarkToHtml)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();
        }

        context.Questions.Remove(question); // EF Core TPH 会自动处理
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
