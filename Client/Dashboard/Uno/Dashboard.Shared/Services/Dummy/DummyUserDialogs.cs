using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanet.SmartSkating.Dashboard.Services.Dummy
{
    class DummyUserDialogs : IUserDialogs
    {
        public IDisposable ActionSheet(ActionSheetConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<string> ActionSheetAsync(string title, string cancel, string destructive, CancellationToken? cancelToken = null, params string[] buttons)
        {
            throw new NotImplementedException();
        }

        public IDisposable Alert(string message, string title = null, string okText = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Alert(AlertConfig config)
        {
            throw new NotImplementedException();
        }

        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task AlertAsync(AlertConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Confirm(ConfirmConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable DatePrompt(DatePromptConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<DatePromptResult> DatePromptAsync(DatePromptConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<DatePromptResult> DatePromptAsync(string title = null, DateTime? selectedDate = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public void HideLoading()
        {
            throw new NotImplementedException();
        }

        public IProgressDialog Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType? maskType = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Login(LoginConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResult> LoginAsync(string title = null, string message = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResult> LoginAsync(LoginConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public IProgressDialog Progress(ProgressDialogConfig config)
        {
            throw new NotImplementedException();
        }

        public IProgressDialog Progress(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType? maskType = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Prompt(PromptConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null, string placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<PromptResult> PromptAsync(PromptConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public void ShowLoading(string title = null, MaskType? maskType = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable TimePrompt(TimePromptConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<TimePromptResult> TimePromptAsync(TimePromptConfig config, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<TimePromptResult> TimePromptAsync(string title = null, TimeSpan? selectedTime = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Toast(string title, TimeSpan? dismissTimer = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable Toast(ToastConfig cfg)
        {
            throw new NotImplementedException();
        }
    }
}
