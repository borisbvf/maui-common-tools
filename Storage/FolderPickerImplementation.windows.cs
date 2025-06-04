using System.Diagnostics;
using Windows.Storage.Pickers;

namespace MauiCommonTools.Storage;
public sealed partial class FolderPickerImplementation : IFolderPicker
{
	static async Task<string> InternalPickAsync(string initialPath, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var folderPicker = new Windows.Storage.Pickers.FolderPicker()
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary
		};
		WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, Process.GetCurrentProcess().MainWindowHandle);
		folderPicker.FileTypeFilter.Add("*");
		var folderPickerOperation = folderPicker.PickSingleFolderAsync();

		void CancelFolderPickerOperation()
		{
			folderPickerOperation.Cancel();
		}

		await using var _ = cancellationToken.Register(CancelFolderPickerOperation);
		var folder = await folderPickerOperation;
		if (folder is null)
		{
			throw new FolderPickerException("Operation cancelled or Folder doesn't exist.");
		}

		return folder.Path;
	}

	static async Task<string> InternalPickAsync(CancellationToken cancellationToken)
	{
		return await InternalPickAsync(string.Empty, cancellationToken);
	}
}
