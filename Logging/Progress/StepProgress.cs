using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Socona.Log.Progress
{
  
public class StepProgress : FiniteProgress {
  /**
   * Title of the current step.
   */
  String stepTitle = "";

  /**
   * Constructor.
   * This constructor does not use a logger; initial logging will happen on the first BeginStep call.
   * 
   * @param total Total number of steps.
   */
  
  public StepProgress(int total) :
    base("Step", total){
  }
  
  /**
   * Constructor.
   * This constructor does not use a logger; initial logging will happen on the first BeginStep call.
   *
   * @param task Task Title
   * @param total Total number of steps.
   */
  
  public StepProgress(String task, int total) :
    base(task, total){
  }
  
  // No constructor with auto logging - call BeginStep() first

  
  public override StringBuilder AppendToBuffer(StringBuilder buf) {
    buf.Append(base.Task);
    if (IsComplete) {
      buf.Append(": complete.");
    } else {
      buf.Append(" #").Append(GetProcessed()+1).Append("/").Append(Total);
      buf.Append(": ").Append(GetStepTitle());
    }
    buf.Append("\n");
    return buf;
  }

  /**
   * Do a new step.
   * 
   * @param step Step number
   * @param stepTitle Step Title
   */
  [Obsolete]
  public void BeginStep(int step, String stepTitle) {
    SetProcessed(step - 1);
    this.stepTitle = stepTitle;
  }

  /**
   * Do a new step and log it
   * 
   * @param step Step number
   * @param stepTitle Step Title
   * @param logger Logger to report to.
   */
  public void BeginStep(int step, String stepTitle, Logging logger) {
    SetProcessed(step - 1);
    this.stepTitle = stepTitle;
    logger.Progress(this);
  }

  /**
   * Mark the progress as completed.
   */
  [Obsolete]
  public void SetCompleted() {
    SetProcessed(Total);
  }

  /**
   * Mark the progress as completed and log it.
   *
   * @param logger Logger to report to.
   */
  public void SetCompleted(Logging logger) {
    SetProcessed(Total);
    logger.Progress(this);
  }

  /**
   * @return the stepTitle
   */
  protected String GetStepTitle() {
    return stepTitle;
  }
}
}
