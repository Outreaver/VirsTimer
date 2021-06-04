using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Threading.Tasks;
using VirsTimer.Core.Models;
using VirsTimer.Core.Services.Login;
using VirsTimer.DesktopApp.Views;
 
namespace VirsTimer.DesktopApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILoginRepository _loginRepository;
 
        [Reactive]
        public bool IsBusy { get; set; } = false;
 
        [Reactive]
        public bool IsResponseUnsccesfull { get; set; } = false;
 
        [Reactive]
        public string LoginName { get; set; } = string.Empty;
 
        [Reactive]
        public string LoginPassowd { get; set; } = string.Empty;
 
        public ReactiveCommand<Window, Unit> AcceptLoginCommand { get; }
 
        public LoginViewModel(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
 
            var acceptLoginEnabled = this.WhenAnyValue(
                x => x.LoginName,
                x => x.LoginPassowd,
                (name, password) => !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(password));
            AcceptLoginCommand = ReactiveCommand.CreateFromTask<Window>(AcceptLoginAsync, acceptLoginEnabled);
        }
 
        private async Task AcceptLoginAsync(Window parent)
        {
            IsBusy = true;
 
            var request = new LoginRequest(LoginName, LoginPassowd);
 
            var response = await _loginRepository.LoginAsync(request).ConfigureAwait(false);
            if (response.Succesfull)
            {
                var mainWinow = new MainWindow();
                mainWinow.Show();
                parent.Close();
            }
 
            ShowUnsuccesfullControlAsync();
            IsBusy = false;
        }
 
        private async void ShowUnsuccesfullControlAsync()
        {
            if (IsResponseUnsccesfull)
                return;
 
            IsResponseUnsccesfull = true;
            await Task.Delay(3000).ConfigureAwait(false);
            IsResponseUnsccesfull = false;
        }
    }
}