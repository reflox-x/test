namespace pawledger.Services;

public static class LanguageService
{
    public static string CurrentLanguage =>
        Preferences.Default.Get("language", "English");

    public static string GetText(string key)
    {
        bool isChinese = CurrentLanguage == "Chinese";

        return key switch
        {
            // Home
            "AppTitle" => isChinese ? "爪账本" : "PawLedger",
            "Income" => isChinese ? "收入" : "Income",
            "Expense" => isChinese ? "支出" : "Expense",
            "Balance" => isChinese ? "余额" : "Balance",
            "Summary" => isChinese ? "摘要" : "Summary",
            "RecentRecords" => isChinese ? "最近记录" : "Recent Records",
            "Total" => isChinese ? "总计" : "Total",
            "Food" => isChinese ? "食物" : "Food",
            "Pet" => isChinese ? "宠物" : "Pet",

            // Add Record
            "AddRecord" => isChinese ? "新增记录" : "Add Record",
            "SaveRecord" => isChinese ? "保存记录" : "Save Record",
            "SelectDate" => isChinese ? "选择日期" : "Select Date",
            "CategoryPlaceholder" => isChinese ? "输入分类，例如 Food 或 Pet" : "Enter category, e.g. Food or Pet",

            // Plan
            "Plan" => isChinese ? "计划" : "Plan",
            "MyPlans" => isChinese ? "我的计划" : "My Plans",
            "AddToPlan" => isChinese ? "添加计划" : "Add to plan",
            "AddSavings" => isChinese ? "增加存款" : "Add Savings",
            "EditPlan" => isChinese ? "编辑计划" : "Edit Plan",
            "DeletePlan" => isChinese ? "删除计划" : "Delete Plan",
            "Saved" => isChinese ? "已存" : "Saved",
            "Deadline" => isChinese ? "截止日期" : "Deadline",
            "Target" => isChinese ? "目标" : "Target",

            // Settings
            "Settings" => isChinese ? "设置" : "Settings",
            "Language" => isChinese ? "语言" : "Language",
            "Currency" => isChinese ? "货币" : "Currency",
            "DarkMode" => isChinese ? "深色模式" : "Dark Mode",

            _ => key
        };
    }
}