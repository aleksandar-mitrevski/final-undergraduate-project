// Type: System.ComponentModel.BackgroundWorker
// Assembly: System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// Assembly location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\Profile\WindowsPhone71\System.dll

namespace System.ComponentModel
{
  /// <summary>
  /// Runs an operation on a separate thread.
  /// </summary>
  public class BackgroundWorker
  {
    /// <summary>
    /// Requests cancellation of a pending background operation.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException"><see cref="P:System.ComponentModel.BackgroundWorker.WorkerSupportsCancellation"/> is false. </exception>
    public void CancelAsync();
    /// <summary>
    /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.DoWorkEventArgs"/> that contains the event data.</param>
    protected virtual void OnDoWork(DoWorkEventArgs e);
    /// <summary>
    /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.ProgressChangedEventArgs"/> that contains the event data.</param>
    protected virtual void OnProgressChanged(ProgressChangedEventArgs e);
    /// <summary>
    /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.RunWorkerCompleted"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.RunWorkerCompletedEventArgs"/> that contains the event data.</param>
    protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e);
    /// <summary>
    /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged"/> event.
    /// </summary>
    /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is completed.</param><exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.BackgroundWorker.WorkerReportsProgress"/> property is set to false. </exception>
    public void ReportProgress(int percentProgress);
    /// <summary>
    /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged"/> event.
    /// </summary>
    /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is completed.</param><param name="userState">The state object passed to <see cref="M:System.ComponentModel.BackgroundWorker.RunWorkerAsync(System.Object)"/>.</param><exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.BackgroundWorker.WorkerReportsProgress"/> property is set to false. </exception>
    public void ReportProgress(int percentProgress, object userState);
    /// <summary>
    /// Starts running a background operation.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException"><see cref="P:System.ComponentModel.BackgroundWorker.IsBusy"/> is true.</exception>
    public void RunWorkerAsync();
    /// <summary>
    /// Starts running a background operation and includes a parameter for use by the background operation.
    /// </summary>
    /// <param name="argument">A parameter for use by the background operation in the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork"/> event handler.</param><exception cref="T:System.InvalidOperationException"><see cref="P:System.ComponentModel.BackgroundWorker.IsBusy"/> is true.</exception>
    public void RunWorkerAsync(object argument);
    /// <summary>
    /// Gets a value that indicates whether the application has requested cancellation of a background operation.
    /// </summary>
    /// 
    /// <returns>
    /// true if the application has requested cancellation of a background operation; otherwise, false. The default is false.
    /// </returns>
    public bool CancellationPending { get; }
    /// <summary>
    /// Gets a value that indicates whether the <see cref="T:System.ComponentModel.BackgroundWorker"/> is running a background operation.
    /// </summary>
    /// 
    /// <returns>
    /// true, if the <see cref="T:System.ComponentModel.BackgroundWorker"/> is running a background operation; otherwise, false.
    /// </returns>
    public bool IsBusy { get; }
    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="T:System.ComponentModel.BackgroundWorker"/> can report progress updates.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.ComponentModel.BackgroundWorker"/> supports progress updates; otherwise false. The default is false.
    /// </returns>
    public bool WorkerReportsProgress { get; set; }
    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="T:System.ComponentModel.BackgroundWorker"/> supports asynchronous cancellation.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.ComponentModel.BackgroundWorker"/> supports cancellation; otherwise false. The default is false.
    /// </returns>
    public bool WorkerSupportsCancellation { get; set; }
    /// <summary>
    /// Occurs when <see cref="M:System.ComponentModel.BackgroundWorker.RunWorkerAsync"/> is called.
    /// </summary>
    public event DoWorkEventHandler DoWork;
    /// <summary>
    /// Occurs when <see cref="Overload:System.ComponentModel.BackgroundWorker.ReportProgress"/> is called.
    /// </summary>
    public event ProgressChangedEventHandler ProgressChanged;
    /// <summary>
    /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
    /// </summary>
    public event RunWorkerCompletedEventHandler RunWorkerCompleted;
  }
}
