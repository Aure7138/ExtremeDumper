using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ExtremeDumper.Forms;

namespace ExtremeDumper;

/// <summary>
/// 全局错误捕获
/// </summary>
static class GlobalExceptionCatcher {
	static bool isStarted;

	/// <summary>
	/// 自动捕获所有异常
	/// </summary>
	public static void Catch() {
		if (!isStarted) {
			Application.ThreadException += (object sender, System.Threading.ThreadExceptionEventArgs e) => ShowDetailException(e.Exception);
			AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => ShowDetailException((Exception)e.ExceptionObject);
			isStarted = true;
		}
	}

	static void ShowDetailException(Exception exception) {
		var sb = new StringBuilder();
		DumpException(exception, sb);
		MessageBoxStub.Show(sb.ToString(), MessageBoxIcon.Error);
	}

	static void DumpException(Exception exception, StringBuilder sb) {
		exception ??= new ArgumentNullException(nameof(exception), "<No exception object>");
		sb.AppendLine($"Type: {Environment.NewLine}{exception.GetType().FullName}");
		sb.AppendLine($"Message: {Environment.NewLine}{exception.Message}");
		sb.AppendLine($"Source: {Environment.NewLine}{exception.Source}");
		sb.AppendLine($"StackTrace: {Environment.NewLine}{exception.StackTrace}");
		sb.AppendLine($"TargetSite: {Environment.NewLine}{exception.TargetSite}");
		sb.AppendLine("----------------------------------------");
		if (exception.InnerException is not null)
			DumpException(exception.InnerException, sb);
		if (exception is ReflectionTypeLoadException reflectionTypeLoadException) {
			foreach (var loaderException in reflectionTypeLoadException.LoaderExceptions)
				DumpException(loaderException, sb);
		}
	}
}
