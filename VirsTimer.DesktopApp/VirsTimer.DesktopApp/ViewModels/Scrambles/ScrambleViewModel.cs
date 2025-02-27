﻿using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VirsTimer.Core.Models;
using VirsTimer.Core.Services;
using VirsTimer.Core.Services.Scrambles;
using VirsTimer.DesktopApp.ViewModels.Common;
using VirsTimer.DesktopApp.Views.Common;
using VirsTimer.Scrambles;

namespace VirsTimer.DesktopApp.ViewModels.Scrambles
{
    public class ScrambleViewModel : ViewModelBase
    {
        private readonly IScrambleGenerator _scrambleGenerator;
        private readonly ICustomScrambleGeneratorsCollector _customScrambleGeneratorsCollector;

        private ICustomScrambleGenerator? _customScrambleGenerator;
        private bool _isCustom = false;
        private Event _currentEvent = null!;
        private Queue<Scramble> _scrambles = null!;

        public bool Loading { get; set; }

        [Reactive]
        public Scramble CurrentScramble { get; set; } = new Scramble();

        [Reactive]
        public bool GenerateScrambleInfoVisible { get; set; }

        public ReactiveCommand<Window, Unit> OpenGenerateScrambleInfoCommand { get; }

        public ScrambleViewModel(
            IScrambleGenerator? scrambleGenerator = null,
            ICustomScrambleGeneratorsCollector? customScrambleGeneratorsCollector = null)
        {
            _scrambleGenerator = scrambleGenerator ?? Ioc.GetService<IScrambleGenerator>();
            _customScrambleGeneratorsCollector = customScrambleGeneratorsCollector ?? Ioc.GetService<ICustomScrambleGeneratorsCollector>();

            OpenGenerateScrambleInfoCommand = ReactiveCommand.CreateFromTask<Window>(OpenGenerateScrambleInfo);
        }

        public override async Task<bool> ConstructAsync()
        {
            await ChangeEventAsync(_currentEvent);
            return true;
        }

        public async Task ChangeEventAsync(Event newEvent)
        {
            Loading = true;
            IsBusy = true;
            _currentEvent = newEvent;
            GenerateScrambleInfoVisible = false;

            if (Core.Constants.Events.Predefined.Contains(newEvent.Name))
            {
                _isCustom = false;
                var scrabmles = await _scrambleGenerator.GenerateScrambles(_currentEvent, 4);
                if (scrabmles.IsSuccesfull)
                {
                    _scrambles = new Queue<Scramble>(scrabmles.Value!);
                    await GetNextScrambleAsync();

                    IsBusy = false;
                    Loading = false;
                    return;
                }

                await ShutdownDialogHandleAsync("Nie można pobrać scrambli z serwera. Sprawdź połączenie internetowe. Jeżeli problem będzie się powtarzał zgłoś problem na https://github.com/VirsTimerCreators/VirsTimer.");
                IsBusy = false;
                Loading = false;
                return;
            }

            _isCustom = true;
            _customScrambleGenerator = _customScrambleGeneratorsCollector.GetCustomScrambleGenerators().FirstOrDefault(x => x.EventName == newEvent.Name);
            await GetNextScrambleAsync();

            if (string.IsNullOrWhiteSpace(CurrentScramble.Value))
                GenerateScrambleInfoVisible = true;

            IsBusy = false;
            Loading = false;
        }

        public async Task GetNextScrambleAsync()
        {
            if (_isCustom)
            {
                try
                {
                    CurrentScramble = _customScrambleGenerator?.GenerateScramble() ?? new Scramble();
                }
                catch
                {
                    CurrentScramble = new Scramble();
                }

                return;
            }

            if (_scrambles.Count == 0)
                await ShutdownDialogHandleAsync("Nie można pobrać scrambli z serwera.");

            CurrentScramble = _scrambles.Dequeue();
            if (_scrambles.Count < 3)
            {
                IsBusy = true;
                var generatedScrambles = await _scrambleGenerator.GenerateScrambles(_currentEvent, 5);
                if (generatedScrambles.IsSuccesfull is false)
                {
                    IsBusy = false;
                    return;
                }

                foreach (var scramble in generatedScrambles.Value!)
                    _scrambles.Enqueue(scramble);
                IsBusy = false;
            }
        }

        public static async Task OpenGenerateScrambleInfo(Window window)
        {
            var message = string.Join("\n\n",
                "1. Stworzyć bibliotekę klas (Class Library) w technologii .NET 6.",
                "2. Dodać paczkę VirsTimer.Scrambles z witryny nuget.org: \n" +
                "https://www.nuget.org/packages/VirsTimer.Scrambles/1.0.0 \n" +
                "do stworzonej biblioteki.",
                "3. Zaimplementować interfejs ICustomScrambleGenerator gdzie w właściwości EventName należy umieścić nazwę stworzonej konkurencji w aplikacji.\n" +
                "Biblioteka może zawierać wiele implemenacjia interfejsu, ale tylko jedną dla danej konkurencji.",
                "4. Zmienić nazwę pliku .dll zbudowanej biblioteki na ScrambleGenerators.dll.",
                "5. Umieścić plik ScrambleGenerators.dll w głównym folderze aplikacji VirsTimer.",
                "6. Zresetować aplikację.");

            var infoBoxViewModel = new InfoBoxViewModel
            {
                Message = message
            };

            var infoBox = new InfoBox
            {
                ViewModel = infoBoxViewModel
            };

            await infoBox.ShowDialog(window);
        }
    }
}