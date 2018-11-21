namespace Sitecore.Support.ContentTesting.Workflows
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.ContentTesting;
  using Sitecore.ContentTesting.Tests;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Pipelines;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Workflows.Simple;

  public class RemoveTestsAction : Sitecore.ContentTesting.Workflows.RemoveTestsAction
  {
    private const string WORKFLOW_ARGS_ID = "RemoveTestAction-workflow-args";
    private const string TESTS_ID = "RemoveTestAction-tests-args";
    protected override void ConfirmTestRemoval(IEnumerable<ITest> tests, WorkflowPipelineArgs args)
    {
      var clientPipelineArgs = new ClientPipelineArgs();

      clientPipelineArgs.CustomData.Add(WORKFLOW_ARGS_ID, args);
      clientPipelineArgs.CustomData.Add(TESTS_ID, tests);

      Context.ClientPage.Start(this, "StartConfirm", clientPipelineArgs);
    }
    protected new void StartConfirm(ClientPipelineArgs args)
    {
      var workflowArgs = (WorkflowPipelineArgs)args.CustomData[WORKFLOW_ARGS_ID];
      var tests = (IEnumerable<ITest>)args.CustomData[TESTS_ID];

      if (args.IsPostBack)
      {
        if (!args.HasResult || (args.HasResult && args.Result == "cancel"))
        {
          workflowArgs.AbortPipeline();
          PipelineFactory.GetSuspendedPipelines().Remove(workflowArgs.Pipeline.ID);
        }
        else if (args.HasResult)
        {
          if (args.Result == "yes")
          {
            foreach (var test in tests)
            {
              test.Remove(this.DeleteTestDefinition);
            }
          }
          try
          {
            workflowArgs.Pipeline.Resume();
          }
          catch (System.ArgumentNullException exc)
          {
            Log.Info("Sitecore.Support.294335 has received an error caused by bug #294335. Error message: " + exc.Message + " . This is caused by removing an incomplete test via a workflow action. This can be ignored.", this);
          }
        }
      }
      else
      {
        SheerResponse.YesNoCancel(
          Translate.Text(Texts.WOULD_YOU_LIKE_TO_REMOVE_THE_FOLLOWING_TESTS) + "\r\n\r\n" + string.Join("\r\n", from test in tests select "- " + test.Name),
          "500px",
          "300px",
          true
        );
        args.WaitForPostBack();
      }
    }
  }
}