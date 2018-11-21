namespace Sitecore.Support.Hooks
{
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.Events.Hooks;
  using Sitecore.SecurityModel;
  using System;
  public class UpdateRemoveTestsAction : IHook
  {
    public void Initialize()
    {
      using (new SecurityDisabler())
      {
        var databaseName = "master";
        var itemPath = "/sitecore/templates/System/Content Testing/Workflow/Remove Tests Action/__Standard Values";
        var fieldName = "Type string";

        // full name of the class is enough
        var fieldValue = "Sitecore.Support.ContentTesting.Workflows.RemoveTestAction,Sitecore.Support.294335";

        var database = Factory.GetDatabase(databaseName);
        var item = database.GetItem(itemPath);

        if (string.Equals(item[fieldName], fieldValue, StringComparison.Ordinal))
        {
          // already installed
          return;
        }
         //test changes
        Log.Info($"Installing {fieldValue}", this);
        item.Editing.BeginEdit();
        item[fieldName] = fieldValue;
        item.Editing.EndEdit();
      }
    }
  }
}