using System.Collections.ObjectModel;

namespace EnglishCopilot.ViewModels;

public partial class ChatListVM : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<ChatMessage> chatMessages;

    public ChatListVM()
    {
    }

}


public class ChatMessage
{
    public string UserName { get; set; }
    public string Message { get; set; }
}